using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Model.Settings
{
    public class NotificationConfig  : BaseConfig
    {
        public NotificationConfig() : base() { }

        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 1)]
        [ExcelConfig(Position = 1, Description = "Enable pushbullet notification")]
        public bool EnablePushBulletNotification { get; set; }

        [DefaultValue(false)]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 2)]
        [ExcelConfig(Position = 2, Description = "Enable email notification")]
        public bool EnableEmailNotification { get; set; }

        [DefaultValue("")]
        [JsonProperty(Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 3)]
        [ExcelConfig(Position = 3, Description = "API Key to connect to pushbullet - go to pushbullet.com to get one")]
        public string PushBulletApiKey { get; set; }

        [DefaultValue("")]
        [JsonProperty(Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        [ExcelConfig(Position = 4, Description = "Gmail email address to use to send email")]
        public string GmailUsername { get; set; }

        [DefaultValue("")]
        [JsonProperty(Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 5)]
        [ExcelConfig(Position = 5, Description = "Gmail password")]
        public string GmailPassword { get; set; }

        [DefaultValue("")]
        [JsonProperty(Required = Required.AllowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 6)]
        [ExcelConfig(Position = 6, Description = "List of email address to recieve notificaitons")]
        public string Recipients { get; set; }

    }
}
