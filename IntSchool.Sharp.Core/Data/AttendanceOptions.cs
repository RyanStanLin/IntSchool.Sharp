namespace IntSchool.Sharp.Core.Data;

public enum AttendanceOptions
{
    NoRecord,
    WeekendHoliday,
    InTime,
    Late,
    Illness,
    Personal,
    Absent
}

public static class AttendanceOptionsMetadata
{
    private static readonly Dictionary<AttendanceOptions, (string Description, int Priority)> Metadata = new()
    {
        { AttendanceOptions.NoRecord, ("No Record", -1) },
        { AttendanceOptions.WeekendHoliday, ("Weekend Holiday", 0) },
        { AttendanceOptions.InTime, ("In Time", 0) },
        { AttendanceOptions.Late, ("Late", 1) },
        { AttendanceOptions.Illness, ("Illness", 0) },
        { AttendanceOptions.Personal, ("Personal Leave", 0) },
        { AttendanceOptions.Absent, ("Absent", 2) }
    };

    public static string GetDescription(this AttendanceOptions option)
    {
        return Metadata.TryGetValue(option, out var data) ? data.Description : option.ToString();
    }

    public static int GetPriority(this AttendanceOptions option)
    {
        return Metadata.TryGetValue(option, out var data) ? data.Priority : int.MaxValue;
    }
}