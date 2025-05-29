using System.Net;
using IntSchool.Sharp.Models;
using Newtonsoft.Json;
using RestSharp;

namespace IntSchool.Sharp.LifeCycle;

public partial class API
{
    public (bool isSuccess, ApiResult<ErrorResponseModel>? apiResult) RetrieveLeaveRequest(string leaveRequestRequestId, string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        ArgumentException.ThrowIfNullOrEmpty(leaveRequestRequestId);
        RestRequest request = new RestRequest(resource: Constants.RetrieveLeaveRequestPath, method: Method.Put)
            .AddQueryParameter(Constants.JsonLeaveApplicationIdKey, leaveRequestRequestId)
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
                return false;
            return true;
        }
        catch (Exception ex) when (ex is not JsonException)
        {
            throw new Exception("Network error occurred", ex);
        }*/
    }
}