using System.Threading.Tasks;
using IntSchool.Sharp.Core.Models;
using IntSchool.Sharp.Models;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;
public partial class Api
{
    public ApiResult<LoginResponseModel, ErrorResponseModel> ResetPasswordByEMailVerificationCodeAndLogin(string email, string verificationCode, string newPassword)
    {
        var request = BuildResetPasswordByEMailVerificationCodeAndLoginRequest(email, verificationCode, newPassword);
        
        return TryExecute(
            request,
            LoginResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    public async Task<ApiResult<LoginResponseModel, ErrorResponseModel>> ResetPasswordByEMailVerificationCodeAndLoginAsync(string email, string verificationCode, string newPassword)
    {
        var request = BuildResetPasswordByEMailVerificationCodeAndLoginRequest(email, verificationCode, newPassword);

        return await TryExecuteAsync(
            request,
            LoginResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    private RestRequest BuildResetPasswordByEMailVerificationCodeAndLoginRequest(string email, string verificationCode, string newPassword)
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
        
        return new RestRequest(Constants.LoginPath, Method.Put)
            .AddBody(body, contentType: Constants.JsonUtf8ContentType);
    }
}