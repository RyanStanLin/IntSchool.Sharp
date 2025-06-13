using System.Net;
using System.Threading.Tasks;
using IntSchool.Sharp.Core.Models;
using IntSchool.Sharp.Core.Data.EventArguments;
using Newtonsoft.Json;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<GetStudentDetailResponseModel, ErrorResponseModel> GetStudentDetail(string studentId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        ArgumentException.ThrowIfNullOrEmpty(studentId);
        
        var request = BuildGetStudentDetailRequest(studentId);

        return TryExecute(
            request,
            GetStudentDetailResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    public async Task<ApiResult<GetStudentDetailResponseModel, ErrorResponseModel>> GetStudentDetailAsync(string studentId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        ArgumentException.ThrowIfNullOrEmpty(studentId);

        var request = BuildGetStudentDetailRequest(studentId);

        return await TryExecuteAsync(
            request,
            GetStudentDetailResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }
    
    private RestRequest BuildGetStudentDetailRequest(string studentId)
    {
        return new RestRequest(Constants.GetStudentDetailPath, Method.Get)
            .AddHeader(Constants.JsonXPathKey, XToken)
            .AddQueryParameter(Constants.JsonStudentIdKey, studentId);
    }
}