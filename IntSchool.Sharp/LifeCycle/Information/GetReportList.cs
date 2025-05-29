using IntSchool.Sharp.Models;
using IntSchool.Sharp.RequestConfigs;
using RestSharp;

namespace IntSchool.Sharp.LifeCycle;
public partial class API
{
    public (bool isSuccess, ApiResult<List<GetReportListResponseModel>, ErrorResponseModel>? apiResult) GetReportList(
        string schoolYearId, string studentId, string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        ArgumentException.ThrowIfNullOrEmpty(schoolYearId);
        ArgumentException.ThrowIfNullOrEmpty(studentId);
        //ArgumentException.ThrowIfNullOrEmpty(studentId); Already done in side the declaration of SharedStudentTimespanConfiguration
        RestRequest request = new RestRequest(resource: Constants.GetReportListPath, method: Method.Get)
            .AddHeader(Constants.JsonXPathKey, XToken)
            .AddHeader(Constants.JsonXSchoolId, schoolId)
            .AddQueryParameter(Constants.JsonSchoolYearIdKey, schoolYearId)
            .AddQueryParameter(Constants.JsonStudentIdKey, studentId);
        return TryExecute(
            request,
            GetReportListResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

}