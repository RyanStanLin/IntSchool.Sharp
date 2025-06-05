using IntSchool.Sharp.Core.Models;
using IntSchool.Sharp.Models;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;
public partial class Api
{
    public ApiResult<LoginResponseModel, ErrorResponseModel> ResetPasswordByEMailVerificationCodeAndLogin(string email, string verificationCode
    , string newPassword)
    {
        var raw = new LoginResetPasswordByVerificationCodeResponseModel()
        {
            Email = email,
            Mobile = email,
            NewPassword = newPassword,
            Vcode = verificationCode,
            VerifyType = Constants.DefaultEmailKeyValue
        };
        string body = raw.ToJson();
        RestRequest request = new RestRequest(resource: Constants.LoginPath, method: Method.Put)
            .AddBody(body, contentType: Constants.JsonUtf8ContentType);
        return TryExecute(
            request,
            LoginResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }
}