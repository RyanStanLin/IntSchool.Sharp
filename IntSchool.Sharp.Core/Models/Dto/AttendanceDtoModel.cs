using IntSchool.Sharp.Core.Data;

namespace IntSchool.Sharp.Core.Models.Dto;

public class AttendanceDtoModel
{
    public List<AttendanceDay> Days { get; set; } = new List<AttendanceDay>();
}

public class AttendanceDay
{
    public DateTime Date { get; set; }
    public AttendanceOptions MorningAttendance { get; set; }
    public List<CourseSession> Courses { get; set; } = new List<CourseSession>();
}

public class CourseSession
{
    public AttendanceOptions Attendance { get; set; }
    public string CourseName { get; set; } = String.Empty;
    public string ClassRoom { get; set; } = String.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}