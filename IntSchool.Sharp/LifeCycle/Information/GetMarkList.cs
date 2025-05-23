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
    public (bool isSuccess, ApiResult<GetMarkListResponseModel, ErrorResponseModel>? apiResult) GetMarkList(
        string courseId, string schoolYearId, string studentId, GetPageControlConfiguration? configuration = null, 
        string? nameFilter = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        ArgumentException.ThrowIfNullOrEmpty(courseId);
        ArgumentException.ThrowIfNullOrEmpty(schoolYearId);
        ArgumentException.ThrowIfNullOrEmpty(studentId);
        //ArgumentException.ThrowIfNullOrEmpty(studentId); Already done in side the declaration of GetStudentCurriculumConfiguration
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
        try
        {
            var response = Client.Execute(request);

            if (response.StatusCode is HttpStatusCode.BadRequest)
            {
                try
                {
                    var error = ErrorResponseModel.FromJson(response.Content);
                    var result = ApiResult<GetMarkListResponseModel, ErrorResponseModel>.Error(error);
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

            var raw = GetMarkListResponseModel.FromJson(response.Content);
            return (true, ApiResult<GetMarkListResponseModel, ErrorResponseModel>.Success(raw));
        }
        catch (Exception ex) when (ex is not JsonException)
        {
            throw new Exception("Network error occurred", ex);
        }
    }

}