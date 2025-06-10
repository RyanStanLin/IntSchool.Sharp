using IntCopilot.Barker.Worker;

namespace IntCopilot.Barker.Client;

    /*
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly AttendanceWorker _attendanceWorker;
        private bool _isStarted;

        public Worker(ILogger<Worker> logger, AttendanceWorker attendanceWorker)
        {
            _logger = logger;
            _attendanceWorker = attendanceWorker;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Starting Attendance monitoring service...");
                _attendanceWorker.Start();
                _isStarted = true;
                _logger.LogInformation("Attendance monitoring service started successfully");

                using var registration = stoppingToken.Register(() => 
                {
                    _logger.LogInformation("Stopping Attendance Worker due to cancellation...");
                    _attendanceWorker.Stop();
                });

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Fatal error in attendance monitoring service");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_isStarted)
            {
                _logger.LogInformation("Stopping attendance monitoring service...");
                _attendanceWorker.Stop();
                _logger.LogInformation("Attendance monitoring service stopped");
            }
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _attendanceWorker.Dispose();
            base.Dispose();
        }
    }
    */

    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly AttendanceWorkerManager _workerManager;

        public Worker(ILogger<Worker> logger, AttendanceWorkerManager workerManager)
        {
            _logger = logger;
            _workerManager = workerManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Main Hosted Service is running.");

            try
            {
                // 保持后台服务运行
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Service is stopping due to cancellation request.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred in the main worker.");
                throw;
            }
        }
    }
