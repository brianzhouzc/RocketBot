using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PoGo.NecroBot.Logic.Model.Settings
{
    [JsonObject(Title = "Poke Stop Config", Description = "Set your poke stop settings.", ItemRequired = Required.DisallowNull)]
    public class PokeStopConfig : BaseConfig
    {
        public PokeStopConfig() : base () { }

        [ExcelConfig (Description = "Allow check for pokestop daily limit - 2000 per 24h", Position =1)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        public bool UsePokeStopLimit { get; set; }

        [ExcelConfig(Description = "Max number of pokestop bot allowed to farm a day", Position = 2)]
        [DefaultValue(1998)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 2)]
        public int PokeStopLimit {get; set; }

        [ExcelConfig(Description = "Time duration apply for the limit above in minutes", Position = 3)]
        [DefaultValue(60 * 24 + 30)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 3)]
        public int PokeStopLimitMinutes { get; set; }
    }
}
