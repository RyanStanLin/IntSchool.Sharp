using IntSchool.Sharp.Core.Models.Dto;

namespace IntCopilot.Barker.Worker.Data
{
    public class Filter
    {
        public Func<CourseSession, CourseSession, bool> Predicate { get; }

        public Filter(Func<CourseSession, CourseSession, bool> predicate)
        {
            Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }
    }
}