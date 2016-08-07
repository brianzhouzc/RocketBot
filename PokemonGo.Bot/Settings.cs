#region

using POGOProtos.Inventory.Item;
using PokemonGo.RocketAPI.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Runtime.CompilerServices;

#endregion

namespace PokemonGo.RocketAPI.Bot
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

        public string TransferType
        {
            get { return GetSetting(); }
            set { SetSetting(value); }
        }

        public int TransferCPThreshold
        {
            get { return TryParseInt(GetSetting()); }
            set { SetSetting(value); }
        }
        public int TransferIVThreshold
        {
            get { return TryParseInt(GetSetting()); }
            set { SetSetting(value); }
        }
        public int TravelSpeed
        {
            get { return TryParseInt(GetSetting()); }
            set { SetSetting(value); }
        }
        public bool EvolveAllGivenPokemons
        {
            get { return TryParseBool(GetSetting()); }
            set { SetSetting(value); }
        }
        public bool CatchPokemon
        {
            get { return TryParseBool(GetSetting()); }
            set { SetSetting(value); }
        }

        public AuthType AuthType
        {
            get { return TryParseEnum<AuthType>(GetSetting()); }
            set { SetSetting<AuthType>(value); }
        }

        public double DefaultLatitude
        {
            get { return TryParseDouble(GetSetting()); }
            set { SetSetting(value); }
        }

        public double DefaultLongitude
        {
            get { return TryParseDouble(GetSetting()); }
            set { SetSetting(value); }
        }

        public string LevelOutput
        {
            get { return GetSetting(); }
            set { SetSetting(value); }
        }

        public int LevelTimeInterval
        {
            get { return TryParseInt(GetSetting()); }
            set { SetSetting(value); }
        }

        public bool Recycler
        {
            get { return TryParseBool(GetSetting()); }
            set { SetSetting(value); }
        }

        private int MaxItemPokeBall
        {
            get { return TryParseInt(GetSetting()); }
            set { SetSetting(value); }
        }

        private int MaxItemGreatBall
        {
            get { return TryParseInt(GetSetting()); }
            set { SetSetting(value); }
        }

        private int MaxItemUltraBall
        {
            get { return TryParseInt(GetSetting()); }
            set { SetSetting(value); }
        }

        private int MaxItemMasterBall
        {
            get { return TryParseInt(GetSetting()); }
            set { SetSetting(value); }
        }

        private int MaxItemRazzBerry
        {
            get { return TryParseInt(GetSetting()); }
            set { SetSetting(value); }
        }

        private int MaxItemRevive
        {
            get { return TryParseInt(GetSetting()); }
            set { SetSetting(value); }
        }

        private int MaxItemPotion
        {
            get { return TryParseInt(GetSetting()); }
            set { SetSetting(value); }
        }

        private int MaxItemSuperPotion
        {
            get { return TryParseInt(GetSetting()); }
            set { SetSetting(value); }
        }

        private int MaxItemHyperPotion
        {
            get { return TryParseInt(GetSetting()); }
            set { SetSetting(value); }
        }

        private int MaxItemMaxPotion
        {
            get { return TryParseInt(GetSetting()); }
            set { SetSetting(value); }
        }

        private ICollection<KeyValuePair<ItemId, int>> ItemRecycleFilter => new[]
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

        public int RecycleItemsInterval
        {
            get { return TryParseInt(GetSetting()); }
            set { SetSetting(value); }
        }

        public string Language
        {
            get { return GetSetting(); }
            set { SetSetting(value); }
        }

        public string RazzBerryMode
        {
            get { return GetSetting(); }
            set { SetSetting(value); }
        }

        public double RazzBerrySetting
        {
            get { return TryParseDouble(GetSetting()); }
            set { SetSetting(value); }
        }

        public string GoogleRefreshToken
        {
            get { return GetSetting(); }
            set { SetSetting(value); }
        }

        public double DefaultAltitude
        {
            get { return TryParseDouble(GetSetting()); }
            set { SetSetting(value); }
        }

        public string GoogleUsername
        {
            get { return GetSetting(); }
            set { SetSetting(value); }
        }

        public string GooglePassword
        {
            get { return GetSetting(); }
            set { SetSetting(value); }
        }

        public string PtcPassword
        {
            get { return GetSetting(); }
            set { SetSetting(value); }
        }

        public string PtcUsername
        {
            get { return GetSetting(); }
            set { SetSetting(value); }
        }

        private static string GetSetting([CallerMemberName] string key = null)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static void SetSetting(string value, [CallerMemberName] string key = null)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (key != null)
            {
                configFile.AppSettings.Settings[key].Value = value;
            }
            configFile.Save(ConfigurationSaveMode.Full);
        }

        public static void SetSetting(double value, [CallerMemberName] string key = null)
            => SetSetting(value.ToString(CultureInfo.InvariantCulture), key);

        public static void SetSetting(int value, [CallerMemberName] string key = null)
            => SetSetting(value.ToString(CultureInfo.InvariantCulture), key);

        public static void SetSetting(bool value, [CallerMemberName] string key = null)
            => SetSetting(value.ToString(CultureInfo.InvariantCulture), key);

        public static void SetSetting<TEnum>(object value, [CallerMemberName] string key = null)
            => SetSetting(Enum.GetName(typeof(TEnum), value), key);

        private static double TryParseDouble(string input)
        {
            var val = 0.0;
            double.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out val);
            return val;
        }

        private static int TryParseInt(string input)
        {
            var val = 0;
            int.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out val);
            return val;
        }

        private static bool TryParseBool(string input)
        {
            var val = false;
            bool.TryParse(input, out val);
            return val;
        }

        private static T TryParseEnum<T>(string input)
            where T:struct
        {
            var val = default(T);
            Enum.TryParse<T>(input, out val);
            return val;
        }
    }
}