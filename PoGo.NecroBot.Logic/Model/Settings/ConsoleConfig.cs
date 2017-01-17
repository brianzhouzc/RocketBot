using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PoGo.NecroBot.Logic.Model.Settings
{
    [JsonObject(
         Title = "Console Config",
         Description = "Set your console settings.",
         ItemRequired = Required.DisallowNull
     )]
    public class ConsoleConfig : BaseConfig
    {
        public ConsoleConfig() : base()
        {
        }

        [DefaultValue("en")]
        [RegularExpression(@"^[a-zA-Z]{2}(-[a-zA-Z]{2})*$")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        [ExcelConfig(SheetName = "ConsoleConfig", Position = 1, Description = "Language transation code")]
        public string TranslationLanguageCode { get; set; }

        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 2)]
        [ExcelConfig(Position = 2, Description = "Yes - Will display start up wellcome / No will turn off")]
        public bool StartupWelcomeDelay { get; set; }

        [DefaultValue(2)]
        [Range(0, 100)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 3)]
        [ExcelConfig(Position = 3, Description = "Amount Of Pokemon To Display On Start")]
        public int AmountOfPokemonToDisplayOnStart { get; set; }

        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        [ExcelConfig(Position = 4, Description = "Detailed Counts Before Recycling")]
        public bool DetailedCountsBeforeRecycling { get; set; }
    }
}