using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.RateLimiting;
using System.Threading.Tasks;
using IntCopilot.Sniffer.StudentId.Configuration;
using IntCopilot.Sniffer.StudentId.Exceptions;
using IntCopilot.Sniffer.StudentId.Infrastructure;
using IntCopilot.Sniffer.StudentId.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// 假设API模型定义
using IntSchool.Sharp.Core.Models;
using IntSchool.Sharp.Core.RequestConfigs;

namespace IntCopilot.Sniffer.StudentId.Core
{
    // 内部使用的任务记录
    internal record SnifferWorkItem(DiscoveredStudent Student, int RetryCount = 0);

    internal sealed class StudentIdSniffer : IStudentIdSniffer
    {
        private readonly ILogger<StudentIdSniffer> _logger;
        private readonly IApiClient _apiClient;
        private readonly RateLimiter _rateLimiter;
        private readonly SnifferConfiguration _config;
        
        // 并发和状态
        private readonly AsyncLock _lock = new();
        private readonly BehaviorSubject<SnifferState> _stateSubject;
        private CancellationTokenSource? _cts;
        private Task _processingTask = Task.CompletedTask;
        
        // 内部数据
        private readonly ConcurrentDictionary<long, DiscoveredStudent> _discoveredStudents = new();
        private readonly ConcurrentQueue<SnifferWorkItem> _workQueue = new();

        public IObservable<SnifferState> StateChanges => _stateSubject.AsObservable();
        public SnifferState CurrentState => _stateSubject.Value;

        public StudentIdSniffer(
            ILogger<StudentIdSniffer> logger,
            IApiClient apiClient,
            RateLimiter rateLimiter,
            IOptions<SnifferConfiguration> options)
        {
            _logger = logger;
            _apiClient = apiClient;
            _rateLimiter = rateLimiter;
            _config = options.Value;
            _stateSubject = new BehaviorSubject<SnifferState>(new SnifferState());
        }

        public async Task StartAsync()
        {
            using (await _lock.LockAsync())
            {
                if (CurrentState.Status != SnifferStatus.NotStarted)
                    throw new SnifferException("Sniffer can only be started once.");

                _cts = new CancellationTokenSource();
                UpdateState(SnifferStatus.Running);

                _processingTask = ProcessQueueAsync();
            }
        }
        
