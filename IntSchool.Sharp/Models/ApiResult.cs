namespace IntSchool.Sharp.Models;

public class ApiResult<TSuccess, TError>
{
    public TSuccess? SuccessResult { get; private set; }
    public TError? ErrorResult { get; private set; }
    public bool IsSuccess { get; private set; }

    private ApiResult() { }

    public static ApiResult<TSuccess, TError> Success(TSuccess result)
        => new ApiResult<TSuccess, TError> { SuccessResult = result, IsSuccess = true };

    public static ApiResult<TSuccess, TError> Error(TError error)
        => new ApiResult<TSuccess, TError> { ErrorResult = error, IsSuccess = false };
}