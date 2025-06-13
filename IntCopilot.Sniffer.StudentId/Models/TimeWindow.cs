using System;

namespace IntCopilot.Sniffer.StudentId.Models
{
    public enum TimeWindow
    {
        Today,
        ThisWeek,
        NextWeek,
        Custom
    }

    public class TimeRange
    {
        public DateTime Start { get; }
        public DateTime End { get; }

        public TimeRange(DateTime start, DateTime end)
        {
            if (end <= start)
                throw new ArgumentException("End time must be greater than start time");
            
            Start = start;
            End = end;
        }
    }

    public static class TimeWindowHelper
    {
        public static TimeRange GetTimeRange(TimeWindow window, Func<DateTime> timeProvider)
        {
            var now = timeProvider();
            var today = now.Date;

            return window switch
            {
                TimeWindow.Today => new TimeRange(today, today.AddDays(1)),
                TimeWindow.ThisWeek => new TimeRange(today, today.AddDays(7)),
                TimeWindow.NextWeek => new TimeRange(today.AddDays(7), today.AddDays(14)),
                _ => throw new ArgumentException($"Unsupported time window: {window}")
            };
        }
    }
}