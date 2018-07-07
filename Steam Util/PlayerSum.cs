namespace PlayerSum
{
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    /// <summary>
    /// Player sums.
    /// </summary>
    public partial class PlayerSums
    {
        [JsonProperty("response")]
        public Response Response { get; set; }
    }

    public partial class Response
    {
        [JsonProperty("players")]
        public Player[] Players { get; set; }
    }

    public partial class Player
    {
        [JsonProperty("steamid")]
        public string Steamid { get; set; }

        [JsonProperty("communityvisibilitystate")]
        public long Communityvisibilitystate { get; set; }

        [JsonProperty("profilestate")]
        public long Profilestate { get; set; }

        [JsonProperty("personaname")]
        public string Personaname { get; set; }

        [JsonProperty("lastlogoff")]
        public long Lastlogoff { get; set; }

        [JsonProperty("profileurl")]
        public string Profileurl { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("avatarmedium")]
        public string Avatarmedium { get; set; }

        [JsonProperty("avatarfull")]
        public string Avatarfull { get; set; }

        [JsonProperty("personastate")]
        public long Personastate { get; set; }

        [JsonProperty("realname")]
        public string Realname { get; set; }

        [JsonProperty("primaryclanid")]
        public string Primaryclanid { get; set; }

        [JsonProperty("timecreated")]
        public long Timecreated { get; set; }

        [JsonProperty("personastateflags")]
        public long Personastateflags { get; set; }
    }

    public partial class PlayerSums
    {
        public static PlayerSums FromJson(string json) => JsonConvert.DeserializeObject<PlayerSums>(json, PlayerSum.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this PlayerSums self) => JsonConvert.SerializeObject(self, PlayerSum.Converter.Settings);
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
