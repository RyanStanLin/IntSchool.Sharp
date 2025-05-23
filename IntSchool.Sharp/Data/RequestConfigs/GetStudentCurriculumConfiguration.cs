namespace IntSchool.Sharp.RequestConfigs;

public struct GetStudentCurriculumConfiguration
{
    public GetStudentCurriculumConfiguration(string schoolYearId, string studentId, DateTime startTime, DateTime endTime)
    {
        ArgumentException.ThrowIfNullOrEmpty(studentId);
        ArgumentException.ThrowIfNullOrEmpty(schoolYearId);
        StudentId = studentId;
        StartTime = startTime;
        EndTime = endTime;
        SchoolYearId = schoolYearId;
    }

    public string StudentId { get; set; }
    
    public string SchoolYearId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}