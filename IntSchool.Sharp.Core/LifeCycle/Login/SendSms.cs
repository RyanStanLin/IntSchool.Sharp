using IntSchool.Sharp.Core.Models;
using Newtonsoft.Json;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class API
{
    public (bool isSuccess,SendSmsResponseModel raw) SendSms(string areaCode, string mobile)
    {
        RestRequest request = new RestRequest(resource: Constants.ApiSendSmsPath, method: Method.Get)
            .AddQueryParameter("areaCode", areaCode)
            .AddQueryParameter("mobile", mobile);
        try
        {
            var response = _client.Execute(request);
            var raw = SendSmsResponseModel.FromJson(response.Content);
            
            if (raw.Success is false)
                throw new Exception($"SMS sending failed: {raw.ResMsg}");
            
            return (raw.Success,raw);
        }
        catch (Exception ex) when (ex is not JsonException)
        {
            throw new Exception("Network error occurred", ex);
        }
    }
}