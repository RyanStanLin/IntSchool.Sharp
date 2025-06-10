using IntSchool.Sharp.Core.Models.Dto;

namespace IntCopilot.Barker.Worker.Data
{
    public class Subscription
    {
        public Func<CourseSession, CourseSession, bool>? Filter { get; init; }
        public required Action<CourseSession, CourseSession> Action { get; init; }

        public bool ShouldNotify(CourseSession prev, CourseSession current)
        {
            try
            {
                return Filter?.Invoke(prev, current) ?? true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Filter execution failed: {ex.Message}");
                return false; 
            }
        }
    }
}