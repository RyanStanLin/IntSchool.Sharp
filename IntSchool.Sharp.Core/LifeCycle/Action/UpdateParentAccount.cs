using System.Threading.Tasks;
using IntSchool.Sharp.Core.Models;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<ErrorResponseModel> UpdateParentAccount(UpdateParentAccountRequestModel model, string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);

        var request = BuildUpdateParentAccountRequest(model, schoolId);

        return TryExecute(
            request,
            ErrorResponseModel.FromJson
        );
    }

    public async Task<ApiResult<ErrorResponseModel>> UpdateParentAccountAsync(UpdateParentAccountRequestModel model, string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);

        var request = BuildUpdateParentAccountRequest(model, schoolId);

        return await TryExecuteAsync(
            request,
            ErrorResponseModel.FromJson
        );
    }

    private RestRequest BuildUpdateParentAccountRequest(UpdateParentAccountRequestModel model, string schoolId)
    {
        string body = model.ToJson();
        return new RestRequest(Constants.UpdateParentPath, Method.Put)
            .AddBody(body)
            .AddHeader(Constants.JsonXSchoolId, schoolId)
            .AddHeader(Constants.JsonXPathKey, XToken);
    }
}