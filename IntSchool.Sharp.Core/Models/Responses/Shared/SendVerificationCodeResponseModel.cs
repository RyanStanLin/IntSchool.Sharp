// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using IntSchool.Sharp.Core.Models;
//
//    var sendSmsResponseModel = SendVerificationCodeResponseModel.FromJson(jsonString);

namespace IntSchool.Sharp.Core.Models
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class SendVerificationCodeResponseModel
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("resCode")]
        public object ResCode { get; set; }

        [JsonProperty("resMsg")]
        public string ResMsg { get; set; }
    }

    public partial class SendVerificationCodeResponseModel
    {
        public static SendVerificationCodeResponseModel FromJson(string json) => JsonConvert.DeserializeObject<SendVerificationCodeResponseModel>(json, ConverterSendVerificationCodeResponseModel.Settings);
    }

    public static class SerializeSendVerificationCodeResponseModel
    {
        public static string ToJson(this SendVerificationCodeResponseModel self) => JsonConvert.SerializeObject(self, ConverterSendVerificationCodeResponseModel.Settings);
    }

    internal static class ConverterSendVerificationCodeResponseModel
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