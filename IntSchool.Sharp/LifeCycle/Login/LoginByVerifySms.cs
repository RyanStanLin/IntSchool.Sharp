using IntSchool.Sharp.Models;
using Newtonsoft.Json;
using RestSharp;

namespace IntSchool.Sharp.LifeCycle;

public partial class API
{
    public (bool isSuccess,LoginResponseModel raw) LoginByVerifySms(LoginByVerifySmsRequestModel loginByVerifySmsRequestModel, bool replaceXTokenIfSuccess = true)
    {
        string body = loginByVerifySmsRequestModel.ToJson();
        RestRequest request = new RestRequest(resource: Constants.ApiLoginPath, method: Method.Post)
            .AddBody(body, contentType: Constants.JsonUtf8ContentType);
        try
        {
            var response = Client.Execute(request);
            var raw = LoginResponseModel.FromJson(response.Content);
            
            if (raw.Success is false)
                throw new Exception($"Login failed: {raw.Msg}");
            
            if (raw.Success && replaceXTokenIfSuccess)
                XToken = raw.Token;

            return (raw.Success,raw);
        }
        catch (Exception ex) when (ex is not JsonException)
        {
            throw new Exception("Network error occurred", ex);
        }
    }
}