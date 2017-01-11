#region using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.Logging;
using PoGo.NecroBot.Logic.State;
using PokemonGo.RocketAPI.Enums;
using POGOProtos.Enums;
using PoGo.NecroBot.Logic.Service.Elevation;
using PokemonGo.RocketAPI.Extensions;

#endregion

namespace PoGo.NecroBot.Logic.Model.Settings
{
    [JsonObject(Title = " Global Settings", Description = "Set your global settings.", ItemRequired = Required.DisallowNull)]
    public class GlobalSettings
    {
        [JsonIgnore]
        public AuthSettings Auth = new AuthSettings();
        [JsonIgnore]
        public string GeneralConfigPath;
        [JsonIgnore]
        public string ProfileConfigPath;
        [JsonIgnore]
        public string ProfilePath;

        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [ExcelConfig(SheetName = "ConsoleConfig", Description = "Setting up the console output")]
        public ConsoleConfig ConsoleConfig = new ConsoleConfig();

        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [ExcelConfig(SheetName = "UpdateConfig", Description = "Setting up the auto checking for every time bot start up")]
        public UpdateConfig UpdateConfig = new UpdateConfig();

        [ExcelConfig(SheetName = "WebsocketsConfig", Description = "Setting up the web socket that allow bot to communicate with Visualizer.")]

        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public WebsocketsConfig WebsocketsConfig = new WebsocketsConfig();

