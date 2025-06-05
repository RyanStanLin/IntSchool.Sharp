using IntSchool.Sharp.Core.Models;
using Newtonsoft.Json;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<SendVerificationCodeResponseModel, ErrorResponseModel> SendSmsCode(string areaCode, string mobile)
    {
        RestRequest request = new RestRequest(resource: Constants.SendSmsCodePath, method: Method.Get)
            .AddQueryParameter(Constants.JsonAreaCodeKey, areaCode)
            .AddQueryParameter(Constants.JsonMobileKey, mobile);
        return TryExecute(
            request,
            SendVerificationCodeResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }
}