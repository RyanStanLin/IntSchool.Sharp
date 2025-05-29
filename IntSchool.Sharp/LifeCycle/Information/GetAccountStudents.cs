using System.Net;
using IntSchool.Sharp.Data.EventArguments;
using IntSchool.Sharp.Models;
using Newtonsoft.Json;
using RestSharp;

namespace IntSchool.Sharp.LifeCycle;

public partial class API
{
    public (bool isSuccess, ApiResult<List<GetAccountStudentsResponseModel>, ErrorResponseModel>? apiResult) GetAccountStudents()
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        RestRequest request = new RestRequest(resource: Constants.GetAccountStudentsPath, method: Method.Get)
            .AddHeader(Constants.JsonXPathKey, XToken);
        return TryExecute(
            request,
            GetAccountStudentsResponseModel.FromJson,
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
                    var result = ApiResult<List<GetAccountStudentsResponseModel>, ErrorResponseModel>.Error(error);
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
            
            var raw = GetAccountStudentsResponseModel.FromJson(response.Content);
            return (true, ApiResult<List<GetAccountStudentsResponseModel>, ErrorResponseModel>.Success(raw));
        }
        catch (Exception ex) when (ex is not JsonException)
        {
            throw new Exception("Network error occurred", ex);
        }*/
    }
}