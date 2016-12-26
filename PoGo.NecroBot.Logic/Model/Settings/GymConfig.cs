using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PoGo.NecroBot.Logic.Model.Settings
{
    [JsonObject(Title = "Gym Config", Description = "This config to setup rules for bot doing gym", ItemRequired = Required.DisallowNull)]
    public class GymConfig
    {
        internal enum TeamColor
        {
            Yellow,
            Red,
            Blue
        }

        [ExcelConfig(Description = "Allow bot go to gym for training, defense or battle with other team.", Position = 1)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool Enable = false;

        [ExcelConfig(Description = "Turn this on bot will select gym go go instead of poekstop if gym distance is valid", Position = 2)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool PrioritizeGymOverPokestop = true;

        [ExcelConfig(Description = "Max distance bot allow wak to gym.", Position = 3)]
        [DefaultValue(500.0)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public double MaxDistance = 1500.0;

        [ExcelConfig(Description = "Default team color bot will join.", Position = 4)]
        [DefaultValue("Yellow")]
        [EnumDataType(typeof(TeamColor))]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public string DefaultTeam = "Yellow";

        [ExcelConfig(Description = "Max CP that pokemmon will be select for defense gym.", Position = 5)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public int MaxCPToDeploy = 1000;

        [ExcelConfig(Description = "Max level of pokemon can be put in gym.", Position = 6)]
        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public int MaxLevelToDeploy = 16;

        [ExcelConfig(Description = "Time in minute to visit come back and check gym, depend on distance setting", Position = 7)]
        [DefaultValue(60)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public int VisitTimeout = 60;

        [ExcelConfig(Description = "Use randome pokemon for gym.", Position = 8)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool UseRandomPokemon;

        [ExcelConfig(Description = "Top N pokemon by CP to exluse from pokemon deploy.", Position = 9)]
        [DefaultValue(10)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public int NumberOfTopPokemonToBeExcluded = 10;

        [ExcelConfig(Description = "Collect coin after N gym deployed", Position = 10)]
        [DefaultValue(1)]
        [Range(0, 10)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        public int CollectCoinAfterDeployed = 0;
    }
}
