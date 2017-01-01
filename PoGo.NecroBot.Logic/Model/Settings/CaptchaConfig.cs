using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PoGo.NecroBot.Logic.Model.Settings
{
    [JsonObject(Title = "Captcha Config", Description = "Setup captcha config", ItemRequired = Required.DisallowNull)]
    public class CaptchaConfig : BaseConfig
    {
        public CaptchaConfig() : base() { }

        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        [ExcelConfig(Position = 1, Description = "Enable display captcha on browser and allow resolve manually")]
        public bool AllowManualCaptchaResolve{ get; set; }

        [DefaultValue(120)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 2)]
        [ExcelConfig(Position = 2, Description = "Number of second bot will wait for you to resolve captcha, if after x second and captcha havent resolved yet. bot will continue ")]
        public int ManualCaptchaTimeout { get; set; }

        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 3)]
        [ExcelConfig(Position =3, Description = "Play alert sound")]
        public bool PlaySoundOnCaptcha { get; set; }

        [DefaultValue(true)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        [ExcelConfig(Position = 4, Description = "Display captcha as top most screen")]
        public bool DisplayOnTop  { get; set; }


        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        [ExcelConfig(Position = 5, Description = "Enable Auto captcha solve with 2Captcha")]
        public bool Enable2Captcha { get; set; }

        [DefaultValue(3)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 6)]
        [ExcelConfig(Position = 6, Description = "Number of time bot try to resolve captcha automatically")]
        public int AutoCaptchaRetries{ get; set; }


        [DefaultValue("")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 7)]
        [ExcelConfig(Position = 7, Description = "The 2Captcha API Key")]

        public string TwoCaptchaAPIKey { get; set; }
     
        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        [ExcelConfig(Position = 8, Description = "Enable Auto captcha solve with Anti-Captcha")]
        public bool EnableAntiCaptcha { get; set; }


        [DefaultValue("201.33.206.229")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        [ExcelConfig(Position = 9, Description = "Anti captcha API KEY")]
        public string AntiCaptchaAPIKey { get; set; }

        [DefaultValue("")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        [ExcelConfig(Position = 10, Description = "Proxy host use by captcha service")]
        public string ProxyHost { get; set; }


        [DefaultValue(3128)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        [ExcelConfig(Position = 11, Description = "Proxy port use by captcha service")]
        public int  ProxyPort{ get; set; }

        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        [ExcelConfig(Position = 12, Description = "Enable Auto captcha solve with CaptchaSolutions.com")]

        public bool EnableCaptchaSolutions { get; internal set; }

        [DefaultValue("")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        [ExcelConfig(Position = 13, Description = "CaptchaSolutions API KEY")]

        public string CaptchaSolutionAPIKey { get; internal set; }
        [DefaultValue("")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        [ExcelConfig(Position = 14, Description = "Captcha Solutions Secret key")]

        public string CaptchaSolutionsSecretKey { get; internal set; }

        [DefaultValue(120)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        [ExcelConfig(Position = 15, Description = "Timeout for auto captcha solving")]
        public int AutoCaptchaTimeout { get; internal set; }
    }
}