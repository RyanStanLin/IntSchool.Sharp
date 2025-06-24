using System.Net.Sockets;
using IntCopilot.DataAccess.Postgres.Configuration;
using IntCopilot.DataAccess.Postgres.DataAccess;
using IntCopilot.DataAccess.Postgres.Exceptions;
using IntCopilot.Sniffer.StudentId.Extensions;
using IntSchool.Sharp.Core.LifeCycle;
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
                    await Task.Delay(1000);
                    retry:
                    try
                    {
                        var detailsRaw = await Api.Instance.GetStudentDetailAsync(student.Student.StudentId.ToString());
                        if (detailsRaw.IsSuccess)
                        {
                            var resultModel = detailsRaw.SuccessResult;
                            await studentRepo.AddStudentAsync(new StudentProfile(
                                StudentId:resultModel.StudentId,
                                StudentNum:resultModel.StudentNum.ToString(),
                                StudentName:student.Student.StudentName,
                                DefaultName:resultModel.Name,
                                EnglishName:resultModel.EnName,
                                FirstName:resultModel.FirstName,
                                LastName:resultModel.LastName,
                                Email:resultModel.Email,
                                Nationality:resultModel.CountryEnName,
                                EnterYear:resultModel.EnterYear.ToString(),
                                Address:resultModel.Address,
                                HouseName:resultModel.HouseName,
                                Stage:resultModel.HouseGroupName,
                                IdNumber:resultModel.IdNum,
                                ImageUrl:resultModel.AvatarUrl.ToString(),
                                IsMale: resultModel.Gender == "male" ? true : false,
                                Birthday: resultModel.Birthday.ToDateOnlyFromUnixMilliseconds(),
                                SectionName:resultModel.SectionName,
                                ClassName:resultModel.ClassName
                            ));
                            _repoLogger.LogDebug($"id:{student.Student.StudentId}, name:{student.Student.StudentName} added.");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error getting detail, retrying in 1s:");
                        await Task.Delay(1000);
                        goto retry;
                    }
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