using IntSchool.Sharp.Core.Models;
using IntSchool.Sharp.Core.Extensions;
using IntSchool.Sharp.Core.RequestConfigs;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class API
{
    public (bool isSuccess, ApiResult<ErrorResponseModel>? apiResult) CreateParentAccount(CreateParentAccountRequestModel model, string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        string body = model.ToJson();
        RestRequest request = new RestRequest(resource: Constants.CreateParentPath, method: Method.Post)
            .AddHeader(Constants.JsonXSchoolId, schoolId)
            .AddHeader(Constants.JsonXPathKey, XToken)
            .AddBody(body);
        return TryExecute(
            request,
            ErrorResponseModel.FromJson
        );
    }
}