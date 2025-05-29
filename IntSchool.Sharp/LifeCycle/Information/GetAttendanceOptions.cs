using System.Net;
using IntSchool.Sharp.Data.EventArguments;
using IntSchool.Sharp.Models;
using Newtonsoft.Json;
using RestSharp;

namespace IntSchool.Sharp.LifeCycle;

public partial class API
{
    public (bool isSuccess, ApiResult<List<GetAttendanceOptionsResponseModel>, ErrorResponseModel>? apiResult) GetAttendanceOptions()
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        RestRequest request = new RestRequest(resource: Constants.GetAttendanceOptionsPath, method: Method.Get)
            .AddHeader(Constants.JsonXPathKey, XToken);
        return TryExecute(
            request,
            GetAttendanceOptionsResponseModel.FromJson,
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
                    var result = ApiResult<List<GetAttendanceOptionsResponseModel>, ErrorResponseModel>.Error(error);
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
            
            var raw = GetAttendanceOptionsResponseModel.FromJson(response.Content);
            return (true, ApiResult<List<GetAttendanceOptionsResponseModel>, ErrorResponseModel>.Success(raw));
        }
        catch (Exception ex) when (ex is not JsonException)
        {
            throw new Exception("Network error occurred", ex);
        }*/
    }
}