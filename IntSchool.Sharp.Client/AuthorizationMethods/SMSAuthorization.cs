using IntSchool.Sharp.Client.Interfaces;
using IntSchool.Sharp.Core.LifeCycle;

namespace IntSchool.Sharp.Client.AuthorizationMethods;

public class SmsAuthorization(string areaCode, string phoneNumber) : IAuthorized
{
    public string Token { get; set; }
    public readonly string PhoneNumber = phoneNumber;
    public readonly string AreaCode = areaCode;
    public static SmsAuthorization New(string areaCode, string phoneNumber)
        => new SmsAuthorization(areaCode, phoneNumber);

    public SmsAuthorization Login(string verificationCode, Action? onSuccess = null, Action<string>? onError = null)
    {
        var result = Api.Instance.LoginByVerifySms(
            phoneNumber:PhoneNumber,
            areaCode:AreaCode,
            verificationCode:verificationCode);
        if (result.IsSuccess)
        {
            onSuccess?.Invoke();
            Token = result.SuccessResult!.Token;
        }
        else
            onError?.Invoke(result.IsErrorNotMappable == true ? "Not Mappable Error": (string)result.ErrorResult!.Message);

        return this;
    }
    public string GetToken() => Token;
}