using IntSchool.Sharp.Core.Models;
using Newtonsoft.Json;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<SendSmsResponseModel, ErrorResponseModel> SendSms(string areaCode, string mobile)
    {
        RestRequest request = new RestRequest(resource: Constants.ApiSendSmsPath, method: Method.Get)
            .AddQueryParameter("areaCode", areaCode)
            .AddQueryParameter("mobile", mobile);
        return TryExecute(
            request,
            SendSmsResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }
}