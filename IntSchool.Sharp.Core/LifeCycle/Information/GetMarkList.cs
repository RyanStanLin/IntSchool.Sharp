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
    public (bool isSuccess, ApiResult<GetMarkListResponseModel, ErrorResponseModel>? apiResult) GetMarkList(
        string courseId, string schoolYearId, string studentId, GetPageControlConfiguration? configuration = null, 
        string? nameFilter = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        ArgumentException.ThrowIfNullOrEmpty(courseId);
        ArgumentException.ThrowIfNullOrEmpty(schoolYearId);
        ArgumentException.ThrowIfNullOrEmpty(studentId);
        //ArgumentException.ThrowIfNullOrEmpty(studentId); Already done in side the declaration of SharedStudentTimespanConfiguration
        RestRequest request = new RestRequest(resource: Constants.GetMarkListPath, method: Method.Get)
            .AddHeader(Constants.JsonXPathKey, XToken)
            .AddQueryParameter(Constants.JsonCourseIdKey, courseId)
            .AddQueryParameter(Constants.JsonSchoolYearIdKey, schoolYearId)
            .AddQueryParameter(Constants.JsonStudentIdKey, studentId);
        if (configuration is not null)
            request.AddQueryParameter(Constants.JsonPageSizeKey, configuration.PageSize)
                .AddQueryParameter(Constants.JsonPageCurrentKay, configuration.PageCurrent);
        if(nameFilter is not not null or "")
            request.AddQueryParameter(Constants.JsonNameKey, nameFilter);
        return TryExecute(
            request,
            GetMarkListResponseModel.FromJson,
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
                    var result = ApiResult<GetMarkListResponseModel, ErrorResponseModel>.Error(error);
                    var eventArgs =
                        new RemoteErrorEventArgs(
                            timestamp: error.Timestamp.DateTime,
                            raw: error,
                            xToken: XToken
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

            var raw = GetMarkListResponseModel.FromJson(response.Content);
            return (true, ApiResult<GetMarkListResponseModel, ErrorResponseModel>.Success(raw));
        }
        catch (Exception ex) when (ex is not JsonException)
        {
            throw new Exception("Network error occurred", ex);
        }*/
    }

}