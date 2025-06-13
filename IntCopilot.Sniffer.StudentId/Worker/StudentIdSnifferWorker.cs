using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IntCopilot.Sniffer.StudentId.Core;
using IntCopilot.Sniffer.StudentId.Models;

namespace IntCopilot.Sniffer.StudentId.Worker
{
    public class StudentIdSnifferWorker : BackgroundService
    {
        private readonly ILogger<StudentIdSnifferWorker> _logger;
        private readonly IStudentIdSniffer _sniffer;

        public StudentIdSnifferWorker(ILogger<StudentIdSnifferWorker> logger, IStudentIdSniffer sniffer)
        {
            _logger = logger;
            _sniffer = sniffer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tcs = new TaskCompletionSource();
            
            // 1. 先订阅事件
            using var subscription = _sniffer.StateChanges
                .Where(s => s.Status is SnifferStatus.Completed or SnifferStatus.Failed)
                .Take(1) // 只关心第一个终结状态
                .Subscribe(
                    state =>
                    {
                        _logger.LogInformation("Sniffer process concluded with status: {Status}", state.Status);
                        if(state.LastError != null)
                        {
                            _logger.LogError(state.LastError, "Sniffer failed with an exception.");
                        }
                        tcs.TrySetResult();
                    },
                    error => tcs.TrySetException(error)
                );
            
            // 2. 检查当前状态，防止在启动前就已经结束
            if (_sniffer.CurrentState.Status is SnifferStatus.Completed or SnifferStatus.Failed)
            {
                _logger.LogWarning("Sniffer was already in a terminal state before worker started. Exiting.");
                return;
            }

            try
            {
                // 3. 再启动任务
                await _sniffer.StartAsync();
                _logger.LogInformation("StudentIdSniffer background task has been started.");
                
                // 4. 等待任务完成信号
                await tcs.Task;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "A critical error occurred while starting or waiting for the sniffer.");
            }
            
             _logger.LogInformation("StudentIdSnifferWorker is finishing its execution.");
        }
        
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StopAsync called on the worker, signaling sniffer to stop.");
            await _sniffer.StopAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}