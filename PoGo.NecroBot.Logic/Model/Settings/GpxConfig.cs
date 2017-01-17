using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PoGo.NecroBot.Logic.Model.Settings
{
    [JsonObject(Title = "Gpx Config", Description = "Set your Gpx settings.", ItemRequired = Required.DisallowNull)]
    public class GpxConfig : BaseConfig
    {
        public GpxConfig() : base()
        {
        }

        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        [ExcelConfig(Description = "If this option turn on, bot will walk using the predifined path in GpxFile", Position = 1)]
        public bool UseGpxPathing { get; set; }

        [DefaultValue("GPXPath.GPX")]
        [MinLength(0)]
        [MaxLength(255)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
        [ExcelConfig(Description = "The GPX file name or full path. if you not enter full path, you have to copy the file into bot directory", Position = 2)]
        public string GpxFile { get; set; }
    }
}