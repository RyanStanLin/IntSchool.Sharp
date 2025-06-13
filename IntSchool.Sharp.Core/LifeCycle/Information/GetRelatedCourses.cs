using System.Collections.Generic;
using System.Threading.Tasks;
using IntSchool.Sharp.Core.Models;
using IntSchool.Sharp.Core.RequestConfigs;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<List<GetRelatedCoursesResponseModel>, ErrorResponseModel> GetRelatedCourses(GetRelatedCoursesConfiguration config, string studentId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        ArgumentException.ThrowIfNullOrEmpty(studentId);

        var request = BuildGetRelatedCoursesRequest(config, studentId);

        return TryExecute(
            request,
            GetRelatedCoursesResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    public async Task<ApiResult<List<GetRelatedCoursesResponseModel>, ErrorResponseModel>> GetRelatedCoursesAsync(GetRelatedCoursesConfiguration config, string studentId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        ArgumentException.ThrowIfNullOrEmpty(studentId);

        var request = BuildGetRelatedCoursesRequest(config, studentId);

        return await TryExecuteAsync(
            request,
            GetRelatedCoursesResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    private RestRequest BuildGetRelatedCoursesRequest(GetRelatedCoursesConfiguration config, string studentId)
    {
        return new RestRequest(Constants.GetRelatedCoursesPath, Method.Get)
            .AddHeader(Constants.JsonXPathKey, XToken)
            .AddQueryParameter(Constants.JsonDeleteFlagKey, config.DeleteFlag)
            .AddQueryParameter(Constants.JsonCourseFlagKey, config.CourseFlag)
            .AddQueryParameter(Constants.JsonSchoolYearIdKey, config.SchoolYearId)
            .AddQueryParameter(Constants.JsonStudentIdKey, studentId);
    }
}