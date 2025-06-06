using IntSchool.Sharp.Core.Models;

namespace IntSchool.Sharp.Core.Data.EventArguments;

public sealed class RemoteErrorEventArgs(
    DateTime timestamp,
    ErrorResponseModel raw,
    string xToken):EventArgs
{
    public readonly ErrorResponseModel? Raw = raw;
    public readonly DateTime Timestamp = timestamp;
    public readonly string? XToken = xToken;
}