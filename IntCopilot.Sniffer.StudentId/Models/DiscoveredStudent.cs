using System;
using IntCopilot.Shared;

namespace IntCopilot.Sniffer.StudentId.Models
{
    public record DiscoveredStudent
    {
        public Student Student { get; init; }
        public long SchoolYearId { get; }
        public DateTimeOffset DiscoveredAt { get; }

        public DiscoveredStudent(string studentId, string studentName, long schoolYearId)
        {
            long studentIdLong;
            if (string.IsNullOrWhiteSpace(studentId))
                throw new ArgumentException("Student ID cannot be empty", nameof(studentId));
            try
            {
                studentIdLong = Convert.ToInt64(studentId);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Failed to parse studentId as long: {StudentId}", nameof(studentId));
            }
            if (string.IsNullOrWhiteSpace(studentName))
                throw new ArgumentException("Student name cannot be empty", nameof(studentName));
            if (schoolYearId <= 0)
                throw new ArgumentException("Invalid school year ID", nameof(schoolYearId));
            Student = new Student(studentIdLong,studentName);
            SchoolYearId = schoolYearId;
            DiscoveredAt = DateTimeOffset.UtcNow;
        }
    }
}