using IntCopilot.DataAccess.Postgres.Configuration;
using IntCopilot.DataAccess.Postgres.DataAccess;
using IntCopilot.DataAccess.Postgres.Exceptions;
using Microsoft.Extensions.Options;

namespace IntCopilot.Sniffer.StudentId.Worker;

using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IntCopilot.Sniffer.StudentId.Core;
using IntCopilot.Sniffer.StudentId.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class ResultPersistenceService : IHostedService
{
    private readonly ILogger<ResultPersistenceService> _logger;
    private readonly ILogger<PostgresStudentRepository> _repoLogger;
    private readonly IStudentIdSniffer _sniffer;
    private readonly PostgresDbSettings _dbSettings;
    private IDisposable? _subscription;

    public ResultPersistenceService(ILogger<ResultPersistenceService> logger,ILoggerFactory loggerFactory, IStudentIdSniffer sniffer, IOptions<PostgresDbSettings> dbSettings)
    {
        _logger = logger;
        _sniffer = sniffer;
        _dbSettings = dbSettings.Value;
        _repoLogger = loggerFactory.CreateLogger<PostgresStudentRepository>();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Result Persistence Service is waiting for sniffer completion.");
        
        _subscription = _sniffer.StateChanges
            // 只关心 Completed 状态
            .Where(state => state.Status == SnifferStatus.Completed)
            // 只取第一个 Completed 信号
            .Take(1)
            .Subscribe(
                async state => // 当接收到 Completed 状态时
                {
                    _logger.LogInformation("Sniffer completed. Persisting results...");
                    await PersistResultsAsync(state);
                }
            );
            
        return Task.CompletedTask;
    }

    private async Task PersistResultsAsync(SnifferState finalState)
    {
        try
        {
            _repoLogger.LogDebug("Initializing repository via async factory...");
            await using var studentRepo = await PostgresStudentRepository.CreateAsync(_dbSettings, _repoLogger);
            _repoLogger.LogInformation("Repository initialized successfully.");
            
            var students = finalState.DiscoveredStudents.Values.OrderBy(s => s.Student.StudentId).ToList();

            foreach (var student in students)
            {
                if (await studentRepo.GetStudentByIdAsync(student.Student.StudentId) == null)
                {
                    await studentRepo.AddStudentAsync(student.Student);
                    _repoLogger.LogDebug($"id:{student.Student.StudentId}, name:{student.Student.StudentName} added.");
                }
            }
            
            _repoLogger.LogInformation($"Finished adding students to result repository.");
        }
        catch (DataAccessInitializationException ex)
        {
            _logger.LogError(ex, "Could not initialize the data access layer. Please check your connection settings and PostgreSQL server status.");
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Configuration is invalid.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while storing results.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _subscription?.Dispose();
        return Task.CompletedTask;
    }
}