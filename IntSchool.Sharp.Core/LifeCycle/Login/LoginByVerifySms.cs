using IntSchool.Sharp.Core.Models;
using Newtonsoft.Json;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<LoginResponseModel, ErrorResponseModel> LoginByVerifySms(LoginByVerifySmsRequestModel loginByVerifySmsRequestModel)
    {
        string body = loginByVerifySmsRequestModel.ToJson();
        RestRequest request = new RestRequest(resource: Constants.ApiLoginPath, method: Method.Post)
            .AddBody(body, contentType: Constants.JsonUtf8ContentType);
        return TryExecute(
            request,
            LoginResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }
}