using Newtonsoft.Json;

namespace IntSchool.Sharp.Models;

public partial class ClassPeriod
{
    [JsonProperty("classPeriodId")]
    public long ClassPeriodId { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("start")]
    public long Start { get; set; }

    [JsonProperty("end")]
    public long End { get; set; }

    [JsonProperty("sevenFive")]
    public bool SevenFive { get; set; }

    [JsonProperty("isFullPeriodArranged")]
    public bool IsFullPeriodArranged { get; set; }
}