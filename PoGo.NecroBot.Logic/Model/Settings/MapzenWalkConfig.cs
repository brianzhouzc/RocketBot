using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PoGo.NecroBot.Logic.Model.Settings
{
    [JsonObject(Title = "MapzenWalk Config", Description = "Set your mapzenwalk settings.", ItemRequired = Required.DisallowNull)]
    public class MapzenWalkConfig    : BaseConfig
    {
        public MapzenWalkConfig() :base () { }

        internal enum MapzenWalkTravelModes
        {
            auto,
            bicycle,
            pedestrian
        }

        [ExcelConfig (Description = "Allow bot using Mapzen api to resolve path", Position = 1)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        public bool UseMapzenWalk { get; set; }

        [ExcelConfig(Description = "The API key to connect to mapzen", Position = 2)]
        [DefaultValue(null)]
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Populate, Order = 2)]
        public string MapzenTurnByTurnApiKey { get; set; }

        [ExcelConfig(Description = "Set the heuristic to find the rout", Position = 3)]
        [DefaultValue("bicycle")]
        [EnumDataType(typeof(MapzenWalkTravelModes))]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 3)]
        public string MapzenWalkHeuristic { get; set; }

        [ExcelConfig(Description = "Set Mapzen elevation api key", Position = 4)]
        [DefaultValue(null)]
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        public string MapzenElevationApiKey { get; set; }
    }
}