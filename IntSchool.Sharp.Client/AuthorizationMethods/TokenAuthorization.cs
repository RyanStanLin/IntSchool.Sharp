using IntSchool.Sharp.Client.Interfaces;

namespace IntSchool.Sharp.Client.AuthorizationMethods;

public class TokenAuthorization(string token) : IAuthorizationMethod
{
    public string Token { get; set; } = token;
}