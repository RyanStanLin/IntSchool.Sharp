using System;

namespace IntCopilot.Sniffer.StudentId.Exceptions
{
    public class SnifferException : Exception
    {
        public SnifferException(string message) : base(message)
        {
        }

        public SnifferException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}