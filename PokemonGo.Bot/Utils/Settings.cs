using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PokemonGo.Bot.TransferPokemonAlgorithms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using POGOProtos.Settings;
using POGOProtos.Networking.Responses;

namespace PokemonGo.Bot.Utils
{
    public class Settings : ViewModelBase
    {
        public Settings()
        {
            Map = new MapSettings();
            Level = new LevelSettings();
            Fort = new FortSettings();
            Inventory = new InventorySettings();

            Save = new AsyncRelayCommand(SaveToFileAsync);
        }
        public AuthType AuthType { get; set; }

        public double DefaultAltitude { get; set; }

        public double DefaultLatitude { get; set; }

        public double DefaultLongitude { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public ICollection<int> PokemonToNotTransferWithAlgorithm { get; set; }

        public int? DoNotTransferPokemonAboveCP { get; set; }

        public int? DoNotTransferPokemonAboveIV { get; set; }

        public TransferPokemonAlgorithm TransferType { get; set; }

        public int UseRazzBerryWhenPokemonIsAboveCP { get; set; }
        public double UseRazzBerryWhenCatchProbabilityIsBelow { get; set; }

        public double MinTravelSpeedInKmH { get; set; }
        public double MaxTravelSpeedInKmH { get; set; }

        public bool AutoManageInventory { get; set; }
        public int MinRevive { get; set; }
        public int MinPokeballs { get; set; }
        public int MinPotions { get; set; }
        public int MinBerries { get; set; }

        MapSettings map;

        [IgnoreDataMember]
        public MapSettings Map
        {
            get { return map; }
            set { if (Map != value) { map = value; RaisePropertyChanged(); } }
        }

        PokemonUpgradeSettings pokemonUpgrade;

        [IgnoreDataMember]
        public PokemonUpgradeSettings PokemonUpgrade
        {
            get { return pokemonUpgrade; }
            set { if (PokemonUpgrade != value) { pokemonUpgrade = value; RaisePropertyChanged(); } }
        }

        FortSettings fort;
        [IgnoreDataMember]
        public FortSettings Fort
        {
            get { return fort; }
            set { if (Fort != value) { fort = value; RaisePropertyChanged(); } }
        }

        InventorySettings inventory;
        [IgnoreDataMember]
        public InventorySettings Inventory
        {
            get { return inventory; }
            set { if (Inventory != value) { inventory = value; RaisePropertyChanged(); } }
        }

        LevelSettings level;
        [IgnoreDataMember]
        public LevelSettings Level
        {
            get { return level; }
            set { if (Level != value) { level = value; RaisePropertyChanged(); } }
        }

        private string loadedFromFile;

        internal void UpdateWith(GlobalSettings globalSettings)
        {
            if(globalSettings.FortSettings != null)
                Fort = new PokemonGo.Bot.Utils.FortSettings(globalSettings.FortSettings);
            if (globalSettings.InventorySettings != null)
                Inventory = new PokemonGo.Bot.Utils.InventorySettings(globalSettings.InventorySettings);
            if (globalSettings.LevelSettings != null)
                Level = new PokemonGo.Bot.Utils.LevelSettings(globalSettings.LevelSettings);
            if (globalSettings.MapSettings != null)
                Map = new PokemonGo.Bot.Utils.MapSettings(globalSettings.MapSettings);
        }

        internal void UpdateWith(DownloadItemTemplatesResponse templates)
        {
            var itemTemplates = templates.ItemTemplates;
            var pokemonUpgradeTemplate = itemTemplates.SingleOrDefault(t => t.PokemonUpgrades != null)?.PokemonUpgrades;
            if (pokemonUpgradeTemplate != null)
            {
                PokemonUpgrade = new PokemonUpgradeSettings(pokemonUpgradeTemplate);
            }
            var pokemonTemplate = itemTemplates.Where(t => t.PokemonSettings != null)?.Select(t => t.PokemonSettings);
            pokemon = pokemonTemplate.ToDictionary(p => (int)p.PokemonId, p => new PokemonSettings(p));
            var playerLevelTemplate = itemTemplates.SingleOrDefault(t => t.PlayerLevel != null)?.PlayerLevel;
            if (playerLevelTemplate != null)
            {
                PlayerLevel = new PlayerLevelSettings(playerLevelTemplate);
            }
        }

        IDictionary<int, PokemonSettings> pokemon;
        public PokemonSettings GetPokemonSettings(int pokemonId)
        {
            PokemonSettings pokemonSettings;
            if (pokemon.TryGetValue(pokemonId, out pokemonSettings))
                return pokemonSettings;
            return new PokemonSettings();
        }

        PlayerLevelSettings playerLevel;
        [IgnoreDataMember]
        public PlayerLevelSettings PlayerLevel
        {
            get { return playerLevel; }
            set { if (PlayerLevel != value) { playerLevel = value; RaisePropertyChanged(); } }
        }



        public static Settings LoadFromJson(string json) => JsonConvert.DeserializeObject<Settings>(json);
        public static Settings LoadFromFile(string file)
        {
            Settings settings;
            try
            {
                using (var reader = File.OpenText(file))
                {
                    var json = reader.ReadToEnd();
                    settings = LoadFromJson(json);
                }
            }
            catch (Exception)
            {
                settings = new Settings();
            }
            settings.loadedFromFile = file;
            return settings;
        }
        public static async Task<Settings> LoadFromFileAsync(string file)
        {
            Settings settings;
            try
            {
                using (var reader = File.OpenText(file))
                {
                    var json = await reader.ReadToEndAsync();
                    settings = LoadFromJson(json);
                }
            }
            catch (Exception)
            {
                settings = new Settings();
            }
            settings.loadedFromFile = file;
            return settings;
        }

        [IgnoreDataMember]
        public AsyncRelayCommand Save { get; }

        public string SaveToJson() => JsonConvert.SerializeObject(this, Formatting.Indented, new StringEnumConverter());

        public async Task SaveToFileAsync()
        {
            var json = SaveToJson();
            var buffer = Encoding.UTF8.GetBytes(json);
            using (var stream = new FileStream(loadedFromFile, FileMode.Create))
            {
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }
        }
    }
}