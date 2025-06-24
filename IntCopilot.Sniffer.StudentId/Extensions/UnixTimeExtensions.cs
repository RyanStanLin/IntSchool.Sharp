namespace IntCopilot.Sniffer.StudentId.Extensions;

// Converts a Unix timestamp in milliseconds to a DateOnly
public static class UnixTimeExtensions
{
    /// <summary>
    /// Convert Unix timestamp (milliseconds since 1970-01-01 UTC) to DateOnly.
    /// </summary>
    /// <param name="unixTimeMilliseconds">Unix timestamp in milliseconds.</param>
    /// <returns>DateOnly representing the UTC date part of the timestamp.</returns>
    public static DateOnly ToDateOnlyFromUnixMilliseconds(this long unixTimeMilliseconds)
    {
        // Define the Unix epoch (start of time)
        var unixEpoch = DateTimeOffset.UnixEpoch; // = 1970-01-01T00:00:00Z

        try
        {
            // Add milliseconds to epoch to get DateTimeOffset
            var dateTime = unixEpoch.AddMilliseconds(unixTimeMilliseconds);

            // Convert to DateOnly (UTC date)
            return DateOnly.FromDateTime(dateTime.UtcDateTime);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            throw new ArgumentOutOfRangeException($"Invalid Unix timestamp: {unixTimeMilliseconds}", ex);
        }
    }
}