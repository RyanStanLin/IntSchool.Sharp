namespace IntSchool.Sharp;

internal static class Constants
{
    internal const string IntSchoolRootUrl = "https://pcd.intschool.cn";
    
    internal const string ApiSendSmsPath = "/api/login/vcodeMobileSend";
    internal const string ApiLoginPath = "/api/login/unify";
    internal const string GetAccountStudentsPath = "/api/student/list";
    internal const string GetAccountInfoPath = "/api/parent/userInfo";

    internal const string JsonXPathKey = "X-Token";
    internal const string JsonUtf8ContentType = "application/json;charset=UTF-8";
}