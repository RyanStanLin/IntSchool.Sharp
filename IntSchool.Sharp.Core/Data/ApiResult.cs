using System.Diagnostics.CodeAnalysis;

namespace IntSchool.Sharp.Core.Models;

public class ApiResult<TSuccess, TError>
{
    public TSuccess? SuccessResult { get; private set; }
    public TError? ErrorResult { get; private set; }
    public bool IsSuccess { get; private set; } = false;
    public bool IsErrorNotMappable { get; private set; } = true;

    private ApiResult() { }

    [MemberNotNull(nameof(SuccessResult))]
    public static ApiResult<TSuccess, TError> Success(TSuccess result)
        => new() { SuccessResult = result, IsSuccess = true, IsErrorNotMappable = false };

    [MemberNotNull(nameof(ErrorResult))]
    public static ApiResult<TSuccess, TError> Error(TError error)
        => new () { ErrorResult = error, IsSuccess = false, IsErrorNotMappable = false };
    
    public static ApiResult<TSuccess, TError> ErrorNotMappable() => new ();
}

public class ApiResult<TError>
{
    public TError? ErrorResult { get; private set; }
    public bool IsSuccess { get; private set; } = false;
    public bool IsErrorNotMappable { get; private set; } = true;

    private ApiResult() { }

    public static ApiResult<TError> Success()
        => new ApiResult<TError> { IsSuccess = true, IsErrorNotMappable = false };

    [MemberNotNull(nameof(ErrorResult))]
    public static ApiResult <TError> Error(TError error)
        => new ApiResult <TError> { ErrorResult = error, IsSuccess = false, IsErrorNotMappable = false };
    
    public static ApiResult<TError> ErrorNotMappable() => new ();
}