using IntSchool.Sharp.Core.Data;

namespace IntSchool.Sharp.Core.Extensions.Converters;

public static class AttendanceOptionsConverter
{
    private static readonly Dictionary<string, AttendanceOptions> _map =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["weekendHoliday"] = AttendanceOptions.WeekendHoliday,
            ["intime"] = AttendanceOptions.InTime,
            ["late"] = AttendanceOptions.Late,
            ["illness"] = AttendanceOptions.Illness,
            ["personal"] = AttendanceOptions.Personal,
            ["absent"] = AttendanceOptions.Absent,
            ["noRecords"] = AttendanceOptions.NoRecord,
        };

    public static AttendanceOptions ToAttendanceOption(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return AttendanceOptions.NoRecord;

        return _map.GetValueOrDefault(value.Trim(), AttendanceOptions.NoRecord);
    }
}