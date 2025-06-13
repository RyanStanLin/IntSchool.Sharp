using System.Collections.Generic;
using System.Threading.Tasks;
using IntSchool.Sharp.Core.Models;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<List<GetAttendanceOptionsResponseModel>, ErrorResponseModel> GetAttendanceOptions(string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);

        var request = BuildGetAttendanceOptionsRequest(schoolId);

        return TryExecute(
            request,
            GetAttendanceOptionsResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    public async Task<ApiResult<List<GetAttendanceOptionsResponseModel>, ErrorResponseModel>> GetAttendanceOptionsAsync(string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);

        var request = BuildGetAttendanceOptionsRequest(schoolId);

        return await TryExecuteAsync(
            request,
            GetAttendanceOptionsResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    private RestRequest BuildGetAttendanceOptionsRequest(string schoolId)
    {
        return new RestRequest(Constants.GetAttendanceOptionsPath, Method.Get)
            .AddHeader(Constants.JsonXPathKey, XToken)
            .AddQueryParameter(Constants.JsonXSchoolId, schoolId);
    }
}