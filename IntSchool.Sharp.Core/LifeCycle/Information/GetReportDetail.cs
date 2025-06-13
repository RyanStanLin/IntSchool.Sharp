using System.Threading.Tasks;
using IntSchool.Sharp.Core.Models;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<GetReportDetailResponseModel, ErrorResponseModel> GetReportDetail(
        string gradePeriodId, string studentId, string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        ArgumentException.ThrowIfNullOrEmpty(gradePeriodId);
        ArgumentException.ThrowIfNullOrEmpty(studentId);
        
        var request = BuildGetReportDetailRequest(gradePeriodId, studentId, schoolId);
        
        return TryExecute(
            request,
            GetReportDetailResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    public async Task<ApiResult<GetReportDetailResponseModel, ErrorResponseModel>> GetReportDetailAsync(
        string gradePeriodId, string studentId, string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        ArgumentException.ThrowIfNullOrEmpty(gradePeriodId);
        ArgumentException.ThrowIfNullOrEmpty(studentId);

        var request = BuildGetReportDetailRequest(gradePeriodId, studentId, schoolId);

        return await TryExecuteAsync(
            request,
            GetReportDetailResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    private RestRequest BuildGetReportDetailRequest(string gradePeriodId, string studentId, string schoolId)
    {
        return new RestRequest(Constants.GetReportDetailPath, Method.Get)
            .AddHeader(Constants.JsonXPathKey, XToken)
            .AddHeader(Constants.JsonXSchoolId, schoolId)
            .AddQueryParameter(Constants.JsonGradePeriodIdKey, gradePeriodId)
            .AddQueryParameter(Constants.JsonStudentIdKey, studentId);
    }
}