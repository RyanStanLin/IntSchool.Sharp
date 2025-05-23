namespace IntSchool.Sharp.RequestConfigs;

public struct GetRelatedCoursesConfiguration(string schoolYearId, bool deleteFlag, bool courseFlag)
{
    public string SchoolYearId { get; set; } = schoolYearId;
    public bool DeleteFlag { get; set; } = deleteFlag;
    public bool CourseFlag { get; set; } = courseFlag;
}