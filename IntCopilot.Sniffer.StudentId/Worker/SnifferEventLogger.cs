using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using IntCopilot.Sniffer.StudentId.Core;
using IntCopilot.Sniffer.StudentId.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IntCopilot.Sniffer.StudentId.Worker
{
    // 这个服务专门负责监听和记录Sniffer的事件/状态
    public class SnifferEventLogger : IHostedService
    {
        private readonly ILogger<SnifferEventLogger> _logger;
        private readonly IStudentIdSniffer _sniffer;
        private IDisposable? _subscription;

        // 通过构造函数注入它需要的服务
        public SnifferEventLogger(ILogger<SnifferEventLogger> logger, IStudentIdSniffer sniffer)
        {
            _logger = logger;
            _sniffer = sniffer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Sniffer Event Logger is starting and subscribing to state changes.");

            // 订阅 StateChanges IObservable
            _subscription = _sniffer.StateChanges
                // 使用 .Sample() 或 .Throttle() 来避免日志刷屏
                // .Sample(TimeSpan.FromSeconds(1)) 会每秒取一次最新的状态来记录
                // 这在高频更新的场景下非常有用
                .Sample(TimeSpan.FromSeconds(1))
                .Subscribe(
                    state => // OnNext: 当有新状态时执行
                    {
                        // 记录一个清晰、结构化的日志
                        _logger.LogInformation(
                            "SNIFFER STATUS | Status: {Status,-10} | Discovered: {DiscoveredCount,3} | Pending: {PendingCount,4}",
                            state.Status,
                            state.DiscoveredStudents.Count,
                            state.PendingQueueCount);

                        // 可以在特定状态时记录更详细的信息
                        if (state.Status == SnifferStatus.Failed && state.LastError != null)
                        {
                            _logger.LogError(state.LastError, "Sniffer entered a FAILED state.");
                        }
                    },
                    error => // OnError: 当Observable序列出现错误时执行
                    {
                        _logger.LogCritical(error, "The sniffer state observable sequence has faulted unexpectedly.");
                    },
                    () => // OnCompleted: 当序列完成时执行
                    {
                        _logger.LogInformation("Sniffer state observable sequence has completed.");
                    }
                );

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Sniffer Event Logger is stopping.");

            // 在服务停止时，取消订阅，防止内存泄漏
            _subscription?.Dispose();
            
            return Task.CompletedTask;
        }
    }
}