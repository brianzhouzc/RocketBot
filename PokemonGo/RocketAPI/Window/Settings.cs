#region

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using AllEnum;
using PokemonGo.RocketAPI.Enums;

#endregion

namespace PokemonGo.RocketAPI.Window
{
    public class Settings : ISettings
    {
        private static volatile Settings _instance;
        private static readonly object SyncRoot = new object();

        public static Settings Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (SyncRoot)
                {
                    if (_instance == null)
                        _instance = new Settings();
                }

                return _instance;
            }
        }

        public void Reload()
        {
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        ///     Don't touch. User settings are in Console/App.config
        /// </summary>
        public string TransferType => GetSetting() != string.Empty ? GetSetting() : "none";
        public int TransferCPThreshold => GetSetting() != string.Empty ? int.Parse(GetSetting(), CultureInfo.InvariantCulture) : 0;
        public bool EvolveAllGivenPokemons => GetSetting() != string.Empty && Convert.ToBoolean(GetSetting(), CultureInfo.InvariantCulture);


        public AuthType AuthType
        {
            get
            {
                return (GetSetting() != string.Empty ? GetSetting() : "Ptc") == "Ptc" ? AuthType.Ptc : AuthType.Google;
            }
            set { SetSetting(value.ToString()); }
        }

        public string PtcUsername => GetSetting() != string.Empty ? GetSetting() : "username";
        public string PtcPassword => GetSetting() != string.Empty ? GetSetting() : "password";

        public double DefaultLatitude
        {
            get { return GetSetting() != string.Empty ? double.Parse(GetSetting(), CultureInfo.InvariantCulture) : 51.22640; }
            set { SetSetting(value); }
        }


        public double DefaultLongitude
        {
            get { return GetSetting() != string.Empty ? double.Parse(GetSetting(), CultureInfo.InvariantCulture) : 6.77874; }
            set { SetSetting(value); }
        }


        public string LevelOutput => GetSetting() != string.Empty ? GetSetting() : "time";

        public int LevelTimeInterval => GetSetting() != string.Empty ? Convert.ToInt16(GetSetting()) : 600;

        public bool Recycler => GetSetting() != string.Empty && Convert.ToBoolean(GetSetting(), CultureInfo.InvariantCulture);

        ICollection<KeyValuePair<ItemId, int>> ISettings.ItemRecycleFilter => new[]
        {
            new KeyValuePair<ItemId, int>(ItemId.ItemPokeBall, 20),
            new KeyValuePair<ItemId, int>(ItemId.ItemGreatBall, 50),
            new KeyValuePair<ItemId, int>(ItemId.ItemUltraBall, 100),
            new KeyValuePair<ItemId, int>(ItemId.ItemMasterBall, 200),
            new KeyValuePair<ItemId, int>(ItemId.ItemRazzBerry, 20),
            new KeyValuePair<ItemId, int>(ItemId.ItemRevive, 20),
            new KeyValuePair<ItemId, int>(ItemId.ItemPotion, 0),
            new KeyValuePair<ItemId, int>(ItemId.ItemSuperPotion, 0),
            new KeyValuePair<ItemId, int>(ItemId.ItemHyperPotion, 50),
            new KeyValuePair<ItemId, int>(ItemId.ItemMaxPotion, 100)
        };

        public int RecycleItemsInterval => GetSetting() != string.Empty ? Convert.ToInt16(GetSetting()) : 60;

        public string Language => GetSetting() != string.Empty ? GetSetting() : "english";

        public string RazzBerryMode => GetSetting() != string.Empty ? GetSetting() : "cp";

        public double RazzBerrySetting => GetSetting() != string.Empty ? double.Parse(GetSetting(), CultureInfo.InvariantCulture) : 500;

        public string GoogleRefreshToken
        {
            get { return GetSetting() != string.Empty ? GetSetting() : string.Empty; }
            set { SetSetting(value); }
        }

        private string GetSetting([CallerMemberName] string key = null)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public void SetSetting(string value, [CallerMemberName] string key = null)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (key != null)
            {
                configFile.AppSettings.Settings[key].Value = value;
            }
            configFile.Save(ConfigurationSaveMode.Full);
        }

        public void SetSetting(double value, [CallerMemberName] string key = null)
        {
            CultureInfo customCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (key != null) configFile.AppSettings.Settings[key].Value = value.ToString();
            configFile.Save(ConfigurationSaveMode.Full);
        }
    }
}
