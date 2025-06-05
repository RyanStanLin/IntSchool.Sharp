using IntSchool.Sharp.Core.Models;
using IntSchool.Sharp.Models;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<LoginResponseModel, ErrorResponseModel> LoginByPassword(string account, string password)
    {
        var raw = new LoginByPasswordRequestModel()
        {
            Account = account,
            Password = password
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