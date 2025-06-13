using System;

namespace IntCopilot.Sniffer.StudentId.Models.Events
{
    public class SnifferEventArgs : EventArgs
    {
        public DiscoveredStudent Student { get; }
        public Exception? Error { get; }
        public string? Message { get; }
        public DateTimeOffset Timestamp { get; }
        public SnifferState State { get; }

        public SnifferEventArgs(
            DiscoveredStudent student,
            SnifferState state,
            Exception? error = null,
            string? message = null)
        {
            Student = student ?? throw new ArgumentNullException(nameof(student));
            State = state ?? throw new ArgumentNullException(nameof(state));
            Error = error;
            Message = message;
            Timestamp = DateTimeOffset.UtcNow;
        }
    }
}