using System.ComponentModel;
using Newtonsoft.Json;

namespace PoGo.NecroBot.Logic.Model.Settings
{
    [JsonObject(Title = "Update Config", Description = "Set your update settings.", ItemRequired = Required.DisallowNull)]
    public class UpdateConfig
    {
        public const int CURRENT_SCHEMA_VERSION = 5;

        [DefaultValue(CURRENT_SCHEMA_VERSION)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        [ExcelConfig(Description ="Allow bot automatically checking for latest version, it will display message on console.", Position =1) ]
        public int SchemaVersion = CURRENT_SCHEMA_VERSION;

        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 2)]
        
        [ExcelConfig(Description = "Allow bot automatically update latest version", Position = 2)]
        public bool CheckForUpdates = true;

        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 3)]
        [ExcelConfig(Description = "Transfer existing config when bot update", Position = 3)]
        public bool AutoUpdate = true;
    }
}