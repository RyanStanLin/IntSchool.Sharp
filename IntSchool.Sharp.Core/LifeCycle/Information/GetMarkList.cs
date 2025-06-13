using System.Threading.Tasks;
using IntSchool.Sharp.Core.Models;
using IntSchool.Sharp.Core.RequestConfigs;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<GetMarkListResponseModel, ErrorResponseModel> GetMarkList(
        string courseId, string schoolYearId, string studentId, GetPageControlConfiguration? configuration = null,
        string? nameFilter = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        ArgumentException.ThrowIfNullOrEmpty(courseId);
        ArgumentException.ThrowIfNullOrEmpty(schoolYearId);
        ArgumentException.ThrowIfNullOrEmpty(studentId);

        var request = BuildGetMarkListRequest(courseId, schoolYearId, studentId, configuration, nameFilter);

        return TryExecute(
            request,
            GetMarkListResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    public async Task<ApiResult<GetMarkListResponseModel, ErrorResponseModel>> GetMarkListAsync(
        string courseId, string schoolYearId, string studentId, GetPageControlConfiguration? configuration = null,
        string? nameFilter = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        ArgumentException.ThrowIfNullOrEmpty(courseId);
        ArgumentException.ThrowIfNullOrEmpty(schoolYearId);
        ArgumentException.ThrowIfNullOrEmpty(studentId);

        var request = BuildGetMarkListRequest(courseId, schoolYearId, studentId, configuration, nameFilter);

        return await TryExecuteAsync(
            request,
            GetMarkListResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    private RestRequest BuildGetMarkListRequest(
        string courseId, string schoolYearId, string studentId, GetPageControlConfiguration? configuration,
        string? nameFilter)
    {
        var request = new RestRequest(Constants.GetMarkListPath, Method.Get)
            .AddHeader(Constants.JsonXPathKey, XToken)
            .AddQueryParameter(Constants.JsonCourseIdKey, courseId)
            .AddQueryParameter(Constants.JsonSchoolYearIdKey, schoolYearId)
            .AddQueryParameter(Constants.JsonStudentIdKey, studentId);

        if (configuration is not null)
        {
            request.AddQueryParameter(Constants.JsonPageSizeKey, configuration.PageSize)
                   .AddQueryParameter(Constants.JsonPageCurrentKay, configuration.PageCurrent);
        }

        if (!string.IsNullOrEmpty(nameFilter))
        {
            request.AddQueryParameter(Constants.JsonNameKey, nameFilter);
        }

        return request;
    }
}