using IntSchool.Sharp.Client.Interfaces;

namespace IntSchool.Sharp.Client;

public class Client
{
    public Client()
    {
        
    }

    public Client Login<TAuth>(TAuth auth) where TAuth : IAuthorized
    {
        
        return this;
    }
}