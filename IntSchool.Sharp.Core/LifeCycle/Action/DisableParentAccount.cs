using IntSchool.Sharp.Core.Models;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public  ApiResult<ErrorResponseModel> DisableParentAccount(long parentId, string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        var model = new ParentAccountIdRequestModel()
        {
            ParentId = parentId
        };
        string body = model.ToJson();

        RestRequest request = new RestRequest(resource: Constants.DisableParentPath, method: Method.Put)
            .AddHeader(Constants.JsonXPathKey, XToken)
            .AddHeader(Constants.JsonXSchoolId, schoolId)
            .AddBody(body);
        return TryExecute(
            request,
            ErrorResponseModel.FromJson
        );
    }
}