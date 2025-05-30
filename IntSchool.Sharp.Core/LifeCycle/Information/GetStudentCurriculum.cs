using System.Net;
using IntSchool.Sharp.Core.Models;
using IntSchool.Sharp.Core.RequestConfigs;
using IntSchool.Sharp.Core.Data.EventArguments;
using IntSchool.Sharp.Core.Extensions;
using Newtonsoft.Json;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class API
{
    public (bool isSuccess, ApiResult<GetStudentCurriculumResponseModel, ErrorResponseModel>? apiResult) GetStudentCurriculum(SharedStudentTimespanConfiguration configuration)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        //ArgumentException.ThrowIfNullOrEmpty(studentId); Already done in side the declaration of SharedStudentTimespanConfiguration
        RestRequest request = new RestRequest(resource: Constants.GetStudentCurriculumPath + configuration.SchoolYearId, method: Method.Get)
            .AddHeader(Constants.JsonXPathKey, XToken)
            .AddQueryParameter(Constants.JsonStudentIdKey, configuration.StudentId)
            .AddQueryParameter(Constants.JsonStartTimeKey, configuration.StartTime.ToUnixTimestampMilliseconds())
            .AddQueryParameter(Constants.JsonEndTimeKey, configuration.EndTime.ToUnixTimestampMilliseconds());
        return TryExecute(
            request,
            GetStudentCurriculumResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
        /*try
        {
            var response = _client.Execute(request);

            if (response.StatusCode is not HttpStatusCode.OK)
            {
                try
                {
                    var error = ErrorResponseModel.FromJson(response.Content);
                    var result = ApiResult<GetStudentCurriculumResponseModel, ErrorResponseModel>.Error(error);
                    var eventArgs =
                        new RemoteErrorEventArgs(
                            timestamp:error.Timestamp.DateTime,
                            raw: error,
                            xToken:XToken
                        );
                    OnRemoteError?.Invoke(this, eventArgs);
                    
                    return (false, result);
                }
                catch (JsonException ex)
                {
                    var eventArgs = new ContentMappingErrorEventArgs(
                        timestamp: DateTime.Now,
                        json: response.Content,
                        jsonException: ex);
                    OnContentMappingError?.Invoke(this, eventArgs);
                    return (false, null);
                }
            }
            
            var raw = GetStudentCurriculumResponseModel.FromJson(response.Content);
            return (true, ApiResult<GetStudentCurriculumResponseModel, ErrorResponseModel>.Success(raw));
        }
        catch (Exception ex) when (ex is not JsonException)
        {
            throw new Exception("Network error occurred", ex);
        }*/
    }

}