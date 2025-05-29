using Newtonsoft.Json;

namespace IntSchool.Sharp.Data.EventArguments;

public sealed class ContentMappingErrorEventArgs(
    DateTime timestamp,
    string? json,
    JsonException jsonException): EventArgs
{
    public readonly DateTime Timestamp = timestamp;
    public readonly string? Json = json;
    public readonly JsonException JsonException = jsonException;
}