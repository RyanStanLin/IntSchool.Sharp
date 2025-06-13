using System.Threading.Tasks;
using IntSchool.Sharp.Core.Models;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<GetMarkDetailResponseModel, ErrorResponseModel> GetMarkDetail(
        string taskStudentId, string studentId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        ArgumentException.ThrowIfNullOrEmpty(studentId);
        ArgumentException.ThrowIfNullOrEmpty(taskStudentId);

        var request = BuildGetMarkDetailRequest(taskStudentId, studentId);

        return TryExecute(
            request,
            GetMarkDetailResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    public async Task<ApiResult<GetMarkDetailResponseModel, ErrorResponseModel>> GetMarkDetailAsync(
        string taskStudentId, string studentId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        ArgumentException.ThrowIfNullOrEmpty(studentId);
        ArgumentException.ThrowIfNullOrEmpty(taskStudentId);

        var request = BuildGetMarkDetailRequest(taskStudentId, studentId);

        return await TryExecuteAsync(
            request,
            GetMarkDetailResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    private RestRequest BuildGetMarkDetailRequest(string taskStudentId, string studentId)
    {
        return new RestRequest(Constants.GetMarkDetailPath, Method.Get)
            .AddHeader(Constants.JsonXPathKey, XToken)
            .AddQueryParameter(Constants.JsonTaskStudentIdKey, taskStudentId)
            .AddQueryParameter(Constants.JsonStudentIdKey, studentId);
    }
}