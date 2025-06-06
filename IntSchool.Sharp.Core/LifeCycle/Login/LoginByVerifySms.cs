using IntSchool.Sharp.Core.Models;
using Newtonsoft.Json;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<LoginResponseModel, ErrorResponseModel> LoginByVerifySms(string phoneNumber, string verificationCode, string areaCode = Constants.DefaultAreaCode)
    {
        var raw = new LoginByVerifySmsRequestModel()
        {
            Account = phoneNumber,
            AreaCode = areaCode,
            Vcode = verificationCode
        };
        string body = raw.ToJson();
        RestRequest request = new RestRequest(resource: Constants.LoginPath, method: Method.Post)
            .AddBody(body, contentType: Constants.JsonUtf8ContentType);
        return TryExecute(
            request,
            LoginResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }
}