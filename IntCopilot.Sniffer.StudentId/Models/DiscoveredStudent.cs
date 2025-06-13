using System;

namespace IntCopilot.Sniffer.StudentId.Models
{
    public record DiscoveredStudent
    {
        public string StudentId { get; }
        public string StudentName { get; }
        public long SchoolYearId { get; }
        public DateTimeOffset DiscoveredAt { get; }

        public DiscoveredStudent(string studentId, string studentName, long schoolYearId)
        {
            if (string.IsNullOrWhiteSpace(studentId))
                throw new ArgumentException("Student ID cannot be empty", nameof(studentId));
            if (string.IsNullOrWhiteSpace(studentName))
                throw new ArgumentException("Student name cannot be empty", nameof(studentName));
            if (schoolYearId <= 0)
                throw new ArgumentException("Invalid school year ID", nameof(schoolYearId));

            StudentId = studentId;
            StudentName = studentName;
            SchoolYearId = schoolYearId;
            DiscoveredAt = DateTimeOffset.UtcNow;
        }
    }
}