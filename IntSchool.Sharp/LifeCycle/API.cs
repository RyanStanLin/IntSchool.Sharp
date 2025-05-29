using System.Net;
using IntSchool.Sharp.Data.EventArguments;
using IntSchool.Sharp.Models;
using Newtonsoft.Json;
using RestSharp;

namespace IntSchool.Sharp.LifeCycle;

public partial class API(string? xToken = null)
{
    private static readonly Lazy<API> _instanceHolder = new(() => new API());
    public static API Instance => _instanceHolder.Value;
    private readonly RestClient Client = new(Constants.IntSchoolRootUrl);
    public string? XToken { get; set; } = xToken;
    
    public event EventHandler<ContentMappingErrorEventArgs>? OnContentMappingError;
    public event EventHandler<RemoteErrorEventArgs>? OnRemoteError;
    
    private (bool isSuccess, ApiResult<TSuccess, TError>? apiResult) TryExecute<TSuccess, TError>(
        RestRequest request,
        Func<string, TSuccess> parseSuccess,
        Func<string, TError> parseError
    )
        where TError : class
    {
        try
        {
            var response = Client.Execute(request);

            if (response.StatusCode is not HttpStatusCode.OK)
            {
                try
                {
                    var error = parseError(response.Content!);
                    var result = ApiResult<TSuccess, TError>.Error(error);

                    OnRemoteError?.Invoke(this, new RemoteErrorEventArgs(
                        timestamp: (error as ErrorResponseModel).Timestamp.DateTime,
                        raw: error as ErrorResponseModel,
                        xToken: XToken
                    ));

                    return (false, result);
                }
                catch (JsonException ex)
                {
                    OnContentMappingError?.Invoke(this, new ContentMappingErrorEventArgs(
                        timestamp: DateTime.Now,
                        json: response.Content,
                        jsonException: ex
                    ));
                    return (false, null);
                }
            }

            var raw = parseSuccess(response.Content!);
            return (true, ApiResult<TSuccess, TError>.Success(raw));
        }
        catch (Exception ex) when (ex is not JsonException)
        {
            throw new Exception("Network error occurred", ex);
        }
    }
    
    private (bool isSuccess, ApiResult<TError>? apiResult) TryExecute<TError>(
        RestRequest request,
        Func<string, TError> parseError
    )
        where TError : class
    {
        try
        {
            var response = Client.Execute(request);

            if (response.StatusCode is not HttpStatusCode.OK)
            {
                try
                {
                    var error = parseError(response.Content!);
                    var result = ApiResult<TError>.Error(error);

                    OnRemoteError?.Invoke(this, new RemoteErrorEventArgs(
                        timestamp: (error as ErrorResponseModel).Timestamp.DateTime,
                        raw: error as ErrorResponseModel,
                        xToken: XToken
                    ));

                    return (false, result);
                }
                catch (JsonException ex)
                {
                    OnContentMappingError?.Invoke(this, new ContentMappingErrorEventArgs(
                        timestamp: DateTime.Now,
                        json: response.Content,
                        jsonException: ex
                    ));
                    return (false, null);
                }
            }

            return (true, ApiResult<TError>.Success());
        }
        catch (Exception ex) when (ex is not JsonException)
        {
            throw new Exception("Network error occurred", ex);
        }
    }
}