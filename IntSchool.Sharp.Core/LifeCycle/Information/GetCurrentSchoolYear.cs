using System.Threading.Tasks;
using IntSchool.Sharp.Core.Models;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<GetCurrentSchoolYearResponseModel, ErrorResponseModel> GetCurrentSchoolYear(string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);

        var request = BuildGetCurrentSchoolYearRequest(schoolId);

        return TryExecute(
            request,
            GetCurrentSchoolYearResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    public async Task<ApiResult<GetCurrentSchoolYearResponseModel, ErrorResponseModel>> GetCurrentSchoolYearAsync(string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);

        var request = BuildGetCurrentSchoolYearRequest(schoolId);

        return await TryExecuteAsync(
            request,
            GetCurrentSchoolYearResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    private RestRequest BuildGetCurrentSchoolYearRequest(string schoolId)
    {
        return new RestRequest(Constants.GetCurrentSchoolYearPath, Method.Get)
            .AddHeader(Constants.JsonXPathKey, XToken)
            .AddHeader(Constants.JsonXSchoolId, schoolId);
    }
}