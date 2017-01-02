using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PoGo.NecroBot.Logic.Model.Settings
{
    [JsonObject(Title = "Recycle Config", Description = "Set your recycle settings.", ItemRequired = Required.DisallowNull)]
    public class ItemRecycleConfig  : BaseConfig
    {
        public ItemRecycleConfig() : base () { }

        [ExcelConfig (Description ="Allow bot display list of item to be recycled", Position =1)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        public bool VerboseRecycling { get; set; }

        [ExcelConfig(Description = "Specify percentace of inventory full to start clean up", Position = 2)]
        [DefaultValue(90)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 2)]
        public double RecycleInventoryAtUsagePercentage { get; set; }

        [ExcelConfig(Description = "Turn on randomize recycle items", Position = 3)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 3)]
        public bool RandomizeRecycle;

        [ExcelConfig(Description = "Number of randomize item to be recycled", Position = 4)]
        [DefaultValue(5)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        public int RandomRecycleValue { get; set; }

        /*Amounts*/
        [ExcelConfig(Description = "How many ball (nomal, greate, ultra) to be kept ", Position = 5)]
        [DefaultValue(120)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 6)]
        public int TotalAmountOfPokeballsToKeep { get; set; }


        [ExcelConfig(Description = "How many portion item (nomal, hyber, ultra, max) too be kept ", Position = 6)]
        [DefaultValue(80)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 7)]
        public int TotalAmountOfPotionsToKeep { get; set; }

        [ExcelConfig(Description = "How many revise item (nomal, max) too be kept ", Position = 7)]
        [DefaultValue(60)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 8)]
        public int TotalAmountOfRevivesToKeep { get; set; }

        [ExcelConfig(Description = "How many berries item  too be kept ", Position = 8)]
        [DefaultValue(50)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 9)]
        public int TotalAmountOfBerriesToKeep { get; set; }

        [ExcelConfig(Description = "Max pokeball tobe keep - redudant need to cleanup ", Position = 9)]
        [Range(0, 999)]
        [DefaultValue(50)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 9)]
        public int MaxPokeballsToKeep { get; set; }
    }
}