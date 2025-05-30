namespace IntSchool.Sharp.Core.RequestConfigs;

public class PostLeaveConfiguration(
    DateTime startTime,
    DateTime endTime,
    string message,
    LeaveOptions reason,
    LeaveTypes type,
    string studentId)
{
    public DateTime StartTime { get; set; } = startTime;
    public DateTime EndTime { get; set; } = endTime;
    public string Message { get; set; } = message;
    public LeaveOptions Reason { get; set; } = reason;
    public LeaveTypes Type { get; set; } = type;
    public string StudentId { get; set; } = studentId;
}