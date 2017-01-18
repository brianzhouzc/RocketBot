using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PoGo.NecroBot.Logic.Model.Settings
{
    [JsonObject(Title = "Player Config", Description = "Set your player settings.", ItemRequired = Required.DisallowNull)]
    public class PlayerConfig  :BaseConfig
    {
        public PlayerConfig() : base()
        {
        }

        [Range(0, 999999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        public int DelayBetweenPlayerActions = 4000;

        [ExcelConfig(Description = "Set delay time for evolve action", Position = 2)]
        [DefaultValue(20000)]
        [Range(0, 999999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 2)]
        public int EvolveActionDelay = 20000;

        [ExcelConfig(Description = "Set delay time for transfer action", Position = 3)]
        [DefaultValue(5000)]
        [Range(0, 999999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 3)]
        public int TransferActionDelay = 5000;

        [ExcelConfig(Description = "Set delay time for recycle item", Position = 4)]
        [DefaultValue(1000)]
        [Range(0, 999999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        public int RecycleActionDelay = 1000;

        [ExcelConfig(Description = "Set delay time for rename pokemon action", Position = 5)]
        [DefaultValue(2000)]
        [Range(0, 999999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 5)]
        public int RenamePokemonActionDelay = 2000;

        [ExcelConfig(Description = "Set delay time for random action", Position = 6)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 6)]
        public bool UseNearActionRandom { get; set; }
    }
}