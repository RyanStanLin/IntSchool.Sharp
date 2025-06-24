namespace IntCopilot.Chat.Client.Data;

public class AppSettings
{
    public string IntSchoolToken { get; set; }
    public string GeminiToken { get; set; }
    public string DBHost { get; set; }
    public int DBPort { get; set; }
    public string DBUser { get; set; }
    public string DBPassword { get; set; }
    public string DBName { get; set; }
    public string GeminiModel { get; set; }
    public string ModelSystemInstruction { get; set; }
}