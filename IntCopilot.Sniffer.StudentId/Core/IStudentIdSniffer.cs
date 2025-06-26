using System;
using System.Threading.Tasks;
using IntCopilot.Sniffer.StudentId.Models;

namespace IntCopilot.Sniffer.StudentId.Core
{
    public interface IStudentIdSniffer : IAsyncDisposable
    {
        Task StartAsync();
        
        Task PauseAsync();
        Task ResumeAsync();
        Task StopAsync();
        
        // 状态追踪
        IObservable<SnifferState> StateChanges { get; }
        SnifferState CurrentState { get; }
    }
}