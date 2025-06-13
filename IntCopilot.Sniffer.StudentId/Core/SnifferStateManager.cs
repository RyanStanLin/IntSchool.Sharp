using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using IntCopilot.Sniffer.StudentId.Models;
using Microsoft.Extensions.Logging;

namespace IntCopilot.Sniffer.StudentId.Core
{
    internal class SnifferStateManager : ISnifferStateManager
    {
        private readonly ILogger<SnifferStateManager> _logger;
        private readonly SnifferState _state;
        private readonly ConcurrentQueue<string> _pendingStudents;
        private readonly ManualResetEventSlim _pauseEvent;
        private DiscoveredStudent? _initialStudent;

        public SnifferState CurrentState => _state;
        public DiscoveredStudent InitialStudent => _initialStudent ?? 
            throw new InvalidOperationException("State manager not initialized");

        public bool IsQueueEmpty => _pendingStudents.IsEmpty;

        public SnifferStateManager(ILogger<SnifferStateManager> logger)
        {
            _logger = logger;
            _state = new SnifferState();
            _pendingStudents = new ConcurrentQueue<string>();
            _pauseEvent = new ManualResetEventSlim(true); // 初始为非暂停状态
        }

        public void Initialize(DiscoveredStudent initialStudent)
        {
            _initialStudent = initialStudent;
            _state.AddStudent(initialStudent);
            _pendingStudents.Enqueue(initialStudent.StudentId);
            _state.UpdateStatus(SnifferStatus.Running);
            _logger.LogInformation("State manager initialized with student {StudentId}", initialStudent.StudentId);
        }

        public async Task WaitIfPausedAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() => _pauseEvent.Wait(cancellationToken), cancellationToken);
        }

        public Task<bool> TryDequeueNextStudentAsync(out string? studentId)
        {
            var success = _pendingStudents.TryDequeue(out studentId);
            if (success)
            {
                _state.DecrementPendingCount();
            }
            return Task.FromResult(success);
        }

        public void EnqueueStudent(string studentId)
        {
            _pendingStudents.Enqueue(studentId);
            _state.IncrementPendingCount();
        }

        public void UpdateState(SnifferStatus status, Exception? error = null)
        {
            _state.UpdateStatus(status, error);
            
            switch (status)
            {
                case SnifferStatus.Paused:
                    _pauseEvent.Reset();
                    break;
                case SnifferStatus.Running:
                    _pauseEvent.Set();
                    break;
            }

            _logger.LogInformation("State updated to {Status}", status);
        }
    }
}