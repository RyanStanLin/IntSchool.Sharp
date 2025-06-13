using System.Threading.Tasks;
using IntSchool.Sharp.Core.Models;
using Newtonsoft.Json;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<SendVerificationCodeResponseModel, ErrorResponseModel> SendSmsCode(string areaCode, string mobile)
    {
        var request = BuildSendSmsCodeRequest(areaCode, mobile);
        
        return TryExecute(
            request,
            SendVerificationCodeResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    public async Task<ApiResult<SendVerificationCodeResponseModel, ErrorResponseModel>> SendSmsCodeAsync(string areaCode, string mobile)
    {
        var request = BuildSendSmsCodeRequest(areaCode, mobile);

        return await TryExecuteAsync(
            request,
            SendVerificationCodeResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    private RestRequest BuildSendSmsCodeRequest(string areaCode, string mobile)
    {
        return new RestRequest(Constants.SendSmsCodePath, Method.Get)
            .AddQueryParameter(Constants.JsonAreaCodeKey, areaCode)
            .AddQueryParameter(Constants.JsonMobileKey, mobile);
    }
}