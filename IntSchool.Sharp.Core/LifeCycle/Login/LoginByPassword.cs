using System.Threading.Tasks;
using IntSchool.Sharp.Core.Models;
using IntSchool.Sharp.Models;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<LoginResponseModel, ErrorResponseModel> LoginByPassword(string account, string password)
    {
        var request = BuildLoginByPasswordRequest(account, password);
        
        return TryExecute(
            request,
            LoginResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    public async Task<ApiResult<LoginResponseModel, ErrorResponseModel>> LoginByPasswordAsync(string account, string password)
    {
        var request = BuildLoginByPasswordRequest(account, password);

        return await TryExecuteAsync(
            request,
            LoginResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    private RestRequest BuildLoginByPasswordRequest(string account, string password)
    {
        var raw = new LoginByPasswordRequestModel()
        {
            Account = account,
            Password = password
        };
        string body = raw.ToJson();
        
        return new RestRequest(Constants.LoginPath, Method.Post)
            .AddBody(body, contentType: Constants.JsonUtf8ContentType);
    }
}