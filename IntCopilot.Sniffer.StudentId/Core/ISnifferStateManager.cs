using System.Threading;
using System.Threading.Tasks;
using IntCopilot.Sniffer.StudentId.Models;

namespace IntCopilot.Sniffer.StudentId.Core
{
    public interface ISnifferStateManager
    {
        SnifferState CurrentState { get; }
        DiscoveredStudent InitialStudent { get; }
        
        void Initialize(DiscoveredStudent initialStudent);
        Task WaitIfPausedAsync(CancellationToken cancellationToken);
        Task<bool> TryDequeueNextStudentAsync(out long? studentId);
        void EnqueueStudent(long studentId);
        bool IsQueueEmpty { get; }
        void UpdateState(SnifferStatus status, Exception? error = null);
    }
}