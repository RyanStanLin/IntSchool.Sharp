using System.Net;
using IntSchool.Sharp.Core.Models;
using IntSchool.Sharp.Core.RequestConfigs;
using IntSchool.Sharp.Core.Data.EventArguments;
using IntSchool.Sharp.Core.Extensions;
using Newtonsoft.Json;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public  ApiResult<GetStudentCurriculumResponseModel, ErrorResponseModel> GetStudentCurriculum(SharedStudentTimespanConfiguration configuration)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        
        var request = BuildGetStudentCurriculumRequest(configuration);

        return TryExecute(
            request,
            GetStudentCurriculumResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    public async Task<ApiResult<GetStudentCurriculumResponseModel, ErrorResponseModel>> GetStudentCurriculumAsync(SharedStudentTimespanConfiguration configuration)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);

        var request = BuildGetStudentCurriculumRequest(configuration);

        return await TryExecuteAsync(
            request,
            GetStudentCurriculumResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }
    
    private RestRequest BuildGetStudentCurriculumRequest(SharedStudentTimespanConfiguration config)
    {
        return new RestRequest(Constants.GetStudentCurriculumPath + config.SchoolYearId, Method.Get)
            .AddHeader(Constants.JsonXPathKey, XToken)
            .AddQueryParameter(Constants.JsonStudentIdKey, config.StudentId)
            .AddQueryParameter(Constants.JsonStartTimeKey, config.StartTime.ToUnixTimestampMilliseconds())
            .AddQueryParameter(Constants.JsonEndTimeKey, config.EndTime.ToUnixTimestampMilliseconds());
    }
}