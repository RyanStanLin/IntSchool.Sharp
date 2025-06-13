using System.Threading.Tasks;
using IntSchool.Sharp.Core.Models;
using Newtonsoft.Json;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<LoginResponseModel, ErrorResponseModel> LoginByVerifySms(string phoneNumber, string verificationCode, string areaCode = Constants.DefaultAreaCode)
    {
        var request = BuildLoginByVerifySmsRequest(phoneNumber, verificationCode, areaCode);

        return TryExecute(
            request,
            LoginResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    public async Task<ApiResult<LoginResponseModel, ErrorResponseModel>> LoginByVerifySmsAsync(string phoneNumber, string verificationCode, string areaCode = Constants.DefaultAreaCode)
    {
        var request = BuildLoginByVerifySmsRequest(phoneNumber, verificationCode, areaCode);

        return await TryExecuteAsync(
            request,
            LoginResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    private RestRequest BuildLoginByVerifySmsRequest(string phoneNumber, string verificationCode, string areaCode)
    {
        var raw = new LoginByVerifySmsRequestModel()
        {
            Account = phoneNumber,
            AreaCode = areaCode,
            Vcode = verificationCode
        };
        string body = raw.ToJson();
        
        return new RestRequest(Constants.LoginPath, Method.Post)
            .AddBody(body, contentType: Constants.JsonUtf8ContentType);
    }
}