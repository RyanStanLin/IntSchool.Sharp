using IntSchool.Sharp.Client.Interfaces;

namespace IntSchool.Sharp.Client.AuthorizationMethods;

public class SmsAuthorization(string areaCode, string phoneNumber) : ITokenRenewableAuthorizationMethod
{
    public string Token { get; set; }
    public readonly string PhoneNumber = phoneNumber;
    public readonly string AreaCode = areaCode;

    public void RenewToken()
    {
       
    }
}