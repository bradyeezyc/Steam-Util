using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BanJson
{
    public partial class GetBanInfo
    {
        [JsonProperty("players")]
        public PlayerBan[] Players { get; set; }
    }
    public partial class PlayerBan
    {
        [JsonProperty("SteamId")]
        public string SteamId { get; set; }

        [JsonProperty("CommunityBanned")]
        public bool CommunityBanned { get; set; }

        [JsonProperty("VACBanned")]
        public bool VacBanned { get; set; }

        [JsonProperty("NumberOfVACBans")]
        public long NumberOfVacBans { get; set; }

        [JsonProperty("DaysSinceLastBan")]
        public long DaysSinceLastBan { get; set; }

        [JsonProperty("NumberOfGameBans")]
        public long NumberOfGameBans { get; set; }

        [JsonProperty("EconomyBan")]
        public string EconomyBan { get; set; }
    }

    public partial class GetBanInfo
    {
        public static Welcome FromJson(string json) => JsonConvert.DeserializeObject<Welcome>(json, BanJson.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Welcome self) => JsonConvert.SerializeObject(self, BanJson.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
