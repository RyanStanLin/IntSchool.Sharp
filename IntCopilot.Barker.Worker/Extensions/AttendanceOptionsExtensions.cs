using IntSchool.Sharp.Core.Data;

namespace IntCopilot.Barker.Worker.Extensions
{
    public static class AttendanceOptionsExtensions
    {
        public static bool HigherThanInclusive(this AttendanceOptions current, AttendanceOptions threshold)
        {
            return current.GetPriority() >= threshold.GetPriority();
        }
    }
}