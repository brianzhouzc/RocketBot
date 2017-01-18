using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PoGo.NecroBot.Logic.Model.Settings
{
    [JsonObject(
         Title = "Device Config",
         Description = "Set your device settings (set \"DevicePackageName\" to \"random\" for auto-generated device). Set \"DevicePlatform\" to \"android\" or \"ios\".",
         ItemRequired = Required.DisallowNull
     )]
    public class DeviceConfig
    {
        internal enum DevicePlatformType
        {
            android,
            ios
        }

        [DefaultValue("android")]
        [EnumDataType(typeof(DevicePlatformType))]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        public string DevicePlatform = "android";

        [DefaultValue("random")]
        [MinLength(0)]
        [MaxLength(32)]
        [RegularExpression(@"[a-zA-Z0-9_\-\.\s]")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 2)]
        public string DevicePackageName = "random";

        [DefaultValue("8525f5d8201f78b5")]
        [MinLength(12)]
        [MaxLength(40)]
        [RegularExpression(@"^[0-9A-Fa-f]+$")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 3)]
        public string DeviceId = "8525f5d8201f78b5";

        [DefaultValue(null)]
        [MinLength(0)]
        [MaxLength(32)]
        [RegularExpression(@"[a-zA-Z0-9_\-\.\s]")]
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        public string AndroidBoardName;

        [DefaultValue(null)]
        [MinLength(0)]
        [MaxLength(32)]
        [RegularExpression(@"[a-zA-Z0-9_\-\.\s]")]
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Populate, Order = 5)]
        public string AndroidBootloader;

        [DefaultValue("HTC")]
        [MinLength(0)]
        [MaxLength(32)]
        [RegularExpression(@"[a-zA-Z0-9_\-\.\s]")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 6)]
        public string DeviceBrand = "HTC";

        [DefaultValue("HTC 10")]
        [MinLength(0)]
        [MaxLength(32)]
        [RegularExpression(@"[a-zA-Z0-9_\-\.\s]")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 7)]
        public string DeviceModel = "HTC 10";

        [DefaultValue(null)]
        [MinLength(0)]
        [MaxLength(32)]
        [RegularExpression(@"[a-zA-Z0-9_\-\.\s]")]
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Populate, Order = 8)]
        public string DeviceModelIdentifier;

        [DefaultValue("qcom")]
        [MinLength(0)]
        [MaxLength(32)]
        [RegularExpression(@"[a-zA-Z0-9_\-\.\s]")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 9)]
        public string DeviceModelBoot = "qcom";

        [DefaultValue("HTC")]
        [MinLength(0)]
        [MaxLength(32)]
        [RegularExpression(@"[a-zA-Z0-9_\-\.\s]")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 10)]
        public string HardwareManufacturer = "HTC";

        [DefaultValue("HTC 10")]
        [MinLength(0)]
        [MaxLength(32)]
        [RegularExpression(@"[a-zA-Z0-9_\-\.\s]")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 11)]
        public string HardwareModel = "HTC 10";

        [DefaultValue("pmewl_00531")]
        [MinLength(0)]
        [MaxLength(32)]
        [RegularExpression(@"[a-zA-Z0-9_\-\.\s]")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 12)]
        public string FirmwareBrand = "pmewl_00531";

        [DefaultValue(null)]
        [MinLength(0)]
        [MaxLength(32)]
        [RegularExpression(@"[a-zA-Z0-9_\-\.\s]")]
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Populate, Order = 13)]
        public string FirmwareTags;

        [DefaultValue("user")]
        [MinLength(0)]
        [MaxLength(32)]
        [RegularExpression(@"[a-zA-Z0-9_\-\.\s]")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 14)]
        public string FirmwareType = "user";

        [DefaultValue(null)]
        [MinLength(0)]
        [MaxLength(128)]
        [RegularExpression(@"[[a-zA-Z0-9_\-\/\.\:]")]
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Populate, Order = 15)]
        public string FirmwareFingerprint;
    }
}