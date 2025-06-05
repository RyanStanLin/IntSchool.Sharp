namespace IntSchool.Sharp.Client.Interfaces;

public interface ITokenRenewableAuthorized : IAuthorized
{
    public void RenewToken();
}