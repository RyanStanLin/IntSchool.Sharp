using IntSchool.Sharp.Core.Models;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<SendVerificationCodeResponseModel, ErrorResponseModel> SendEMailCode(string accountPhoneNumber)
    {
        RestRequest request = new RestRequest(resource: Constants.SendEMailCodePath, method: Method.Get)
            .AddQueryParameter(Constants.JsonAccountKey, accountPhoneNumber);
        return TryExecute(
            request,
            SendVerificationCodeResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }
}