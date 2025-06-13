using System.Collections.Generic;
using System.Threading.Tasks;
using IntSchool.Sharp.Core.Models;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<List<GetAccountStudentsResponseModel>, ErrorResponseModel> GetAccountStudents()
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        
        var request = BuildGetAccountStudentsRequest();
        
        return TryExecute(
            request,
            GetAccountStudentsResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    public async Task<ApiResult<List<GetAccountStudentsResponseModel>, ErrorResponseModel>> GetAccountStudentsAsync()
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);

        var request = BuildGetAccountStudentsRequest();

        return await TryExecuteAsync(
            request,
            GetAccountStudentsResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    private RestRequest BuildGetAccountStudentsRequest()
    {
        return new RestRequest(Constants.GetAccountStudentsPath, Method.Get)
            .AddHeader(Constants.JsonXPathKey, XToken);
    }
}