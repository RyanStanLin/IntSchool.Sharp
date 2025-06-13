using System.Threading.Tasks;
using IntSchool.Sharp.Core.Models;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<SendVerificationCodeResponseModel, ErrorResponseModel> SendEMailCode(string accountPhoneNumber)
    {
        var request = BuildSendEMailCodeRequest(accountPhoneNumber);

        return TryExecute(
            request,
            SendVerificationCodeResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    public async Task<ApiResult<SendVerificationCodeResponseModel, ErrorResponseModel>> SendEMailCodeAsync(string accountPhoneNumber)
    {
        var request = BuildSendEMailCodeRequest(accountPhoneNumber);

        return await TryExecuteAsync(
            request,
            SendVerificationCodeResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    private RestRequest BuildSendEMailCodeRequest(string accountPhoneNumber)
    {
        return new RestRequest(Constants.SendEMailCodePath, Method.Get)
            .AddQueryParameter(Constants.JsonAccountKey, accountPhoneNumber);
    }
}