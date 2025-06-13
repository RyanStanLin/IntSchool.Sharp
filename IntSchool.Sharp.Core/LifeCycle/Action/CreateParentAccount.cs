using System.Threading.Tasks;
using IntSchool.Sharp.Core.Models;
using IntSchool.Sharp.Core.Extensions;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<ErrorResponseModel> CreateParentAccount(CreateParentAccountRequestModel model, string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);

        var request = BuildCreateParentAccountRequest(model, schoolId);

        return TryExecute(
            request,
            ErrorResponseModel.FromJson
        );
    }

    public async Task<ApiResult<ErrorResponseModel>> CreateParentAccountAsync(CreateParentAccountRequestModel model, string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);

        var request = BuildCreateParentAccountRequest(model, schoolId);

        return await TryExecuteAsync(
            request,
            ErrorResponseModel.FromJson
        );
    }

    private RestRequest BuildCreateParentAccountRequest(CreateParentAccountRequestModel model, string schoolId)
    {
        string body = model.ToJson();
        return new RestRequest(Constants.CreateParentPath, Method.Post)
            .AddHeader(Constants.JsonXSchoolId, schoolId)
            .AddHeader(Constants.JsonXPathKey, XToken)
            .AddBody(body);
    }
}