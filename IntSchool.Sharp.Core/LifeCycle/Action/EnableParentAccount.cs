using System.Threading.Tasks;
using IntSchool.Sharp.Core.Models;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<ErrorResponseModel> EnableParentAccount(long parentId, string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);

        var request = BuildEnableParentAccountRequest(parentId, schoolId);

        return TryExecute(
            request,
            ErrorResponseModel.FromJson
        );
    }

    public async Task<ApiResult<ErrorResponseModel>> EnableParentAccountAsync(long parentId, string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);

        var request = BuildEnableParentAccountRequest(parentId, schoolId);

        return await TryExecuteAsync(
            request,
            ErrorResponseModel.FromJson
        );
    }

    private RestRequest BuildEnableParentAccountRequest(long parentId, string schoolId)
    {
        var model = new ParentAccountIdRequestModel()
        {
            ParentId = parentId
        };
        string body = model.ToJson();

        return new RestRequest(Constants.EnableParentPath, Method.Put)
            .AddHeader(Constants.JsonXPathKey, XToken)
            .AddHeader(Constants.JsonXSchoolId, schoolId)
            .AddBody(body);
    }
}