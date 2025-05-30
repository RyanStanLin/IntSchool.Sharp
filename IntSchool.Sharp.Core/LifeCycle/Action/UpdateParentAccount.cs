using IntSchool.Sharp.Core.Models;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class API
{
    public (bool isSuccess, ApiResult<ErrorResponseModel>? apiResult) UpdateParentAccount(UpdateParentAccountRequestModel model, string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        string body = model.ToJson();
        RestRequest request = new RestRequest(resource: Constants.UpdateParentPath, method: Method.Put)
            .AddBody(body)
            .AddHeader(Constants.JsonXSchoolId, schoolId)
            .AddHeader(Constants.JsonXPathKey, XToken);
        return TryExecute(
            request,
            ErrorResponseModel.FromJson
        );
    }
}