using System.Net;
using IntSchool.Sharp.Data.EventArguments;
using IntSchool.Sharp.Extensions;
using IntSchool.Sharp.Models;
using IntSchool.Sharp.RequestConfigs;
using Newtonsoft.Json;
using RestSharp;

namespace IntSchool.Sharp.LifeCycle;

public partial class API
{
    public (bool isSuccess, ApiResult<GetAttendanceResponseModel, ErrorResponseModel>? apiResult) GetAttendance(SharedStudentTimespanConfiguration configuration, string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        //ArgumentException.ThrowIfNullOrEmpty(studentId); Already done in side the declaration of SharedStudentTimespanConfiguration
        RestRequest request = new RestRequest(resource: Constants.GetAttendancePath + configuration.SchoolYearId, method: Method.Get)
            .AddHeader(Constants.JsonXPathKey, XToken)
            .AddHeader(Constants.JsonXSchoolId, schoolId)
            .AddQueryParameter(Constants.JsonStudentIdKey, configuration.StudentId)
            .AddQueryParameter(Constants.JsonStartTimeKey, configuration.StartTime.ToUnixTimestampMilliseconds())
            .AddQueryParameter(Constants.JsonEndTimeKey, configuration.EndTime.ToUnixTimestampMilliseconds());
        return TryExecute(
            request,
            GetAttendanceResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
        /*try
        {
            var response = Client.Execute(request);

            if (response.StatusCode is not HttpStatusCode.OK)
            {
                try
                {
                    var error = ErrorResponseModel.FromJson(response.Content);
                    var result = ApiResult<GetAttendanceResponseModel, ErrorResponseModel>.Error(error);
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
            
            var raw = GetAttendanceResponseModel.FromJson(response.Content);
            return (true, ApiResult<GetAttendanceResponseModel, ErrorResponseModel>.Success(raw));
        }
        catch (Exception ex) when (ex is not JsonException)
        {
            throw new Exception("Network error occurred", ex);
        }*/
    }

}