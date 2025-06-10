namespace IntSchool.Sharp.Core.Extensions;

/// <summary>
/// Extension methods for converting DateTime to Unix timestamp in milliseconds.
/// </summary>
public static class DateTimeTimestampExtension
{
    /// <summary>
    /// Converts a DateTime to Unix timestamp in milliseconds.
    /// </summary>
    /// <param name="dateTime">The DateTime value to convert.</param>
    /// <returns>Unix timestamp in milliseconds (since 1970-01-01 UTC).</returns>
    public static long ToUnixTimestampMilliseconds(this DateTime dateTime)
    {
        return new DateTimeOffset(dateTime.ToUniversalTime()).ToUnixTimeMilliseconds();
    }
    
    public static DateTime ToDateTimeFromUnixMilliseconds(this long timestamp)
    {
        try
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).LocalDateTime;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            throw new ArgumentOutOfRangeException($"Invalid Unix timestamp: {timestamp}", ex);
        }
    }
}