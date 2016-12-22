using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PoGo.NecroBot.Logic.Model.Settings
{
    [JsonObject(Title = "Custom Catch Config", Description = "Set your custom catch settings.", ItemRequired = Required.DisallowNull)]
    public class CustomCatchConfig
    {
        [ExcelConfig (Description ="Allow bot simulate throw as human", Position =1)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        public bool EnableHumanizedThrows = true;

        [ExcelConfig(Description = "Allow bot throw  miss pokemon", Position = 2)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 2)]
        public bool EnableMissedThrows = true;

        [ExcelConfig(Description = "Set how many percent bot missed pokemon", Position = 3)]
        [DefaultValue(25)]
        [Range(0,100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 3)]
        public int ThrowMissPercentage = 25;

        [ExcelConfig(Description = "Set how many percent bot thown ball with nice hit", Position = 4)]
        [DefaultValue(40)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        public int NiceThrowChance = 40;

        [ExcelConfig(Description = "Set how many percent bot thown ball with greate hit", Position = 5)]
        [DefaultValue(30)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 5)]
        public int GreatThrowChance = 30;

        [ExcelConfig(Description = "Set how many percent bot thown ball with excellent hit", Position = 6)]
        [DefaultValue(10)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 6)]
        public int ExcellentThrowChance = 10;

        [ExcelConfig(Description = "Set how many percent bot thown ball with curve hit", Position = 7)]
        [DefaultValue(90)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 7)]
        public int CurveThrowChance = 90;

        [ExcelConfig(Description = "Force bot use greate throw if IV higher than this value", Position = 8)]
        [DefaultValue(90.00)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 8)]
        public double ForceGreatThrowOverIv = 90.00;

        [ExcelConfig(Description = "Force bot use excellent throw if IV higher than this value", Position = 9)]
        [DefaultValue(95.00)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 9)]
        public double ForceExcellentThrowOverIv = 95.00;

        [ExcelConfig(Description = "Force bot use greate throw if CP higher than this value", Position = 10)]
        [DefaultValue(1000)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 10)]
        public int ForceGreatThrowOverCp = 1000;

        [ExcelConfig(Description = "Force bot use excellent throw if IV higher than this value", Position = 11)]
        [DefaultValue(1500)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 11)]
        public int ForceExcellentThrowOverCp = 1500;

        [ExcelConfig(Description = "Allow bot use transfer filter to catch pokemon - ", Position = 12)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 12)]
        public bool UseTransferFilterToCatch = false;

    }
}