        private async Task ProcessQueueAsync()
        {
            try
            {
                await InitializeSnifferAsync();

                while (!_cts!.Token.IsCancellationRequested)
                {
                    if (CurrentState.Status == SnifferStatus.Paused)
                    {
                        await Task.Delay(200, _cts.Token);
                        continue;
                    }
                    
                    if (!_workQueue.TryDequeue(out var workItem))
                    {
                        if (_workQueue.IsEmpty)
                        {
                            _logger.LogInformation("Processing queue is empty. Sniffer completed successfully.");
                            UpdateState(SnifferStatus.Completed);
                            return;
                        }
                        continue;
                    }
                    
                    await ProcessStudentWithRateLimitAsync(workItem);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Sniffer task was canceled.");
                UpdateState(SnifferStatus.Completed); // Canceled is a form of completion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "A fatal error occurred in the processing loop.");
                UpdateState(SnifferStatus.Failed, new SnifferException("Processing loop failed.", ex));
            }
        }
        
        private async Task InitializeSnifferAsync()
        {
            _logger.LogInformation("Initializing sniffer...");
            var schoolYearId = await GetSchoolYearIdAsync();
            var initialStudent = new DiscoveredStudent(_config.InitialStudentId, _config.InitialStudentName, schoolYearId);
            
            _discoveredStudents.TryAdd(initialStudent.Student.StudentId, initialStudent);
            _workQueue.Enqueue(new SnifferWorkItem(initialStudent));
            UpdateState(CurrentState.Status); // Trigger state update with initial student
            _logger.LogInformation("Sniffer initialized for student {StudentName} ({StudentId}) in school year {SchoolYearId}.",
                initialStudent.Student.StudentName, initialStudent.Student.StudentId, schoolYearId);
        }

        private async Task ProcessStudentWithRateLimitAsync(SnifferWorkItem workItem)
        {
            RateLimitLease? lease = null;
            try
            {
                lease = await _rateLimiter.AcquireAsync(1, _cts!.Token);
                if (!lease.IsAcquired)
                {
                    _logger.LogWarning("Failed to acquire rate limit lease. Re-queueing student {StudentId}.", workItem.Student.Student.StudentId);
                    _workQueue.Enqueue(workItem);
                    return;
                }
                
                await ProcessSingleStudentAsync(workItem);
            }
            finally
            {
                lease?.Dispose();
            }
        }

        private async Task ProcessSingleStudentAsync(SnifferWorkItem workItem)
        {
            var student = workItem.Student;
            _logger.LogDebug("Processing student {StudentId}", student.Student.StudentId);
            
            try
            {
                var timeRange = TimeWindowHelper.GetTimeRange(_config.TimeWindow, () => DateTime.UtcNow);
                var curriculumResult = await _apiClient.GetStudentCurriculumAsync(new SharedStudentTimespanConfiguration
                {
                    SchoolYearId = student.SchoolYearId.ToString(),
                    StudentId = student.Student.StudentId.ToString(),
                    StartTime = timeRange.Start,
                    EndTime = timeRange.End
                }, _cts!.Token);

                if (!curriculumResult.IsSuccess || curriculumResult.SuccessResult == null)
                {
                    throw new SnifferException($"API call failed for student {student.Student.StudentId}: {curriculumResult.ErrorResult?.Message ?? "Unknown error"}");
                }

                ProcessCurriculum(curriculumResult.SuccessResult, student.SchoolYearId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to process student {StudentId}. Retry {RetryCount}/{MaxRetries}",
                    student.Student.StudentId, workItem.RetryCount + 1, _config.MaxRetries);
                
                if (workItem.RetryCount < _config.MaxRetries)
                {
                    _workQueue.Enqueue(workItem with { RetryCount = workItem.RetryCount + 1 });
                }
                else
                {
                    _logger.LogError("Max retries reached for student {StudentId}. Giving up.", student.Student.StudentId);
                }
            }
        }
        
        private void ProcessCurriculum(GetStudentCurriculumResponseModel curriculum, long schoolYearId)
        {
            if (curriculum.ClassArranges == null) return;
            
            var newStudentsFound = 0;
            foreach (var classmate in curriculum.ClassArranges.Values.SelectMany(d => d.Values).SelectMany(c => c.CourseId?.Students ?? new List<GetStudentCurriculumResponseModelStudent>()))
            {
                var newStudent = new DiscoveredStudent(classmate.StudentId.ToString(), classmate.Name, schoolYearId);
                if (_discoveredStudents.TryAdd(newStudent.Student.StudentId, newStudent))
                {
                    _workQueue.Enqueue(new SnifferWorkItem(newStudent));
                    newStudentsFound++;
                }
            }
            
            if(newStudentsFound > 0)
            {
                _logger.LogInformation("Discovered {Count} new students.", newStudentsFound);
                UpdateState(CurrentState.Status);
            }
        }

        private async Task<long> GetSchoolYearIdAsync()
        {
            if (_config.SchoolYearPreset != SchoolYearPreset.Current)
                throw new NotSupportedException("Only SchoolYearPreset.Current is supported in this version.");
            
            var result = await _apiClient.GetCurrentSchoolYearAsync(_cts!.Token);
            if (!result.IsSuccess || result.SuccessResult == null)
                throw new SnifferException("Failed to get current school year from API.");
                
            return result.SuccessResult.SchoolYearId;
        }

        private void UpdateState(SnifferStatus newStatus, SnifferException? error = null)
        {
            var newState = new SnifferState
            {
                Status = newStatus,
                DiscoveredStudents = _discoveredStudents.ToImmutableDictionary(),
                PendingQueueCount = _workQueue.Count,
                LastError = error,
                Timestamp = DateTimeOffset.UtcNow
            };
            _stateSubject.OnNext(newState);
        }

        public async Task PauseAsync()
        {
            using (await _lock.LockAsync())
            {
                if (CurrentState.Status == SnifferStatus.Running)
                {
                    _logger.LogInformation("Pausing sniffer.");
                    UpdateState(SnifferStatus.Paused);
                }
            }
        }

        public async Task ResumeAsync()
        {
            using (await _lock.LockAsync())
            {
                if (CurrentState.Status == SnifferStatus.Paused)
                {
                    _logger.LogInformation("Resuming sniffer.");
                    UpdateState(SnifferStatus.Running);
                }
            }
        }
        
        public async Task StopAsync()
        {
            bool wasRunning;
            using (await _lock.LockAsync())
            {
                wasRunning = CurrentState.Status is SnifferStatus.Running or SnifferStatus.Paused;
                if (wasRunning)
                {
                    _logger.LogInformation("Stopping sniffer...");
                    UpdateState(SnifferStatus.Completed); // Treat stop as a clean completion
                    _cts?.Cancel();
                }
            }

            if(wasRunning)
            {
                // Wait for the task to finish outside the lock
                await _processingTask;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await StopAsync();
            _stateSubject.OnCompleted();
            _stateSubject.Dispose();
            _cts?.Dispose();
        }
    }
}