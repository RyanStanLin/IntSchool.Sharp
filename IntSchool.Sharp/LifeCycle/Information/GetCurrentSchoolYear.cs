using System.Net;
using IntSchool.Sharp.Data.EventArguments;
using IntSchool.Sharp.Models;
using Newtonsoft.Json;
using RestSharp;

namespace IntSchool.Sharp.LifeCycle;

public partial class API
{
    public (bool isSuccess, ApiResult<GetCurrentSchoolYearResponseModel, ErrorResponseModel>? apiResult) GetCurrentSchoolYear()
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        RestRequest request = new RestRequest(resource: Constants.GetCurrentSchoolYearPath, method: Method.Get)
            .AddHeader(Constants.JsonXPathKey, XToken);
        try
        {
            var response = Client.Execute(request);

            if (response.StatusCode is HttpStatusCode.BadRequest)
            {
                try
                {
                    var error = ErrorResponseModel.FromJson(response.Content);
                    var result = ApiResult<GetCurrentSchoolYearResponseModel, ErrorResponseModel>.Error(error);
                    var eventArgs =
                        new UnauthorizedErrorEventArgs(
                            timestamp:error.Timestamp.DateTime,
                            xToken:XToken
                        );
                    OnUnauthorizedError?.Invoke(this, eventArgs);
                    
                    return (false, result);
                }
                catch (JsonException ex)
                {
                    var eventArgs = new FallbackJsonErrorEventArgs(
                        timestamp: DateTime.Now,
                        json: response.Content,
                        jsonException: ex);
                    OnFallbackJsonError?.Invoke(this, eventArgs);
                    return (false, null);
                }
            }
            
            var raw = GetCurrentSchoolYearResponseModel.FromJson(response.Content);
            return (true, ApiResult<GetCurrentSchoolYearResponseModel, ErrorResponseModel>.Success(raw));
        }
        catch (Exception ex) when (ex is not JsonException)
        {
            throw new Exception("Network error occurred", ex);
        }
    }
}