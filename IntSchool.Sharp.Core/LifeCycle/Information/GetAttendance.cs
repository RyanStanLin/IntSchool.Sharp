using System.Threading.Tasks;
using IntSchool.Sharp.Core.Models;
using IntSchool.Sharp.Core.RequestConfigs;
using IntSchool.Sharp.Core.Extensions;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<GetAttendanceResponseModel, ErrorResponseModel> GetAttendance(SharedStudentTimespanConfiguration configuration, string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);
        
        var request = BuildGetAttendanceRequest(configuration, schoolId);

        return TryExecute(
            request,
            GetAttendanceResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    public async Task<ApiResult<GetAttendanceResponseModel, ErrorResponseModel>> GetAttendanceAsync(SharedStudentTimespanConfiguration configuration, string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);

        var request = BuildGetAttendanceRequest(configuration, schoolId);

        return await TryExecuteAsync(
            request,
            GetAttendanceResponseModel.FromJson,
            ErrorResponseModel.FromJson
        );
    }

    private RestRequest BuildGetAttendanceRequest(SharedStudentTimespanConfiguration configuration, string schoolId)
    {
        return new RestRequest(Constants.GetAttendancePath + configuration.SchoolYearId, Method.Get)
            .AddHeader(Constants.JsonXPathKey, XToken)
            .AddHeader(Constants.JsonXSchoolId, schoolId)
            .AddQueryParameter(Constants.JsonStudentIdKey, configuration.StudentId)
            .AddQueryParameter(Constants.JsonStartTimeKey, configuration.StartTime.ToUnixTimestampMilliseconds())
            .AddQueryParameter(Constants.JsonEndTimeKey, configuration.EndTime.ToUnixTimestampMilliseconds());
    }
}