        [ExcelConfig(SheetName = "LocationConfig", Description = "Setting up location setting for bot.")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public LocationConfig LocationConfig = new LocationConfig();

        [ExcelConfig(SheetName = "TelegramConfig", Description = "Setting up Telegram API to allow control bot from Telegram .")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public TelegramConfig TelegramConfig = new TelegramConfig();

        [ExcelConfig(SheetName = "GPXConfig", Description = "Setup GPS Pathing for bot.")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public GpxConfig GPXConfig = new GpxConfig();

        [ExcelConfig(SheetName = "SnipeConfig", Description = "Setting up option for snipe.")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public SnipeConfig SnipeConfig = new SnipeConfig();

        [ExcelConfig(SheetName = "HumanWalkSnipeConfig", Description = "Setting up option for human walk snipe.")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public HumanWalkSnipeConfig HumanWalkSnipeConfig = new HumanWalkSnipeConfig();

        [ExcelConfig(SheetName = "DataSharingConfig", Description = "Setting up data socket sharing.")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DataSharingConfig DataSharingConfig = new DataSharingConfig();

        [ExcelConfig(SheetName = "PokeStopConfig", Description = "Setting up for farming pokestop.")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public PokeStopConfig PokeStopConfig = new PokeStopConfig();

        [ExcelConfig(SheetName = "GymConfig", Description = "Setting up for gym and battle.")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public GymConfig GymConfig = new GymConfig();

        [ExcelConfig(SheetName = "PokemonConfig", Description = "Setting up for pokemon catching, evolve, transfer, upgrade.")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public PokemonConfig PokemonConfig = new PokemonConfig();

        [ExcelConfig(SheetName = "RecycleConfig", Description = "Setting up for inventory cleanup.")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ItemRecycleConfig RecycleConfig = new ItemRecycleConfig();

        [ExcelConfig(SheetName = "CustomCatchConfig", Description = "Setting up for some custom parametter for catching.")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public CustomCatchConfig CustomCatchConfig = new CustomCatchConfig();

        [ExcelConfig(SheetName = "PlayerConfig", Description = "Setting up for some custom parametter for bot perfom user action.")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public PlayerConfig PlayerConfig = new PlayerConfig();

        [ExcelConfig(SheetName = "SoftBanConfig", Description = "Setting up for softban resolve.")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public SoftBanConfig SoftBanConfig = new SoftBanConfig();

        [ExcelConfig (SheetName = "GoogleWalkConfig", Description = "Setup parametter for google walk such as api key, account, rules..")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public GoogleWalkConfig GoogleWalkConfig = new GoogleWalkConfig();

        [ExcelConfig(SheetName = "YoursWalkConfig", Description = "Setup parametter for YoursWalkConfig walk such as api key, account, rules..")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public YoursWalkConfig YoursWalkConfig = new YoursWalkConfig();

        [ExcelConfig(SheetName = "MapzenWalkConfig", Description = "Setup parametter for MapzenWalkConfig walk such as api key, account, rules..")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public MapzenWalkConfig MapzenWalkConfig = new MapzenWalkConfig();

        //[ExcelConfig (SheetName = "ItemRecycleFilter", Description ="Set number of each item we want bot to keep when it perfom recycle for get free space")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ItemRecycleFilter> ItemRecycleFilter = Settings.ItemRecycleFilter.ItemRecycleFilterDefault();

        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<PokemonId> PokemonsNotToTransfer = TransferConfig.PokemonsNotToTransferDefault();

        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<PokemonId> PokemonsToEvolve = EvolveConfig.PokemonsToEvolveDefault();

        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<PokemonId> PokemonsToLevelUp = LevelUpConfig.PokemonsToLevelUpDefault();

        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<PokemonId> PokemonsToIgnore = CatchConfig.PokemonsToIgnoreDefault();

        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]

        [ExcelConfig(SheetName = "CaptchaConfig", Description = "Captcha config to define the way you prefer to resolve captcha")]
        public CaptchaConfig CaptchaConfig = new CaptchaConfig();


        [ExcelConfig(SheetName = "PokemonsTransferFilter", Description = "Setting up pokemon filter rules")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<PokemonId, TransferFilter> PokemonsTransferFilter = TransferFilter.TransferFilterDefault();

        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public SnipeSettings PokemonToSnipe = SnipeSettings.Default();

        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<PokemonId> PokemonToUseMasterball = CatchConfig.PokemonsToUseMasterballDefault();

        [ExcelConfig(Description = "Setting up human walk snipe filter by pokemon", SheetName = "HumanWalkSnipeFilter")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<PokemonId, HumanWalkSnipeFilter> HumanWalkSnipeFilters = HumanWalkSnipeFilter.Default();

        [ExcelConfig(Description = "Setting up pokemon filter for level up", SheetName = "PokemonUpgradeFilter")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<PokemonId, UpgradeFilter> PokemonUpgradeFilters = UpgradeFilter.Default();

        [ExcelConfig (Description ="Setting up bot to use multiple account" , SheetName = "MultipleBotConfig")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public MultipleBotConfig MultipleBotConfig = MultipleBotConfig.Default();

        [ExcelConfig(Description = "Setting up notifications setting", SheetName = "NotificationConfig")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public NotificationConfig NotificationConfig = new NotificationConfig();

        [ExcelConfig(SheetName = "SnipePokemonFilter", Description ="Setup list pokemon for auto snipe")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<PokemonId, SnipeFilter> SnipePokemonFilter = SnipeFilter.SniperFilterDefault();

        [ExcelConfig(SheetName = "EvolvePokemonFilter", Description = "Setup list pokemon for auto evolve")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<PokemonId, EvolveFilter> EvolvePokemonFilter = EvolveFilter.Default();

        [ExcelConfig(SheetName = "CatchPokemonFilter", Description = "Setup list pokemon for catch")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<PokemonId, CatchFilter> CatchPokemonFilter = CatchFilter.Default();

        [ExcelConfig(SheetName = "BotSwitchPokemonFilter", Description = "Define the filter to switch bot in multiple account mode.")]
        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<PokemonId, BotSwitchPokemonFilter> BotSwitchPokemonFilters = BotSwitchPokemonFilter.Default();


        public GlobalSettings()
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

        public static GlobalSettings Default => new GlobalSettings();

        private static JSchema _schema;

        private static JSchema JsonSchema
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
                    // types with no defined ID have their type name as the ID
                    SchemaIdGenerationHandling = SchemaIdGenerationHandling.TypeName,
                    // use the default order of properties.
                    SchemaPropertyOrderHandling = SchemaPropertyOrderHandling.Default,
                    // referenced schemas are inline.
                    SchemaLocationHandling = SchemaLocationHandling.Inline,
                    // no schemas can be referenced.    
                    SchemaReferenceHandling = SchemaReferenceHandling.None
                };
                // change Zone enum to generate a string property
                var strEnumGen = new StringEnumGenerationProvider {CamelCaseText = true};
                generator.GenerationProviders.Add(strEnumGen);
                // generate json schema 
                var type = typeof(GlobalSettings);
                var schema = generator.Generate(type);
                schema.Title = type.Name;
                //
                _schema = schema;
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


        //public void Load(JObject jsonObj)
        //{
        //    try
        //    {
        //        var input = jsonObj.ToString(Formatting.None, new StringEnumConverter { CamelCaseText = true });
        //        var settings = new JsonSerializerSettings();
        //        settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
        //        JsonConvert.PopulateObject(input, this, settings);
        //        var configFile = Path.Combine(ProfileConfigPath, "config.json");
        //        this.Save(configFile);

        //    }
        //    catch (JsonReaderException exception)
        //    {
        //            Logger.Write("JSON Exception: " + exception.Message, LogLevel.Error);
        //    }
        //}

        public static GlobalSettings Load(string path, bool validate = false)
        {
            GlobalSettings settings;

            var profilePath = "";
            var profileConfigPath = "";
            var configFile = "";
            var schemaFile = "";


            if (Path.IsPathRooted(path))
            {
                profileConfigPath = Path.GetDirectoryName(path);
                configFile = path;
                schemaFile = path.Replace(".json", ".schema.json");


            }
            else {
                profilePath = Path.Combine(Directory.GetCurrentDirectory(), path);
                profileConfigPath = Path.Combine(profilePath, "config");
                configFile = Path.Combine(profileConfigPath, "config.json");
                schemaFile = Path.Combine(profileConfigPath, "config.schema.json");
            }
                var shouldExit = false;
            int schemaVersionBeforeUpgrade = 0;

            if (File.Exists(configFile))
            {
                try
                {
                    //if the file exists, load the settings
                    string input;
                    var count = 0;
                    while (true)
                    {
                        try
                        {
                            input = File.ReadAllText(configFile, Encoding.UTF8);
                            if (!input.Contains("DeprecatedMoves"))
                                input = input.Replace("\"Moves\"", $"\"DeprecatedMoves\"");

                            break;
                        }
                        catch (Exception exception)
                        {
                            if (count > 10)
                            {
                                //sometimes we have to wait close to config.json for access
                                Logger.Write("configFile: " + exception.Message, LogLevel.Error);
                            }
                            count++;
                            Thread.Sleep(1000);
                        }
                    }

                    var jsonSettings = new JsonSerializerSettings();
                    jsonSettings.Converters.Add(new StringEnumConverter {CamelCaseText = true});
                    jsonSettings.ObjectCreationHandling = ObjectCreationHandling.Replace;
                    jsonSettings.DefaultValueHandling = DefaultValueHandling.Populate;

                    try
                    {
                        // validate Json using JsonSchema
                        if (validate)
                        {
                            JObject jsonObj = JObject.Parse(input);

                            // Migrate before validation.
                            MigrateSettings(jsonObj, configFile, schemaFile);

                            // Save the original schema version since we need to pass it to AuthSettings for migration.
                            schemaVersionBeforeUpgrade = (int)jsonObj["UpdateConfig"]["SchemaVersion"];

                            // After migration we need to update the schema version to the latest version.
                            jsonObj["UpdateConfig"]["SchemaVersion"] = UpdateConfig.CURRENT_SCHEMA_VERSION;

                            Logger.Write("Validating config.json...");
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
                                        "config.json: " + ex.Message);
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
                                    foreach (var error in errors)
                                    {
                                        Logger.Write(
                                            "config.json [Line: " + error.LineNumber + ", Position: " + error.LinePosition +
                                            "]: " +
                                            error.Path + " " +
                                            error.Message, LogLevel.Error);
                                    }

                                Logger.Write(
                                    "Fix config.json and restart NecroBot or press a key to ignore and continue...",
                                    LogLevel.Warning);
                                Console.ReadKey();
                            }

                            // Now we know it's valid so update input with the migrated version.
                            input = jsonObj.ToString();
                        }

                        settings = JsonConvert.DeserializeObject<GlobalSettings>(input, jsonSettings);
                    }
                    catch (JsonSerializationException exception)
                    {
                        Logger.Write("JSON Exception: " + exception.Message, LogLevel.Error);
                        return null;
                    }
                    catch (JsonReaderException exception)
                    {
                        Logger.Write("JSON Exception: " + exception.Message, LogLevel.Error);
                        return null;
                    }

                    //This makes sure that existing config files dont get null values which lead to an exception
                    foreach (var filter in settings.PokemonsTransferFilter.Where(x => x.Value.KeepMinOperator == null))
                    {
                        filter.Value.KeepMinOperator = "or";
                    }
                    foreach (var filter in settings.PokemonsTransferFilter.Where(x => x.Value.Moves == null))
                    {
                        filter.Value.Moves = filter.Value.DeprecatedMoves != null
                            ? new List<List<PokemonMove>> {filter.Value.DeprecatedMoves}
                            : filter.Value.Moves ?? new List<List<PokemonMove>>();
                    }
                    foreach (var filter in settings.PokemonsTransferFilter.Where(x => x.Value.MovesOperator == null))
                    {
                        filter.Value.MovesOperator = "or";
                    }
                }
                catch (JsonReaderException exception)
                {
                    Logger.Write("JSON Exception: " + exception.Message, LogLevel.Error);
                    return null;
                }
            }
            else
            {
                settings = new GlobalSettings();
                shouldExit = true;
            }

            settings.ProfilePath = profilePath;
            settings.ProfileConfigPath = profileConfigPath;
            settings.GeneralConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "config");

            settings.Save(configFile);
            settings.Auth.Load(Path.Combine(profileConfigPath, "auth.json"), Path.Combine(profileConfigPath, "auth.schema.json"), schemaVersionBeforeUpgrade, validate);

            return shouldExit ? null : settings;
        }

        private static void MigrateSettings(JObject settings, string configFile, string schemaFile)
        {
            if (settings["UpdateConfig"]?["SchemaVersion"] == null)
            {
                // The is the first time setup for old config.json files without the SchemaVersion.
                // Just set this to 0 so that we can handle the upgrade in case 0.
                settings["UpdateConfig"]["SchemaVersion"] = 0;
            }

            int schemaVersion = (int)settings["UpdateConfig"]["SchemaVersion"];
            if (schemaVersion == UpdateConfig.CURRENT_SCHEMA_VERSION)
            {
                Logger.Write("Configuration is up-to-date. Schema version: " + schemaVersion);
                return;
            }

            // Backup old config file.
            long ts = DateTime.UtcNow.ToUnixTime(); // Add timestamp to avoid file conflicts
            string backupPath = configFile.Replace(".json", $"-{schemaVersion}-{ts}.backup.json");
            Logger.Write($"Backing up config.json to: {backupPath}", LogLevel.Info);
            File.Copy(configFile, backupPath);

            // Add future schema migrations below.
            int version;
            for (version = schemaVersion; version < UpdateConfig.CURRENT_SCHEMA_VERSION; version++)
            {
                Logger.Write($"Migrating configuration from schema version {version} to {version + 1}", LogLevel.Info);
                switch(version)
                {
                    case 1:
                        // Delete the auto complete tutorial settings.
                        ((JObject)settings["PlayerConfig"]).Remove("AutoCompleteTutorial");
                        ((JObject)settings["PlayerConfig"]).Remove("DesiredNickname");
                        ((JObject)settings["PlayerConfig"]).Remove("DesiredGender");
                        ((JObject)settings["PlayerConfig"]).Remove("DesiredStarter");
                        break;

                    case 2:
                        // Remove the TransferConfigAndAuthOnUpdate setting since we always transfer now.
                        ((JObject)settings["UpdateConfig"]).Remove("TransferConfigAndAuthOnUpdate");
                        break;

                    // Add more here.
                }
            }
        }

        public void CheckProxy(ITranslation translator)
        {
            Auth.CheckProxy(translator);
        }

        public static bool PromptForSetup(ITranslation translator)
        {
            bool promptForSetup = PromptForBoolean(translator, translator.GetTranslation(TranslationString.FirstStartPrompt, "Y", "N"));
            if (!promptForSetup)
                Logger.Write(translator.GetTranslation(TranslationString.FirstStartAutoGenSettings));

            return promptForSetup;
        }

        public static Session SetupSettings(Session session, GlobalSettings settings, IElevationService elevationService, string configPath)
        {
            var newSession = SetupTranslationCode(session, elevationService, session.Translation, settings);

            SetupAccountType(newSession.Translation, settings);
            SetupUserAccount(newSession.Translation, settings);
            SetupConfig(newSession.Translation, settings);
            SetupWalkingConfig(newSession.Translation, settings);
            SetupTelegramConfig(newSession.Translation, settings);
            SetupProxyConfig(newSession.Translation, settings);
            SetupWebSocketConfig(newSession.Translation, settings);
            SaveFiles(settings, configPath);

            Logger.Write(session.Translation.GetTranslation(TranslationString.FirstStartSetupCompleted), LogLevel.Info);

            return newSession;
        }

        private static Session SetupTranslationCode(Session session, IElevationService elevationService, ITranslation translator, GlobalSettings settings)
        {
            if (false == PromptForBoolean(translator, translator.GetTranslation(TranslationString.FirstStartLanguagePrompt, "Y", "N")))
                return session;

            string strInput = PromptForString(translator, translator.GetTranslation(TranslationString.FirstStartLanguageCodePrompt));

            settings.ConsoleConfig.TranslationLanguageCode = strInput;
            session = new Session(new ClientSettings(settings, elevationService), new LogicSettings(settings), elevationService);
            translator = session.Translation;
            Logger.Write(translator.GetTranslation(TranslationString.FirstStartLanguageConfirm, strInput));

            return session;
        }

        private static void SetupProxyConfig(ITranslation translator, GlobalSettings settings)
        {
            if (false == PromptForBoolean(translator, translator.GetTranslation(TranslationString.FirstStartSetupProxyPrompt, "Y", "N")))
                return;
            
            settings.Auth.ProxyConfig.UseProxy = true;

            string strInput = PromptForString(translator, translator.GetTranslation(TranslationString.FirstStartSetupProxyHostPrompt));
            settings.Auth.ProxyConfig.UseProxyHost = strInput;
            Logger.Write(translator.GetTranslation(TranslationString.FirstStartSetupProxyHostConfirm, strInput));
            
            strInput = PromptForString(translator, translator.GetTranslation(TranslationString.FirstStartSetupProxyPortPrompt));
            settings.Auth.ProxyConfig.UseProxyPort = strInput;
            Logger.Write(translator.GetTranslation(TranslationString.FirstStartSetupProxyPortConfirm, strInput));
            
            if (false == PromptForBoolean(translator, translator.GetTranslation(TranslationString.FirstStartSetupProxyAuthPrompt, "Y", "N")))
                return;
            
            settings.Auth.ProxyConfig.UseProxyAuthentication = true;
            
            strInput = PromptForString(translator, translator.GetTranslation(TranslationString.FirstStartSetupProxyUsernamePrompt));
            settings.Auth.ProxyConfig.UseProxyUsername = strInput;
            Logger.Write(translator.GetTranslation(TranslationString.FirstStartSetupProxyUsernameConfirm, strInput));

            strInput = PromptForString(translator, translator.GetTranslation(TranslationString.FirstStartSetupProxyPasswordPrompt));
            settings.Auth.ProxyConfig.UseProxyPassword = strInput;
            Logger.Write(translator.GetTranslation(TranslationString.FirstStartSetupProxyPasswordConfirm, strInput));
        }
        
        private static void SetupWalkingConfig(ITranslation translator, GlobalSettings settings)
        {
            if (false == PromptForBoolean(translator, translator.GetTranslation(TranslationString.FirstStartSetupWalkingSpeedPrompt, "Y", "N")))
                return;

            settings.LocationConfig.WalkingSpeedInKilometerPerHour = PromptForDouble(translator, translator.GetTranslation(TranslationString.FirstStartSetupWalkingSpeedKmHPrompt));
            Logger.Write(translator.GetTranslation(TranslationString.FirstStartSetupWalkingSpeedKmHConfirm, settings.LocationConfig.WalkingSpeedInKilometerPerHour.ToString()));

            settings.LocationConfig.UseWalkingSpeedVariant = PromptForBoolean(translator, translator.GetTranslation(TranslationString.FirstStartSetupUseWalkingSpeedVariantPrompt, "Y", "N"));

            settings.LocationConfig.WalkingSpeedVariant = PromptForDouble(translator, translator.GetTranslation(TranslationString.FirstStartSetupWalkingSpeedVariantPrompt));
            Logger.Write(translator.GetTranslation(TranslationString.FirstStartSetupWalkingSpeedVariantConfirm, settings.LocationConfig.WalkingSpeedVariant.ToString()));
        }
        
        private static void SetupWebSocketConfig(ITranslation translator, GlobalSettings settings)
        {
            if (false == PromptForBoolean(translator, translator.GetTranslation(TranslationString.FirstStartSetupWebSocketPrompt, "Y", "N")))
                return;
            
            settings.WebsocketsConfig.UseWebsocket = true;

            settings.WebsocketsConfig.WebSocketPort = PromptForInteger(translator, translator.GetTranslation(TranslationString.FirstStartSetupWebSocketPortPrompt));
            Logger.Write(translator.GetTranslation(TranslationString.FirstStartSetupWebSocketPortConfirm, settings.WebsocketsConfig.WebSocketPort.ToString()));
        }

        private static void SetupTelegramConfig(ITranslation translator, GlobalSettings settings)
        {
            if (false == PromptForBoolean(translator, translator.GetTranslation(TranslationString.FirstStartSetupTelegramPrompt, "Y", "N")))
                return;

            settings.TelegramConfig.UseTelegramAPI = true;

            string strInput = PromptForString(translator, translator.GetTranslation(TranslationString.FirstStartSetupTelegramCodePrompt));
            settings.TelegramConfig.TelegramAPIKey = strInput;
            Logger.Write(translator.GetTranslation(TranslationString.FirstStartSetupTelegramCodeConfirm, strInput));

            strInput = PromptForString(translator, translator.GetTranslation(TranslationString.FirstStartSetupTelegramPasswordPrompt));
            settings.TelegramConfig.TelegramPassword = strInput;
            Logger.Write(translator.GetTranslation(TranslationString.FirstStartSetupTelegramPasswordConfirm, strInput));
        }

        private static void SetupAccountType(ITranslation translator, GlobalSettings settings)
        {
            Logger.Write(translator.GetTranslation(TranslationString.FirstStartSetupAccount), LogLevel.Info);

            string accountType = PromptForString(translator, translator.GetTranslation(TranslationString.FirstStartSetupTypePrompt, "google", "ptc"), new string[] { "google", "ptc" }, translator.GetTranslation(TranslationString.FirstStartSetupTypePromptError, "google", "ptc"), false);
            
            switch (accountType)
            {
                case "google":
                    settings.Auth.AuthConfig.AuthType = AuthType.Google;
                    break;
                case "ptc":
                    settings.Auth.AuthConfig.AuthType = AuthType.Ptc;
                    break;
            }

            Logger.Write(translator.GetTranslation(TranslationString.FirstStartSetupTypeConfirm, accountType.ToUpper()));
        }

        private static void SetupUserAccount(ITranslation translator, GlobalSettings settings)
        {
            Logger.Write("", LogLevel.Info);
            var strInput = PromptForString(translator, translator.GetTranslation(TranslationString.FirstStartSetupUsernamePrompt));
            
            if (settings.Auth.AuthConfig.AuthType == AuthType.Google)
                settings.Auth.AuthConfig.GoogleUsername = strInput;
            else
                settings.Auth.AuthConfig.PtcUsername = strInput;
            Logger.Write(translator.GetTranslation(TranslationString.FirstStartSetupUsernameConfirm, strInput));

            Logger.Write("", LogLevel.Info);
            strInput = PromptForString(translator, translator.GetTranslation(TranslationString.FirstStartSetupPasswordPrompt));
            
            if (settings.Auth.AuthConfig.AuthType == AuthType.Google)
                settings.Auth.AuthConfig.GooglePassword = strInput;
            else
                settings.Auth.AuthConfig.PtcPassword = strInput;
            Logger.Write(translator.GetTranslation(TranslationString.FirstStartSetupPasswordConfirm, strInput));

            Logger.Write(translator.GetTranslation(TranslationString.FirstStartAccountCompleted), LogLevel.Info);
        }

        private static void SetupConfig(ITranslation translator, GlobalSettings settings)
        {
            if (false == PromptForBoolean(translator, translator.GetTranslation(TranslationString.FirstStartDefaultLocationPrompt, "Y", "N")))
            {
                Logger.Write(translator.GetTranslation(TranslationString.FirstStartDefaultLocationSet));
                return;
            }
            
            Logger.Write(translator.GetTranslation(TranslationString.FirstStartDefaultLocation), LogLevel.Info);
            Logger.Write(translator.GetTranslation(TranslationString.FirstStartSetupDefaultLatLongPrompt));
            while (true)
            {
                try
                {
                    var strInput = Console.ReadLine();
                    var strSplit = strInput.Split(',');

                    if (strSplit.Length > 1)
                    {
                        var dblLat = double.Parse(strSplit[0].Trim(' '));
                        var dblLong = double.Parse(strSplit[1].Trim(' '));

                        settings.LocationConfig.DefaultLatitude = dblLat;
                        settings.LocationConfig.DefaultLongitude = dblLong;

                        Logger.Write(translator.GetTranslation(TranslationString.FirstStartSetupDefaultLatLongConfirm,
                            $"{dblLat}, {dblLong}"));
                    }
                    else
                    {
                        Logger.Write(translator.GetTranslation(TranslationString.FirstStartSetupDefaultLocationError,
                            $"{settings.LocationConfig.DefaultLatitude}, {settings.LocationConfig.DefaultLongitude}",
                            LogLevel.Error));
                        continue;
                    }

                    break;
                }
                catch (FormatException)
                {
                    Logger.Write(translator.GetTranslation(TranslationString.FirstStartSetupDefaultLocationError,
                        $"{settings.LocationConfig.DefaultLatitude}, {settings.LocationConfig.DefaultLongitude}",
                        LogLevel.Error));
                }
            }
        }

        public static void SaveFiles(GlobalSettings settings, string configFile)
        {
            settings.Save(configFile);
            settings.Auth.Load(Path.Combine(settings.ProfileConfigPath, "auth.json"), Path.Combine(settings.ProfileConfigPath, "auth.schema.json"), settings.UpdateConfig.SchemaVersion);
        }

        public void Save(string fullPath, bool validate = false)
        {
            var output = JsonConvert.SerializeObject(this, Formatting.Indented,
                new StringEnumConverter {CamelCaseText = true});

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
            Logger.Write("Validating config.json...");
            var jsonObj = JObject.Parse(output);
            IList<ValidationError> errors;
            var valid = jsonObj.IsValid(JsonSchema, out errors);
            if (valid) return;
            foreach (var error in errors)
            {
                Logger.Write(
                    "config.json [Line: " + error.LineNumber + ", Position: " + error.LinePosition + "]: " + error.Path +
                    " " +
                    error.Message, LogLevel.Error);
                //"Default value is '" + error.Schema.Default + "'"
            }
            Logger.Write("Fix config.json and restart NecroBot or press a key to ignore and continue...",
                LogLevel.Warning);
            Console.ReadKey();
        }

        public static bool PromptForBoolean(ITranslation translator, string initialPrompt, string errorPrompt = null)
        {
            while (true)
            {
                Logger.Write(initialPrompt, LogLevel.Info);
                var strInput = Console.ReadLine().ToLower();

                switch (strInput)
                {
                    case "y":
                        return true;
                    case "n":
                        return false;
                    default:
                        if (string.IsNullOrEmpty(errorPrompt))
                            errorPrompt = translator.GetTranslation(TranslationString.PromptError, "y", "n");

                        Logger.Write(errorPrompt, LogLevel.Error);
                        continue;
                }
            }
        }

        public static double PromptForDouble(ITranslation translator, string initialPrompt, string errorPrompt = null)
        {
            while (true)
            {
                Logger.Write(initialPrompt, LogLevel.Info);
                var strInput = Console.ReadLine();

                double doubleVal;
                if (double.TryParse(strInput, out doubleVal))
                {
                    return doubleVal;
                }
                else
                {
                    if (string.IsNullOrEmpty(errorPrompt))
                        errorPrompt = translator.GetTranslation(TranslationString.PromptErrorDouble);

                    Logger.Write(errorPrompt, LogLevel.Error);
                }
            }
        }

        public static int PromptForInteger(ITranslation translator, string initialPrompt, string errorPrompt = null)
        {
            while (true)
            {
                Logger.Write(initialPrompt, LogLevel.Info);
                var strInput = Console.ReadLine();

                int intVal;
                if (int.TryParse(strInput, out intVal))
                {
                    return intVal;
                }
                else
                {
                    if (string.IsNullOrEmpty(errorPrompt))
                        errorPrompt = translator.GetTranslation(TranslationString.PromptErrorInteger);

                    Logger.Write(errorPrompt, LogLevel.Error);
                }
            }
        }

        public static string PromptForString(ITranslation translator, string initialPrompt, string[] validStrings = null, string errorPrompt = null, bool caseSensitive = true)
        {
            while (true)
            {
                Logger.Write(initialPrompt, LogLevel.Info);
                // For now this just reads from the console, but in the future, we may change this to read from the GUI.
                string strInput = Console.ReadLine();

                if (!caseSensitive)
                    strInput = strInput.ToLower();

                // If no valid strings to validate, then return immediately.
                if (validStrings == null)
                    return strInput;

                // Validate string
                foreach (string validString in validStrings)
                {
                    if (String.Equals(strInput, validString, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
                        return strInput;
                }

                // If we got here, no valid strings.
                if (string.IsNullOrEmpty(errorPrompt))
                {
                    errorPrompt = translator.GetTranslation(TranslationString.PromptErrorString, string.Join(",", validStrings));
                }
                Logger.Write(errorPrompt, LogLevel.Error);
            }
        }
    }
}
