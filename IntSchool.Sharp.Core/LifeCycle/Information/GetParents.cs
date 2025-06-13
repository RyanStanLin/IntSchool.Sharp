using System.Collections.Generic;
using System.Threading.Tasks;
using IntSchool.Sharp.Core.Models;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<List<GetParentsResponseModel>, ErrorResponseModel> GetParents(string studentId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        ArgumentException.ThrowIfNullOrEmpty(studentId);

        var request = BuildGetParentsRequest(studentId);

        return TryExecute(
            request,
            GetParentsResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    public async Task<ApiResult<List<GetParentsResponseModel>, ErrorResponseModel>> GetParentsAsync(string studentId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        ArgumentException.ThrowIfNullOrEmpty(studentId);

        var request = BuildGetParentsRequest(studentId);

        return await TryExecuteAsync(
            request,
            GetParentsResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }
    
    private RestRequest BuildGetParentsRequest(string studentId)
    {
        return new RestRequest(Constants.GetParentsPath, Method.Get)
            .AddHeader(Constants.JsonXPathKey, XToken)
            .AddQueryParameter(Constants.JsonStudentIdKey, studentId);
    }
}