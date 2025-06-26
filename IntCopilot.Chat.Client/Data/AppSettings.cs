namespace IntCopilot.Chat.Client.Data;

public class AppSettings
{
    public required string IntSchoolToken { get; set; }
    public required string GeminiToken { get; set; }
    public required string DBHost { get; set; }
    public required int DBPort { get; set; }
    public required string DBUser { get; set; }
    public required string DBPassword { get; set; }
    public required string DBName { get; set; }
    public required string GeminiModel { get; set; }
    public required string ModelSystemInstruction { get; set; }
}