#region using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.Logging;
using PoGo.NecroBot.Logic.Utils;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.Helpers;

#endregion

namespace PoGo.NecroBot.Logic.Model.Settings
{
    [JsonObject(Title = "Authentication Settings", Description = "Set your authentication settings.", ItemRequired = Required.DisallowNull)]
    public class AuthSettings
    {
        [JsonIgnore]
        private string _filePath;

        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore, Order = 1)]
        public AuthConfig AuthConfig = new AuthConfig();
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore, Order = 2)]
        public ProxyConfig ProxyConfig = new ProxyConfig();
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore, Order = 3)]
        public DeviceConfig DeviceConfig = new DeviceConfig();

        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore, Order = 3)]
        public APIConfig APIConfig = new APIConfig();

        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate, Order = 4)]
        [DefaultValue(false)]
        public bool AllowMultipleBot = false;

        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore, Order = 5)]
        public List<AuthConfig> Bots= new List<AuthConfig>();

        private JSchema _schema;

        private JSchema JsonSchema
        {
            get
            {
                if (_schema != null)
                    return _schema;
                // JSON Schemas from .NET types
                var generator = new JSchemaGenerator
                {
                    // change contract resolver so property names are camel case
                    //ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    // sets the default required state of schemas
                    DefaultRequired = Required.Default,
                    // types with no defined ID have their type name as the ID
                    SchemaIdGenerationHandling = SchemaIdGenerationHandling.TypeName,
                    // use the default order of properties.
                    SchemaPropertyOrderHandling = SchemaPropertyOrderHandling.Default,
                    // referenced schemas are inline.
                    SchemaLocationHandling = SchemaLocationHandling.Inline,
                    // all schemas can be referenced.    
                    SchemaReferenceHandling = SchemaReferenceHandling.None
                };
                // change Zone enum to generate a string property
                var strEnumGen = new StringEnumGenerationProvider {CamelCaseText = true};
                generator.GenerationProviders.Add(strEnumGen);
                // generate json schema 
                var type = typeof(AuthSettings);
                try
                {
                    var schema = generator.Generate(type);
                    schema.Title = type.Name;
                    //
                    _schema = schema;
                }
                catch (Exception)
                {
                }
                return _schema;
            }
        }

        //private JObject _jsonObject;
        //public JObject JsonObject
        //{
        //    get
        //    {
        //        if (_jsonObject == null)
        //            _jsonObject = JObject.FromObject(this);

        //        return _jsonObject;
        //    }
        //    set
        //    {
        //        _jsonObject = value;
        //    }
        //}

        public AuthSettings()
        {
            InitializePropertyDefaultValues(this);
        }

        public void InitializePropertyDefaultValues(object obj)
        {
            var fields = obj.GetType().GetFields();

            foreach (var field in fields)
            {
                var d = field.GetCustomAttribute<DefaultValueAttribute>();

                if (d != null)
                    field.SetValue(obj, d.Value);
            }
        }

        //public void Load(JObject jsonObj)
        //{
        //    try
        //    {
        //        var input = jsonObj.ToString(Formatting.None, new StringEnumConverter { CamelCaseText = true });
        //        var settings = new JsonSerializerSettings();
        //        settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
        //        JsonConvert.PopulateObject(input, this, settings);
        //        Save(_filePath);
        //    }
        //    catch (JsonReaderException exception)
        //    {
        //            Logger.Write("JSON Exception: " + exception.Message, LogLevel.Error);
        //    }
        //}

        public void Load(string configFile, string schemaFile, int schemaVersion, bool validate = false)
        {
            try
            {
                _filePath = configFile;

                if (File.Exists(_filePath))
                {
                    // if the file exists, load the settings
                    var input = File.ReadAllText(_filePath, Encoding.UTF8);

                    if (validate)
                    {
                        var jsonObj = JObject.Parse(input);

                        // Migrate before validation.
                        MigrateSettings(schemaVersion, jsonObj, configFile, schemaFile);

                        // validate Json using JsonSchema
                        Logger.Write("Validating auth.json...");
                        IList<ValidationError> errors = null;
                        bool valid;
                        try
                        {
                            valid = jsonObj.IsValid(JsonSchema, out errors);
                        }
                        catch (JSchemaException ex)
                        {
                            if (ex.Message.Contains("commercial licence") || ex.Message.Contains("free-quota"))
                            {
                                Logger.Write(
                                    "auth.json: " + ex.Message);
                                valid = false;
                            }
                            else
                            {
                                throw;
                            }
                        }
                        if (!valid)
                        {
                            if (errors != null)
                            {
                                foreach (var error in errors)
                                {
                                    Logger.Write(
                                        "auth.json [Line: " + error.LineNumber + ", Position: " + error.LinePosition + "]: " +
                                        error.Path + " " +
                                        error.Message, LogLevel.Error);
                                }
                            }

                            Logger.Write("Fix auth.json and restart NecroBot or press a key to ignore and continue...",
                                LogLevel.Warning);
                            Console.ReadKey();
                        }

                        // Now we know it's valid so update input with the migrated version.
                        input = jsonObj.ToString();
                    }

                    var settings = new JsonSerializerSettings();
                    settings.Converters.Add(new StringEnumConverter {CamelCaseText = true});
                    JsonConvert.PopulateObject(input, this, settings);
                }
                // Do some post-load logic to determine what device info to be using - if 'custom' is set we just take what's in the file without question
                if (DeviceConfig.DevicePlatform.Equals("ios", StringComparison.InvariantCultureIgnoreCase))
                {
                    // iOS
                    if (DeviceConfig.DevicePackageName.Equals("random", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var randomAppleDeviceInfo = DeviceInfoHelper.GetRandomIosDevice();
                        SetDevInfoByDeviceInfo(randomAppleDeviceInfo);

                        // After generating iOS settings, automatically set the package name to "custom", so that we don't regenerate settings every time we start.
                        DeviceConfig.DevicePackageName = "custom";
                    }
                }
                else
                {
                    // We cannot emulate Android at the moment, so if we got here, then regenerate the settings with random iOS device.
                    /*
                    // Android
                    if (!DeviceConfig.DevicePackageName.Equals("random", StringComparison.InvariantCultureIgnoreCase) &&
                        !DeviceConfig.DevicePackageName.Equals("custom", StringComparison.InvariantCultureIgnoreCase))
                    {
                        // User requested a specific device package, check to see if it exists and if so, set it up - otherwise fall-back to random package
                        var keepDevId = DeviceConfig.DeviceId;
                        SetDevInfoByKey();
                        DeviceConfig.DeviceId = keepDevId;
                    }
                    if (DeviceConfig.DevicePackageName.Equals("random", StringComparison.InvariantCultureIgnoreCase))
                    {
                        // Random is set, so pick a random device package and set it up - it will get saved to disk below and re-used in subsequent sessions
                        var rnd = new Random();
                        var rndIdx = rnd.Next(0, DeviceInfoHelper.AndroidDeviceInfoSets.Keys.Count - 1);
                        DeviceConfig.DevicePackageName = DeviceInfoHelper.AndroidDeviceInfoSets.Keys.ToArray()[rndIdx];
                        SetDevInfoByKey();
                    }
                    */
                    DeviceConfig.DevicePlatform = "ios";
                    DeviceConfig.DevicePackageName = "custom";

                    var randomAppleDeviceInfo = DeviceInfoHelper.GetRandomIosDevice();
                    SetDevInfoByDeviceInfo(randomAppleDeviceInfo);

                    // Clear out the android fields.
                    DeviceConfig.AndroidBoardName = null;
                    DeviceConfig.AndroidBootloader = null;
                    DeviceConfig.DeviceModelIdentifier = null;
                    DeviceConfig.FirmwareTags = null;
                    DeviceConfig.FirmwareFingerprint = null;
                }

                if (string.IsNullOrEmpty(DeviceConfig.DeviceId) || DeviceConfig.DeviceId == "8525f5d8201f78b5")
                {
                    // Changed to random hex as full alphabet letters could have been flagged
                    // iOS device ids are 16 bytes (32 chars long)
                    DeviceConfig.DeviceId = RandomString(32, "0123456789abcdef");
                }

                Save(_filePath);
            }
            catch (JsonReaderException exception)
            {
                if (exception.Message.Contains("Unexpected character") && exception.Message.Contains("PtcUsername"))
                    Logger.Write("JSON Exception: You need to properly configure your PtcUsername using quotations.",
                        LogLevel.Error);
                else if (exception.Message.Contains("Unexpected character") &&
                         exception.Message.Contains("PtcPassword"))
                    Logger.Write(
                        "JSON Exception: You need to properly configure your PtcPassword using quotations.",
                        LogLevel.Error);
                else if (exception.Message.Contains("Unexpected character") &&
                         exception.Message.Contains("GoogleUsername"))
                    Logger.Write(
                        "JSON Exception: You need to properly configure your GoogleUsername using quotations.",
                        LogLevel.Error);
                else if (exception.Message.Contains("Unexpected character") &&
                         exception.Message.Contains("GooglePassword"))
                    Logger.Write(
                        "JSON Exception: You need to properly configure your GooglePassword using quotations.",
                        LogLevel.Error);
                else
                    Logger.Write("JSON Exception: " + exception.Message, LogLevel.Error);
            }
        }

        private static void MigrateSettings(int schemaVersion, JObject settings, string configFile, string schemaFile)
        {
            if (schemaVersion == UpdateConfig.CURRENT_SCHEMA_VERSION)
            {
                Logger.Write("Auth Configuration is up-to-date. Schema version: " + schemaVersion);
                return;
            }

            // Backup old config file.
            long ts = DateTime.UtcNow.ToUnixTime(); // Add timestamp to avoid file conflicts
            string backupPath = configFile.Replace(".json", $"-{schemaVersion}-{ts}.backup.json");
            Logger.Write($"Backing up auth.json to: {backupPath}", LogLevel.Info);
            File.Copy(configFile, backupPath);

            // Add future schema migrations below.
            int version;
            for (version = schemaVersion; version < UpdateConfig.CURRENT_SCHEMA_VERSION; version++)
            {
                Logger.Write(
                    $"Migrating auth configuration from schema version {version} to {version + 1}",
                    LogLevel.Info
                );
                switch (version)
                {
                    case 3:
                        settings["DeviceConfig"]["AndroidBoardName"] = null;
                        settings["DeviceConfig"]["AndroidBootloader"] = null;
                        settings["DeviceConfig"]["DeviceModelIdentifier"] = null;
                        settings["DeviceConfig"]["FirmwareTags"] = null;
                        settings["DeviceConfig"]["FirmwareFingerprint"] = null;
                        break;

                    // Add more here.
                }
            }
        }

        public void Save(string fullPath, bool validate = false)
        {
            var jsonSerializeSettings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Include,
                Formatting = Formatting.Indented,
                Converters = new JsonConverter[] {new StringEnumConverter {CamelCaseText = true}}
            };
            var output = JsonConvert.SerializeObject(this, jsonSerializeSettings);

            var folder = Path.GetDirectoryName(fullPath);
            if (folder != null && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            File.WriteAllText(fullPath, output, Encoding.UTF8);

            //JsonSchema
            File.WriteAllText(fullPath.Replace(".json", ".schema.json"), JsonSchema.ToString(), Encoding.UTF8);

            if (!validate) return;

            // validate Json using JsonSchema
            Logger.Write("Validating auth.json...");
            var jsonObj = JObject.Parse(output);
            IList<ValidationError> errors;
            var valid = jsonObj.IsValid(JsonSchema, out errors);
            if (valid) return;
            foreach (var error in errors)
            {
                Logger.Write(
                    "auth.json [Line: " + error.LineNumber + ", Position: " + error.LinePosition + "]: " + error.Path +
                    " " +
                    error.Message, LogLevel.Error);
            }
            Logger.Write(
                "Fix auth.json and restart NecroBot or press a key to ignore and continue...",
                LogLevel.Warning
            );
            Console.ReadKey();
        }

        public void Save()
        {
            if (!string.IsNullOrEmpty(_filePath))
            {
                Save(_filePath);
            }
        }

        public void CheckProxy(ITranslation translator)
        {
            using (var tempWebClient = new NecroWebClient())
            {
                var unproxiedIp = WebClientExtensions.DownloadString(tempWebClient,
                    new Uri("https://api.ipify.org/?format=text"));
                if (ProxyConfig.UseProxy)
                {
                    tempWebClient.Proxy = InitProxy();
                    var proxiedIPres = WebClientExtensions.DownloadString(tempWebClient,
                        new Uri("https://api.ipify.org/?format=text"));
                    var proxiedIp = proxiedIPres == null ? "INVALID PROXY" : proxiedIPres;
                    Logger.Write(translator.GetTranslation(TranslationString.Proxied, unproxiedIp, proxiedIp),
                        LogLevel.Info, unproxiedIp == proxiedIp ? ConsoleColor.Red : ConsoleColor.Green);

                    if (unproxiedIp != proxiedIp && proxiedIPres != null)
                        return;

                    Logger.Write(translator.GetTranslation(TranslationString.FixProxySettings), LogLevel.Info,
                        ConsoleColor.Red);
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                else
                {
                    Logger.Write(translator.GetTranslation(TranslationString.Unproxied, unproxiedIp), LogLevel.Info,
                        ConsoleColor.Red);
                }
            }
        }

        private static string RandomString(int length, string alphabet = "abcdefghijklmnopqrstuvwxyz0123456789")
        {
            var outOfRange = byte.MaxValue + 1 - (byte.MaxValue + 1) % alphabet.Length;

            return string.Concat(
                Enumerable
                    .Repeat(0, int.MaxValue)
                    .Select(e => RandomByte())
                    .Where(randomByte => randomByte < outOfRange)
                    .Take(length)
                    .Select(randomByte => alphabet[randomByte % alphabet.Length])
            );
        }

        private static byte RandomByte()
        {
            using (var randomizationProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[1];
                randomizationProvider.GetBytes(randomBytes);
                return randomBytes.Single();
            }
        }

        private void SetDevInfoByKey()
        {
            if (DeviceInfoHelper.AndroidDeviceInfoSets.ContainsKey(DeviceConfig.DevicePackageName))
            {
                SetDevInfoByDeviceInfo(DeviceInfoHelper.AndroidDeviceInfoSets[DeviceConfig.DevicePackageName]);
            }
            else
            {
                throw new ArgumentException(
                    "Invalid device info package! Check your auth.config file and make sure a valid DevicePackageName is set. For simple use set it to 'random'. If you have a custom device, then set it to 'custom'.");
            }
        }

        private void SetDevInfoByDeviceInfo(DeviceInfo deviceInfo)
        {
            DeviceConfig.AndroidBoardName = deviceInfo.AndroidBoardName;
            DeviceConfig.AndroidBootloader = deviceInfo.AndroidBootloader;
            DeviceConfig.DeviceBrand = deviceInfo.DeviceBrand;
            DeviceConfig.DeviceId = deviceInfo.DeviceId;
            DeviceConfig.DeviceModel = deviceInfo.DeviceModel;
            DeviceConfig.DeviceModelBoot = deviceInfo.DeviceModelBoot;
            DeviceConfig.DeviceModelIdentifier = deviceInfo.DeviceModelIdentifier;
            DeviceConfig.FirmwareBrand = deviceInfo.FirmwareBrand;
            DeviceConfig.FirmwareFingerprint = deviceInfo.FirmwareFingerprint;
            DeviceConfig.FirmwareTags = deviceInfo.FirmwareTags;
            DeviceConfig.FirmwareType = deviceInfo.FirmwareType;
            DeviceConfig.HardwareManufacturer = deviceInfo.HardwareManufacturer;
            DeviceConfig.HardwareModel = deviceInfo.HardwareModel;
        }

        private WebProxy InitProxy()
        {
            if (!ProxyConfig.UseProxy) return null;

            var prox = new WebProxy(new Uri($"http://{ProxyConfig.UseProxyHost}:{ProxyConfig.UseProxyPort}"), false,
                null);

            if (ProxyConfig.UseProxyAuthentication)
                prox.Credentials = new NetworkCredential(ProxyConfig.UseProxyUsername, ProxyConfig.UseProxyPassword);

            return prox;
        }
    }
}