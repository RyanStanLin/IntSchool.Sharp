namespace IntSchool.Sharp;

internal static class Constants
{
    internal const string IntSchoolRootUrl = "https://pcd.intschool.cn";
    
    internal const string ApiSendSmsPath = "/api/login/vcodeMobileSend";
    internal const string ApiLoginPath = "/api/login/unify";
    internal const string GetAccountStudentsPath = "/api/student/list";
    internal const string GetAccountInfoPath = "/api/parent/userInfo";
    internal const string GetStudentDetailPath = "/api/student/detail";
    internal const string GetParentsPath = "/api/student/getParents";
    internal const string GetStudentCurriculumPath = "/api/curriculum/student/";
    internal const string GetCurrentSchoolYearPath = "/api/semester/currentSchoolYear";
    internal const string GetRelatedCoursesPath = "/api/dropDown/relatedAllCourses";
    internal const string GetMarkListPath = "/api/task/mergeList";
    internal const string GetMarkDetailPath = "/api/task/detail";

    internal const string JsonXPathKey = "X-Token";
    internal const string JsonStudentIdKey = "studentId";
    internal const string JsonUtf8ContentType = "application/json;charset=UTF-8";
    internal const string JsonStartTimeKey = "start";
    internal const string JsonEndTimeKey = "end";
    internal const string JsonCourseFlagKey = "courseFlag";
    internal const string JsonDeleteFlagKey = "deleteFlag";
    internal const string JsonSchoolYearIdKey = "schoolYearId";
    internal const string JsonCourseIdKey = "courseId";
    internal const string JsonPageSizeKey = "pageSize";
    internal const string JsonPageCurrentKay = "pageCurrent";
    internal const string JsonNameKey = "name";
    internal const string JsonTaskStudentIdKey = "taskStudentId";
}