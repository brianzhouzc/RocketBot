#region

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
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
        public int TransferIVThreshold => GetSetting() != string.Empty ? int.Parse(GetSetting(), CultureInfo.InvariantCulture) : 0;
        public int TravelSpeed => GetSetting() != string.Empty ? int.Parse(GetSetting(), CultureInfo.InvariantCulture) : 60;
        public int ImageSize => GetSetting() != string.Empty ? int.Parse(GetSetting(), CultureInfo.InvariantCulture) : 50;
        public bool EvolveAllGivenPokemons => GetSetting() != string.Empty && Convert.ToBoolean(GetSetting(), CultureInfo.InvariantCulture);
        public bool CatchPokemon => GetSetting() != string.Empty && Convert.ToBoolean(GetSetting(), CultureInfo.InvariantCulture);


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
        public string Email => GetSetting() != string.Empty ? GetSetting() : "Email";
        public string Password => GetSetting() != string.Empty ? GetSetting() : "Password";

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

        private int MaxItemPokeBall => GetSetting() != string.Empty ? Convert.ToInt16(GetSetting()) : 500;
        private int MaxItemGreatBall => GetSetting() != string.Empty ? Convert.ToInt16(GetSetting()) : 500;
        private int MaxItemUltraBall => GetSetting() != string.Empty ? Convert.ToInt16(GetSetting()) : 500;
        private int MaxItemMasterBall => GetSetting() != string.Empty ? Convert.ToInt16(GetSetting()) : 500;
        private int MaxItemRazzBerry => GetSetting() != string.Empty ? Convert.ToInt16(GetSetting()) : 500;
        private int MaxItemRevive => GetSetting() != string.Empty ? Convert.ToInt16(GetSetting()) : 500;
        private int MaxItemPotion => GetSetting() != string.Empty ? Convert.ToInt16(GetSetting()) : 500;
        private int MaxItemSuperPotion => GetSetting() != string.Empty ? Convert.ToInt16(GetSetting()) : 500;
        private int MaxItemHyperPotion => GetSetting() != string.Empty ? Convert.ToInt16(GetSetting()) : 500;
        private int MaxItemMaxPotion => GetSetting() != string.Empty ? Convert.ToInt16(GetSetting()) : 500;

        ICollection<KeyValuePair<ItemId, int>> ISettings.ItemRecycleFilter => new[]
        {
            new KeyValuePair<ItemId, int>(ItemId.ItemPokeBall, MaxItemPokeBall),
            new KeyValuePair<ItemId, int>(ItemId.ItemGreatBall, MaxItemGreatBall),
            new KeyValuePair<ItemId, int>(ItemId.ItemUltraBall, MaxItemUltraBall),
            new KeyValuePair<ItemId, int>(ItemId.ItemMasterBall, MaxItemMasterBall),
            new KeyValuePair<ItemId, int>(ItemId.ItemRazzBerry, MaxItemRazzBerry),
            new KeyValuePair<ItemId, int>(ItemId.ItemRevive, MaxItemRevive),
            new KeyValuePair<ItemId, int>(ItemId.ItemPotion, MaxItemPotion),
            new KeyValuePair<ItemId, int>(ItemId.ItemSuperPotion, MaxItemSuperPotion),
            new KeyValuePair<ItemId, int>(ItemId.ItemHyperPotion, MaxItemHyperPotion),
            new KeyValuePair<ItemId, int>(ItemId.ItemMaxPotion, MaxItemMaxPotion)
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
            CultureInfo customCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = customCulture;
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (key != null) configFile.AppSettings.Settings[key].Value = value.ToString();
            configFile.Save(ConfigurationSaveMode.Full);
        }
    }
}
