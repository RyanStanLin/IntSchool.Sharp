using System.Diagnostics.CodeAnalysis;
using System.Net;
using IntSchool.Sharp.Core.Data.EventArguments;
using IntSchool.Sharp.Core.Models;
using Newtonsoft.Json;
using RestSharp;

namespace IntSchool.Sharp.Core.LifeCycle;

public partial class Api(string? xToken = null)
{
    private static readonly Lazy<Api> _instanceHolder = new(() => new Api());
    public static Api Instance => _instanceHolder.Value;
    private readonly RestClient _client = new(Constants.IntSchoolRootUrl);
    public string? XToken { get; set; } = xToken;
    public event EventHandler<ContentMappingErrorEventArgs>? OnContentMappingError;
    public event EventHandler<RemoteErrorEventArgs>? OnRemoteError;
    
    private ApiResult<TSuccess, TError> TryExecute<TSuccess, TError>(
        RestRequest request,
        Func<string, TSuccess> parseSuccess,
        Func<string, TError> parseError
    )
        where TError : class
    {
        try
        {
            var response = _client.Execute(request);

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

                    return result;
                }
                catch (JsonException ex)
                {
                    OnContentMappingError?.Invoke(this, new ContentMappingErrorEventArgs(
                        timestamp: DateTime.Now,
                        json: response.Content,
                        jsonException: ex
                    ));
                    var result = ApiResult<TSuccess, TError>.ErrorNotMappable();
                    return result;
                }
            }

            var raw = parseSuccess(response.Content!);
            return ApiResult<TSuccess, TError>.Success(raw);
        }
        catch (Exception ex) when (ex is not JsonException)
        {
            throw new Exception("Network error occurred", ex);
        }
    }
    
    private ApiResult<TError> TryExecute<TError>(
        RestRequest request,
        Func<string, TError> parseError)
        where TError : class
    {
        try
        {
            var response = _client.Execute(request);

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

                    return result;
                }
                catch (JsonException ex)
                {
                    OnContentMappingError?.Invoke(this, new ContentMappingErrorEventArgs(
                        timestamp: DateTime.Now,
                        json: response.Content,
                        jsonException: ex
                    ));
                    var result = ApiResult<TError>.ErrorNotMappable();
                    return result;
                }
            }

            return ApiResult<TError>.Success();
        }
        catch (Exception ex) when (ex is not JsonException)
        {
            throw new Exception("Network error occurred", ex);
        }
    }
    
    private async Task<ApiResult<TSuccess, TError>> TryExecuteAsync<TSuccess, TError>(
        RestRequest request,
        Func<string, TSuccess> parseSuccess,
        Func<string, TError> parseError
    )
        where TError : class
    {
        try
        {
            var response = await _client.ExecuteAsync(request);

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

                    return result;
                }
                catch (JsonException ex)
                {
                    OnContentMappingError?.Invoke(this, new ContentMappingErrorEventArgs(
                        timestamp: DateTime.Now,
                        json: response.Content,
                        jsonException: ex
                    ));

                    return ApiResult<TSuccess, TError>.ErrorNotMappable();
                }
            }

            var raw = parseSuccess(response.Content!);
            return ApiResult<TSuccess, TError>.Success(raw);
        }
        catch (Exception ex) when (ex is not JsonException)
        {
            throw new Exception("Network error occurred", ex);
        }
    }

    private async Task<ApiResult<TError>> TryExecuteAsync<TError>(
        RestRequest request,
        Func<string, TError> parseError)
        where TError : class
    {
        try
        {
            var response = await _client.ExecuteAsync(request);

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

                    return result;
                }
                catch (JsonException ex)
                {
                    OnContentMappingError?.Invoke(this, new ContentMappingErrorEventArgs(
                        timestamp: DateTime.Now,
                        json: response.Content,
                        jsonException: ex
                    ));

                    return ApiResult<TError>.ErrorNotMappable();
                }
            }

            return ApiResult<TError>.Success();
        }
        catch (Exception ex) when (ex is not JsonException)
        {
            throw new Exception("Network error occurred", ex);
        }
    }
}