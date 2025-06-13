using System.Threading.Tasks;
using IntSchool.Sharp.Core.Models;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<ErrorResponseModel> DisableParentAccount(long parentId, string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);

        var request = BuildDisableParentAccountRequest(parentId, schoolId);
        
        return TryExecute(
            request,
            ErrorResponseModel.FromJson
        );
    }

    public async Task<ApiResult<ErrorResponseModel>> DisableParentAccountAsync(long parentId, string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);

        var request = BuildDisableParentAccountRequest(parentId, schoolId);

        return await TryExecuteAsync(
            request,
            ErrorResponseModel.FromJson
        );
    }

    private RestRequest BuildDisableParentAccountRequest(long parentId, string schoolId)
    {
        var model = new ParentAccountIdRequestModel()
        {
            ParentId = parentId
        };
        string body = model.ToJson();

        return new RestRequest(Constants.DisableParentPath, Method.Put)
            .AddHeader(Constants.JsonXPathKey, XToken)
            .AddHeader(Constants.JsonXSchoolId, schoolId)
            .AddBody(body);
    }
}