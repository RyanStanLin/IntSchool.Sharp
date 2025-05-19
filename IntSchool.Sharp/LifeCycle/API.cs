using IntSchool.Sharp.Data.EventArguments;
using RestSharp;

namespace IntSchool.Sharp.LifeCycle;

public partial class API(string? xToken = null)
{
    private static readonly Lazy<API> _instanceHolder = new(() => new API());
    public static API Instance => _instanceHolder.Value;
    private readonly RestClient Client = new(Constants.IntSchoolRootUrl);
    public string? XToken { get; set; } = xToken;
    
    public event EventHandler<FallbackJsonErrorEventArgs>? OnFallbackJsonError;
    public event EventHandler<UnauthorizedErrorEventArgs>? OnUnauthorizedError;
}