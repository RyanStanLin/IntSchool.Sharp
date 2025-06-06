// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using IntSchool.Sharp.Core.Models;
//
//    var getCurrentSchoolYearResponseModel = GetCurrentSchoolYearResponseModel.FromJson(jsonString);

namespace IntSchool.Sharp.Core.Models
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class GetCurrentSchoolYearResponseModel
    {
        [JsonProperty("schoolYearId")]
        public long SchoolYearId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("startTime")]
        public long StartTime { get; set; }

        [JsonProperty("endTime")]
        public long EndTime { get; set; }
    }

    public partial class GetCurrentSchoolYearResponseModel
    {
        public static GetCurrentSchoolYearResponseModel FromJson(string json) => JsonConvert.DeserializeObject<GetCurrentSchoolYearResponseModel>(json, ConverterGetCurrentSchoolYearResponseModel.Settings);
    }

    public static class SerializeGetCurrentSchoolYearResponseModel
    {
        public static string ToJson(this GetCurrentSchoolYearResponseModel self) => JsonConvert.SerializeObject(self, ConverterGetCurrentSchoolYearResponseModel.Settings);
    }

    internal static class ConverterGetCurrentSchoolYearResponseModel
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
