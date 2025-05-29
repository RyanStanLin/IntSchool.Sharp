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
    public (bool isSuccess, ApiResult<ErrorResponseModel>? apiResult) PostLeaveRequest(PostLeaveConfiguration config, string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        var model = new PostLeaveRequestRequestModel()
        {
            StartTime = DateTimeToTimestampExtension.ToUnixTimestampMilliseconds(config.StartTime),
            EndTime = DateTimeToTimestampExtension.ToUnixTimestampMilliseconds(config.EndTime),
            Reason = config.Message,
            ReasonId = (short)config.Reason,
            ResourceIds = new List<object>(),
            StudentId = config.StudentId,
            Type = config.Type.GetDescription()
        };
        string body = model.ToJson();
        RestRequest request = new RestRequest(resource: Constants.LeavesPath, method: Method.Post)
            .AddBody(body)
            .AddHeader(Constants.JsonXSchoolId, schoolId)
            .AddHeader(Constants.JsonXPathKey, XToken);
        return TryExecute(
            request,
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
                    var eventArgs =
                        new RemoteErrorEventArgs(
                            timestamp:error.Timestamp.DateTime,
                            raw: error,
                            xToken:XToken
                        );
                    OnRemoteError?.Invoke(this, eventArgs);
                    var result = ApiResult<ErrorResponseModel>.Error(error);
                    return (false,result);
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
            var raw = ApiResult<ErrorResponseModel>.Success();
            return (true,raw);
        }
        catch (Exception ex) when (ex is not JsonException)
        {
            throw new Exception("Network error occurred", ex);
        }*/
    }
}