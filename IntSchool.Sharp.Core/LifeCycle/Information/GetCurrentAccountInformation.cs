using System.Threading.Tasks;
using IntSchool.Sharp.Core.Models;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<GetCurrentAccountInformationResponseModel, ErrorResponseModel> GetCurrentAccountInformation()
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);

        var request = BuildGetCurrentAccountInformationRequest();
        
        return TryExecute(
            request,
            GetCurrentAccountInformationResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    public async Task<ApiResult<GetCurrentAccountInformationResponseModel, ErrorResponseModel>> GetCurrentAccountInformationAsync()
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);

        var request = BuildGetCurrentAccountInformationRequest();

        return await TryExecuteAsync(
            request,
            GetCurrentAccountInformationResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    private RestRequest BuildGetCurrentAccountInformationRequest()
    {
        return new RestRequest(Constants.GetAccountInfoPath, Method.Get)
            .AddHeader(Constants.JsonXPathKey, XToken);
    }
}