using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PoGo.NecroBot.Logic.Model.Settings
{
    [JsonObject(Title = "Snipe Config", Description = "Set your snipe settings.", ItemRequired = Required.DisallowNull)]
    public class SnipeConfig
    {
        [ExcelConfig (Description = "Tell bot to use location service, detail at  - https://github.com/5andr0/PogoLocationFeeder", Position =1)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        public bool UseSnipeLocationServer;

        [ExcelConfig(Description = "IP Address or server name of location server, ussually that is localhost", Position = 2)]
        [DefaultValue("localhost")]
        [MinLength(0)]
        [MaxLength(32)]
        //[RegularExpression(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$")] //Ip Only
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 2)]
        public string SnipeLocationServer = "localhost";

        [ExcelConfig(Description = "Port number of location server. ", Position = 3)]
        [DefaultValue(16969)]
        [Range(1, 65535)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 3)]
        public int SnipeLocationServerPort = 16969;

        [ExcelConfig(Description = "Tell bot to connect to PokeZZ to get data - nolonger work. ", Position = 4)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        public bool GetSniperInfoFromPokezz;

        [ExcelConfig(Description = "Tell bot to get only verified pokemon from PokeZZ  for snipe - nolonger work. ", Position = 5)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 5)]
        public bool GetOnlyVerifiedSniperInfoFromPokezz = true;

        [ExcelConfig(Description = "Tell bot to connect to pokesnipers.com to get sniper data - nolonger work. ", Position = 6)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 6)]
        public bool GetSniperInfoFromPokeSnipers;

        [ExcelConfig(Description = "Tell bot to connect to pokewatchers.com to get sniper data - nolonger work. ", Position = 7)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 7)]
        public bool GetSniperInfoFromPokeWatchers;

        [ExcelConfig(Description = "Tell bot to connect to snippedlagged.com to get sniper data - nolonger work. ", Position = 8)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 8)]
        public bool GetSniperInfoFromSkiplagged;

        [ExcelConfig(Description = "Number of ball in inventory to get sniper function work. ", Position = 9)]
        [DefaultValue(20)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 9)]
        public int MinPokeballsToSnipe = 20;

        [ExcelConfig(Description = "Min ball allow to exist sniper", Position = 10)]
        [DefaultValue(0)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 10)]
        public int MinPokeballsWhileSnipe = 0;

        [ExcelConfig(Description = "Delay time between 2 snipe. ", Position = 11)]
        [DefaultValue(60000)]
        [Range(0, 999999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 11)]
        public int MinDelayBetweenSnipes = 60000;

        [ExcelConfig(Description = "The area bot try to scan for target pokemon. ", Position = 12)]
        [DefaultValue(0.005)]
        [Range(0, 1)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 12)]
        public double SnipingScanOffset = 0.005;

        [ExcelConfig(Description = "That setting will make bot snipe when he reach every pokestop.", Position = 13)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 13)]
        public bool SnipeAtPokestops;

        [DefaultValue(false)]
        [ExcelConfig(Description = "Turn it on to ignore that pokemon with unknow IV from data source", Position = 14)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 14)]
        public bool SnipeIgnoreUnknownIv;

        [ExcelConfig(Description = "Bot will transfer pokemon for snipe if the snipping pokemon higher IV . ", Position = 15)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 15)]
        public bool UseTransferIvForSnipe;

        [ExcelConfig(Description = "Turn this on it mean bot only priority to snipe pokemon not in pokedex.", Position = 16)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 16)]
        public bool SnipePokemonNotInPokedex;

        /*SnipeLimit*/
        [ExcelConfig(Description = "Turn on to limit the speed for sniping. ", Position = 17)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 17)]
        public bool UseSnipeLimit = true;

        [ExcelConfig(Description = "Delay time between 2 snipes ", Position = 18)]
        [DefaultValue(10 * 60)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 18)]
        public int SnipeRestSeconds = 10 * 60;

        [ExcelConfig(Description = "Limit number of snipe in hour. ", Position = 19)]
        [DefaultValue(39)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 19)]
        public int SnipeCountLimit = 39;

        [ExcelConfig(Description = "Allow MSniper feature with bot. ", Position = 20)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 20)]
        public bool ActivateMSniper = true;

        [ExcelConfig(Description = "Min IV that bot will automatically snipe pokemon", Position = 21)]
        [DefaultValue(95)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 21)]
        public int MinIVForAutoSnipe;
    }
}