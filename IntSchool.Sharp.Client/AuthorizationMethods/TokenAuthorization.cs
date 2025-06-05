using IntSchool.Sharp.Client.Interfaces;

namespace IntSchool.Sharp.Client.AuthorizationMethods;

public class TokenAuthorization(string token) : IAuthorized
{
    public string Token { get; set; } = token;
    public string GetToken() => Token;
}