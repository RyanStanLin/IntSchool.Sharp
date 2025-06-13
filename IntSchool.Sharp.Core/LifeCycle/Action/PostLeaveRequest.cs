using System.Collections.Generic;
using System.Threading.Tasks;
using IntSchool.Sharp.Core.Extensions;
using IntSchool.Sharp.Core.Models;
using IntSchool.Sharp.Core.RequestConfigs;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api
{
    public ApiResult<ErrorResponseModel> PostLeaveRequest(PostLeaveConfiguration config, string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);

        var request = BuildPostLeaveRequestRequest(config, schoolId);

        return TryExecute(
            request,
            ErrorResponseModel.FromJson
        );
    }

    public async Task<ApiResult<ErrorResponseModel>> PostLeaveRequestAsync(PostLeaveConfiguration config, string schoolId = Constants.DefaultSchoolId)
    {
        ArgumentException.ThrowIfNullOrEmpty(XToken);

        var request = BuildPostLeaveRequestRequest(config, schoolId);

        return await TryExecuteAsync(
            request,
            ErrorResponseModel.FromJson
        );
    }

    private RestRequest BuildPostLeaveRequestRequest(PostLeaveConfiguration config, string schoolId)
    {
        var model = new PostLeaveRequestRequestModel()
        {
            StartTime = DateTimeTimestampExtension.ToUnixTimestampMilliseconds(config.StartTime),
            EndTime = DateTimeTimestampExtension.ToUnixTimestampMilliseconds(config.EndTime),
            Reason = config.Message,
            ReasonId = (short)config.Reason,
            ResourceIds = new List<object>(),
            StudentId = config.StudentId,
            Type = config.Type.GetDescription()
        };
        string body = model.ToJson();
        
        return new RestRequest(Constants.LeavesPath, Method.Post)
            .AddBody(body)
            .AddHeader(Constants.JsonXSchoolId, schoolId)
            .AddHeader(Constants.JsonXPathKey, XToken);
    }
}