namespace IntCopilot.Barker.Worker.Data
{
    public class TimeWindow
    {
        public required DateTime StartTime { get; set; }
        public required DateTime EndTime { get; set; }

        public static TimeWindow Today => new()
        {
            StartTime = DateTime.Today,
            EndTime = DateTime.Today.AddDays(1).AddSeconds(-1)
        };

        public static TimeWindow ThisWeek
        {
            get
            {
                var today = DateTime.Today;
                var diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                var monday = today.AddDays(-1 * diff).Date;
                return new TimeWindow
                {
                    StartTime = monday,
                    EndTime = monday.AddDays(7).AddSeconds(-1)
                };
            }
        }

        public bool Validate()
        {
            return StartTime < EndTime;
        }
    }
}