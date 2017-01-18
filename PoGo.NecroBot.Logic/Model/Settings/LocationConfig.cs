using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PoGo.NecroBot.Logic.Model.Settings
{
    [JsonObject(Title = "Location Config", Description = "Set your location settings.", ItemRequired = Required.DisallowNull)]
    public class LocationConfig : BaseConfig
    {
        public LocationConfig() : base()
        {
        }

        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        [ExcelConfig(Description = "When you set this to true, Bot will use teleport instead of walking. this is not recommended.", Position = 1)]
        public bool DisableHumanWalking { get; set; }

        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 2)]
        [ExcelConfig(Description = "When set to true, bot will start from the last known location instead of default location", Position = 2)]
        public bool StartFromLastPosition { get; set; }

        [DefaultValue(40.785092)]
        [Range(-90, 90)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 3)]
        [ExcelConfig(Description = "Default Latitude that bot will start with.", Position = 3)]
        public double DefaultLatitude { get; set; }

        [DefaultValue(-73.968286)]
        [Range(-180, 180)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        [ExcelConfig(Description = "Default Longitude that bot will start with.", Position = 4)]
        public double DefaultLongitude { get; set; }

        [DefaultValue(4.16)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 5)]
        [ExcelConfig(Description = "The walking speed apply for bot to move between pokestops.", Position = 5)]
        public double WalkingSpeedInKilometerPerHour { get; set; }

        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 6)]
        [ExcelConfig(Description = "Turn this option on to add random speed change into WalkingSpeedInKilometerPerHour.", Position = 6)]
        public bool UseWalkingSpeedVariant { get; set; }

        [DefaultValue(1.2)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 7)]
        [ExcelConfig(Description = "The randome speed add/minus into walking speed when UseWalkingSpeedVariant set to true", Position = 7)]
        public double WalkingSpeedVariant { get; set; }

        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 8)]
        [ExcelConfig(Description = "Display variant speed change in console window ", Position = 8)]
        public bool ShowVariantWalking { get; set; }

        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 9)]
        [ExcelConfig(Description = "Turn on this option will make bot stop at pokestop randomly. make him more humanlike", Position = 9)]
        public bool RandomlyPauseAtStops { get; set; }

        [DefaultValue(10)]
        [Range(0, 999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 10)]
        [ExcelConfig(Description = "This value to set random offset change when bot start from defautl location", Position = 10)]
        public int MaxSpawnLocationOffset { get; set; }

        [DefaultValue(1000)]
        [Range(0, 9999)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 11)]
        [ExcelConfig(Description = "The radius distance we want bot to travel from defautl location. Notice that this will be change depend on other config bot may walk out of that radius", Position = 11)]
        public int MaxTravelDistanceInMeters { get; set; }

        [JsonIgnore]
        public int ResumeTrack = 0;
        [JsonIgnore]
        public int ResumeTrackSeg = 0;
        [JsonIgnore]
        public int ResumeTrackPt = 0;
    }
}