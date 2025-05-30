using System.Net;
using IntSchool.Sharp.Core.Models;
using Newtonsoft.Json;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

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
    }
}