using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PokemonGo.Bot.TransferPokemonAlgorithms;
using PokemonGo.Bot.ViewModels;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.Bot.Utils
{
    public class Settings : ViewModelBase, ISettings
    {
        public AuthType AuthType { get; set; }

        public double DefaultAltitude { get; set; }

        public double DefaultLatitude { get; set; }

        public double DefaultLongitude { get; set; }

        [IgnoreDataMember]
        public string GooglePassword { get { return Password; } set { throw new InvalidOperationException($"Use {nameof(Password)}."); } }

        [IgnoreDataMember]
        public string GoogleRefreshToken { get; set; }

        [IgnoreDataMember]
        public string GoogleUsername { get { return Username; } set { throw new InvalidOperationException($"Use {nameof(Username)}."); } }

        [IgnoreDataMember]
        public string PtcPassword { get { return Password; } set { throw new InvalidOperationException($"Use {nameof(Password)}."); } }

        [IgnoreDataMember]
        public string PtcUsername { get { return Username; } set { throw new InvalidOperationException($"Use {nameof(Username)}."); } }

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

        [IgnoreDataMember]
        internal Client Client { get; set; }

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

        RocketAPI.Enums.AuthType ISettings.AuthType { get { return (RocketAPI.Enums.AuthType)AuthType; }  set { throw new InvalidOperationException($"use {nameof(AuthType)} from Settings class."); } }

        private string loadedFromFile;
        public Settings()
        {
            Save = new AsyncRelayCommand(SaveToFileAsync);
        }
        public async Task DownloadSettingsAsync()
        {
            var response = await Client.Download.GetSettings();
            if (response.Settings != null)
            {
                var forstSettings = response.Settings.FortSettings;
                var inventorySettings = response.Settings.InventorySettings;
                var levelSettings = response.Settings.LevelSettings;
                Map = new PokemonGo.Bot.Utils.MapSettings(response.Settings.MapSettings);
            }
        }

        public async Task DownloadItemTemplatesAsync()
        {
            var response = await Client.Download.GetItemTemplates();
            if (response.Success)
            {
                var itemTemplates = response.ItemTemplates;
                var pokemonUpgradeTemplate = response.ItemTemplates.SingleOrDefault(t => t.PokemonUpgrades != null)?.PokemonUpgrades;
                if (pokemonUpgradeTemplate != null)
                {
                    PokemonUpgrade = new PokemonUpgradeSettings(pokemonUpgradeTemplate);
                }
            }
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