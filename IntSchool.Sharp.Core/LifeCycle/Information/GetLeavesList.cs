using System.Threading.Tasks;
using IntSchool.Sharp.Core.Models;
using IntSchool.Sharp.Core.RequestConfigs;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;
public partial class Api
{
    public ApiResult<GetLeavesListResponseModel, ErrorResponseModel> GetLeavesList(
        string studentId, GetPageControlConfiguration configuration)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        ArgumentException.ThrowIfNullOrEmpty(studentId);

        var request = BuildGetLeavesListRequest(studentId, configuration);

        return TryExecute(
            request,
            GetLeavesListResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    public async Task<ApiResult<GetLeavesListResponseModel, ErrorResponseModel>> GetLeavesListAsync(
        string studentId, GetPageControlConfiguration configuration)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        ArgumentException.ThrowIfNullOrEmpty(studentId);

        var request = BuildGetLeavesListRequest(studentId, configuration);

        return await TryExecuteAsync(
            request,
            GetLeavesListResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    private RestRequest BuildGetLeavesListRequest(string studentId, GetPageControlConfiguration configuration)
    {
        return new RestRequest(Constants.LeavesPath, Method.Get)
            .AddHeader(Constants.JsonXPathKey, XToken)
            .AddQueryParameter(Constants.JsonStudentIdKey, studentId)
            .AddQueryParameter(Constants.JsonPageSizeKey, configuration.PageSize)
            .AddQueryParameter(Constants.JsonPageCurrentKay, configuration.PageCurrent);
    }
}