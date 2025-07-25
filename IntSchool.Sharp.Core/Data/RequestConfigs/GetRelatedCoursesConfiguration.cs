namespace IntSchool.Sharp.Core.RequestConfigs;

public struct GetRelatedCoursesConfiguration(string schoolYearId, bool deleteFlag = true, bool courseFlag = true)
{
    public string SchoolYearId { get; set; } = schoolYearId;
    public bool DeleteFlag { get; set; } = deleteFlag;
    public bool CourseFlag { get; set; } = courseFlag;
}