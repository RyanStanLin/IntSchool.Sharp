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
    public class SnifferEventLogger(ILogger<SnifferEventLogger> logger, IStudentIdSniffer sniffer)
        : IHostedService
    {
        private IDisposable? _subscription;
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Sniffer Event Logger is starting and subscribing to state changes.");

            _subscription = sniffer.StateChanges
                .Sample(TimeSpan.FromSeconds(1))
                .Subscribe(
                    state => // OnNext: 当有新状态时执行
                    {
                        logger.LogInformation(
                            "SNIFFER STATUS | Status: {Status,-10} | Discovered: {DiscoveredCount,3} | Pending: {PendingCount,4}",
                            state.Status,
                            state.DiscoveredStudents.Count,
                            state.PendingQueueCount);

                        if (state.Status == SnifferStatus.Failed && state.LastError != null)
                        {
                            logger.LogError(state.LastError, "Sniffer entered a FAILED state.");
                        }
                    },
                    error => 
                    {
                        logger.LogCritical(error, "The sniffer state observable sequence has faulted unexpectedly.");
                    },
                    () =>
                    {
                        logger.LogInformation("Sniffer state observable sequence has completed.");
                    }
                );

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Sniffer Event Logger is stopping.");

            _subscription?.Dispose();
            
            return Task.CompletedTask;
        }
    }
}