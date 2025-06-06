// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using IntSchool.Sharp.Core.Models;
//
//    var getMarkDetailResponseModel = GetMarkDetailResponseModel.FromJson(jsonString);

namespace IntSchool.Sharp.Core.Models
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class GetMarkDetailResponseModel
    {
        [JsonProperty("taskId")]
        public long TaskId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("subjectName")]
        public string SubjectName { get; set; }

        [JsonProperty("courseName")]
        public string CourseName { get; set; }

        [JsonProperty("typeName")]
        public string TypeName { get; set; }

        [JsonProperty("scoreFlag")]
        public bool ScoreFlag { get; set; }

        [JsonProperty("topScore")]
        public long TopScore { get; set; }

        [JsonProperty("endDate")]
        public long EndDate { get; set; }

        [JsonProperty("inTotal")]
        public bool InTotal { get; set; }

        [JsonProperty("online")]
        public bool Online { get; set; }

        [JsonProperty("overDeadline")]
        public bool OverDeadline { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("taskStudentId")]
        public long TaskStudentId { get; set; }

        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("score")]
        public long Score { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("comments")]
        public string Comments { get; set; }

        [JsonProperty("resources")]
        public List<object> Resources { get; set; }

        [JsonProperty("studentResources")]
        public List<object> StudentResources { get; set; }
    }

    public partial class GetMarkDetailResponseModel
    {
        public static GetMarkDetailResponseModel FromJson(string json) => JsonConvert.DeserializeObject<GetMarkDetailResponseModel>(json, ConverterGetMarkDetailResponseModel.Settings);
    }

    public static class SerializeGetMarkDetailResponseModel
    {
        public static string ToJson(this GetMarkDetailResponseModel self) => JsonConvert.SerializeObject(self, ConverterGetMarkDetailResponseModel.Settings);
    }

    internal static class ConverterGetMarkDetailResponseModel
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
