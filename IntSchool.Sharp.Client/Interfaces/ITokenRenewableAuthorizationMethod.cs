namespace IntSchool.Sharp.Client.Interfaces;

public interface ITokenRenewableAuthorizationMethod : IAuthorizationMethod
{
    public void RenewToken();
}