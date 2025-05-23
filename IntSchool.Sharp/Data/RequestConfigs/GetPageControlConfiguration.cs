namespace IntSchool.Sharp.RequestConfigs;

public class GetPageControlConfiguration(string pageSize, string pageCurrent)
{
    public string PageSize { get; set; } = pageSize;
    public string PageCurrent { get; set; } = pageCurrent;
}