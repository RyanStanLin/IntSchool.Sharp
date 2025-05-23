namespace IntSchool.Sharp.Data.EventArguments;

public sealed class UnauthorizedErrorEventArgs(
    DateTime timestamp,
    string xToken):EventArgs
{
    public readonly DateTime Timestamp = timestamp;
    public readonly string XToken = xToken;
}