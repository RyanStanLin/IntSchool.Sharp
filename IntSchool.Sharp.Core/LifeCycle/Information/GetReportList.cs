using System.Collections.Generic;
using System.Threading.Tasks;
using IntSchool.Sharp.Core.Models;
using IntSchool.Sharp.Core.RequestConfigs;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<List<GetReportListResponseModel>, ErrorResponseModel> GetReportList(
        string schoolYearId, string studentId, string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        ArgumentException.ThrowIfNullOrEmpty(schoolYearId);
        ArgumentException.ThrowIfNullOrEmpty(studentId);

        var request = BuildGetReportListRequest(schoolYearId, studentId, schoolId);
        
        return TryExecute(
            request,
            GetReportListResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    public async Task<ApiResult<List<GetReportListResponseModel>, ErrorResponseModel>> GetReportListAsync(
        string schoolYearId, string studentId, string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        ArgumentException.ThrowIfNullOrEmpty(schoolYearId);
        ArgumentException.ThrowIfNullOrEmpty(studentId);

        var request = BuildGetReportListRequest(schoolYearId, studentId, schoolId);

        return await TryExecuteAsync(
            request,
            GetReportListResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    private RestRequest BuildGetReportListRequest(string schoolYearId, string studentId, string schoolId)
    {
        return new RestRequest(Constants.GetReportListPath, Method.Get)
            .AddHeader(Constants.JsonXPathKey, XToken)
            .AddHeader(Constants.JsonXSchoolId, schoolId)
            .AddQueryParameter(Constants.JsonSchoolYearIdKey, schoolYearId)
            .AddQueryParameter(Constants.JsonStudentIdKey, studentId);
    }
}