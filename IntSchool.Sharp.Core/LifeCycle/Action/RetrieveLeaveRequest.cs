using System.Threading.Tasks;
using IntSchool.Sharp.Core.Models;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<ErrorResponseModel> RetrieveLeaveRequest(string leaveRequestRequestId, string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        ArgumentException.ThrowIfNullOrEmpty(leaveRequestRequestId);
        
        var request = BuildRetrieveLeaveRequestRequest(leaveRequestRequestId, schoolId);

        return TryExecute(
            request,
            ErrorResponseModel.FromJson
        );
    }

    public async Task<ApiResult<ErrorResponseModel>> RetrieveLeaveRequestAsync(string leaveRequestRequestId, string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        ArgumentException.ThrowIfNullOrEmpty(leaveRequestRequestId);

        var request = BuildRetrieveLeaveRequestRequest(leaveRequestRequestId, schoolId);

        return await TryExecuteAsync(
            request,
            ErrorResponseModel.FromJson
        );
    }

    private RestRequest BuildRetrieveLeaveRequestRequest(string leaveRequestRequestId, string schoolId)
    {
        return new RestRequest(Constants.RetrieveLeaveRequestPath, Method.Put)
            .AddQueryParameter(Constants.JsonLeaveApplicationIdKey, leaveRequestRequestId)
            .AddHeader(Constants.JsonXSchoolId, schoolId)
            .AddHeader(Constants.JsonXPathKey, XToken);
    }
}