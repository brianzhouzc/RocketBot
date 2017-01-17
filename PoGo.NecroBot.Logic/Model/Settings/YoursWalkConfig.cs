using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PoGo.NecroBot.Logic.Model.Settings
{
    [JsonObject(Title = "YoursWalk Config", Description = "Set your yourswalk settings.", ItemRequired = Required.DisallowNull)]
    public class YoursWalkConfig  : BaseConfig
    {
        internal enum YoursWalkTravelModes
        {
            motorcar,
            hgv,
            goods,
            psv,
            bicycle,
            cycleroute,
            foot,
            moped,
            mofa
        }

        public YoursWalkConfig() : base()
        {
        }

        [ExcelConfig(Description = "Use your walk api to resolve path for bot moving", Position = 1)]
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        public bool UseYoursWalk { get; set; }

        [ExcelConfig(Description = "Set heuricstic for moving: motorcar, bicycle, foot..", Position = 2)]
        [DefaultValue("bicycle")]
        [EnumDataType(typeof(YoursWalkTravelModes))]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 2)]
        public string YoursWalkHeuristic { get; set; }
    }
}