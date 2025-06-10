namespace IntCopilot.Barker.Client;

public class AttendanceWorkerSettings
{
    public int PollingIntervalSeconds { get; set; } = 30;
    public List<ProfileSetting> Profiles { get; set; } = new();

    public void Validate()
    {
        if (PollingIntervalSeconds <= 0)
            throw new InvalidOperationException("PollingIntervalSeconds must be positive");
        
        if (!Profiles.Any())
            throw new InvalidOperationException("At least one profile must be configured");
            
        foreach (var profile in Profiles)
        {
            profile.Validate();
        }
    }
}

public class ProfileSetting
{
    public required string Description { get; set; }
    public required string StudentId { get; set; }
    public required string SchoolYearId { get; set; }
    public required List<string> BarkTokens { get; set; } = new List<string>();
    public string TimeWindow { get; set; } = "Today";

    public void Validate()
    {
        if (string.IsNullOrEmpty(StudentId))
            throw new InvalidOperationException("StudentId is required");
        if (string.IsNullOrEmpty(SchoolYearId))
            throw new InvalidOperationException("SchoolYearId is required");
        if (string.IsNullOrEmpty(TimeWindow))
            throw new InvalidOperationException("TimeWindow is required");
        if(BarkTokens.Any() is not true)
            throw new InvalidOperationException("At least one bark token must be configured");
    }
}