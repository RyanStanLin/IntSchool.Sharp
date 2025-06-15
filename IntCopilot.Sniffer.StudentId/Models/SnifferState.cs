using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using IntCopilot.Sniffer.StudentId.Exceptions;

namespace IntCopilot.Sniffer.StudentId.Models
{
    public class SnifferState
    {
        public SnifferStatus Status { get; init; } = SnifferStatus.NotStarted;
        public IImmutableDictionary<long, DiscoveredStudent> DiscoveredStudents { get; init; } = ImmutableDictionary<long, DiscoveredStudent>.Empty;
        public int PendingQueueCount { get; init; } = 0;
        public SnifferException? LastError { get; init; }
        public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
        private readonly ConcurrentDictionary<long, DiscoveredStudent> _discoveredStudents;
        private readonly object _lock = new();
        private volatile SnifferStatus _status;
        private volatile Exception? _lastError;
        private int _processedCount;
        private int _pendingCount;

        //public SnifferStatus Status => _status;
        //public Exception? LastError => _lastError;
        public int ProcessedCount => _processedCount;
        public int PendingCount => _pendingCount;
        public DateTimeOffset LastUpdateTime { get; private set; }
        
        //public IReadOnlyDictionary<long, DiscoveredStudent> DiscoveredStudents => _discoveredStudents.ToImmutableDictionary();

        public SnifferState()
        {
            _status = SnifferStatus.NotStarted;
            _discoveredStudents = new ConcurrentDictionary<long, DiscoveredStudent>();
            LastUpdateTime = DateTimeOffset.UtcNow;
        }

        public void UpdateStatus(SnifferStatus newStatus, Exception? error = null)
        {
            lock (_lock)
            {
                _status = newStatus;
                _lastError = error;
                LastUpdateTime = DateTimeOffset.UtcNow;
            }
        }

        public bool AddStudent(DiscoveredStudent student)
        {
            var added = _discoveredStudents.TryAdd(student.Student.StudentId, student);
            if (added)
            {
                Interlocked.Increment(ref _processedCount);
                LastUpdateTime = DateTimeOffset.UtcNow;
            }
            return added;
        }

        public bool TryGetStudent(long studentId, out DiscoveredStudent? student)
        {
            return _discoveredStudents.TryGetValue(studentId, out student);
        }

        public void UpdatePendingCount(int count)
        {
            Interlocked.Exchange(ref _pendingCount, count);
            LastUpdateTime = DateTimeOffset.UtcNow;
        }

        public void IncrementPendingCount()
        {
            Interlocked.Increment(ref _pendingCount);
            LastUpdateTime = DateTimeOffset.UtcNow;
        }

        public void DecrementPendingCount()
        {
            Interlocked.Decrement(ref _pendingCount);
            LastUpdateTime = DateTimeOffset.UtcNow;
        }
    }
}