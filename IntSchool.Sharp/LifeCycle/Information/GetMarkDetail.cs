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
    public (bool isSuccess, ApiResult<GetMarkDetailResponseModel, ErrorResponseModel>? apiResult) GetMarkDetail(
        string taskStudentId, string studentId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        ArgumentException.ThrowIfNullOrEmpty(studentId);
        ArgumentException.ThrowIfNullOrEmpty(taskStudentId);
        //ArgumentException.ThrowIfNullOrEmpty(studentId); Already done in side the declaration of GetStudentCurriculumConfiguration
        RestRequest request = new RestRequest(resource: Constants.GetMarkDetailPath, method: Method.Get)
            .AddHeader(Constants.JsonXPathKey, XToken)
            .AddQueryParameter(Constants.JsonTaskStudentIdKey, taskStudentId)
            .AddQueryParameter(Constants.JsonStudentIdKey, studentId);
        try
        {
            var response = Client.Execute(request);

            if (response.StatusCode is HttpStatusCode.BadRequest)
            {
                try
                {
                    var error = ErrorResponseModel.FromJson(response.Content);
                    var result = ApiResult<GetMarkDetailResponseModel, ErrorResponseModel>.Error(error);
                    var eventArgs =
                        new UnauthorizedErrorEventArgs(
                            timestamp: error.Timestamp.DateTime,
                            xToken: XToken
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

            var raw = GetMarkDetailResponseModel.FromJson(response.Content);
            return (true, ApiResult<GetMarkDetailResponseModel, ErrorResponseModel>.Success(raw));
        }
        catch (Exception ex) when (ex is not JsonException)
        {
            throw new Exception("Network error occurred", ex);
        }
    }

}