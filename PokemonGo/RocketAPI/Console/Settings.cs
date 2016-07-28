#region

using System.Configuration;
using System.Globalization;
using System.Runtime.CompilerServices;
using PokemonGo.RocketAPI.Enums;
using System.Collections.Generic;
using AllEnum;
using System;

#endregion

namespace PokemonGo.RocketAPI.Console
{
    public class Settings : ISettings
    {
        /// <summary>
        ///     Don't touch. User settings are in Console/App.config
        /// </summary>
        public string TransferType => GetSetting() != string.Empty ? GetSetting() : "none";
        public int TransferCPThreshold => GetSetting() != string.Empty ? int.Parse(GetSetting(), CultureInfo.InvariantCulture) : 0;
        public int TransferIVThreshold => GetSetting() != string.Empty ? int.Parse(GetSetting(), CultureInfo.InvariantCulture) : 0;
        public int TravelSpeed => GetSetting() != string.Empty ? int.Parse(GetSetting(), CultureInfo.InvariantCulture) : 60;
        public bool EvolveAllGivenPokemons => GetSetting() != string.Empty ? System.Convert.ToBoolean(GetSetting(), CultureInfo.InvariantCulture) : false;
        public bool CatchPokemon => GetSetting() != string.Empty ? System.Convert.ToBoolean(GetSetting(), CultureInfo.InvariantCulture) : false;


        public AuthType AuthType => (GetSetting() != string.Empty ? GetSetting() : "Ptc") == "Ptc" ? AuthType.Ptc : AuthType.Google;
        public string PtcUsername => GetSetting() != string.Empty ? GetSetting() : "username";
        public string PtcPassword => GetSetting() != string.Empty ? GetSetting() : "password";

        private double latcheckpoint = -1;
        private double lngcheckpoint = -1;

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

        public double LatitudeCheckPoint
        {
            get
            {
                if (latcheckpoint == -1)
                {
                    return GetSetting() != string.Empty ? double.Parse(GetSetting(), CultureInfo.InvariantCulture) : 51.22640;
                }
                else
                {
                    return latcheckpoint;
                }
            }

            set { latcheckpoint = value; }
        }

        public double LongtitudeCheckPoint
        {
            get
            {
                if (lngcheckpoint == -1)
                {
                    return GetSetting() != string.Empty ? double.Parse(GetSetting(), CultureInfo.InvariantCulture) : 6.77874;
                }
                else
                {
                    return lngcheckpoint;
                }
            }

            set { lngcheckpoint = value; }
        }


        public string LevelOutput => GetSetting() != string.Empty ? GetSetting() : "time";

        public int LevelTimeInterval => GetSetting() != string.Empty ? System.Convert.ToInt16(GetSetting()) : 600;

        public bool Recycler => GetSetting() != string.Empty ? System.Convert.ToBoolean(GetSetting(), CultureInfo.InvariantCulture) : false;

        ICollection<KeyValuePair<ItemId, int>> ISettings.ItemRecycleFilter
        {
            get
            {
                //Type and amount to keep
                return new[]
                {
                    new KeyValuePair<ItemId, int>(ItemId.ItemPokeBall, 20),
                    new KeyValuePair<ItemId, int>(ItemId.ItemGreatBall, 50),
                    new KeyValuePair<ItemId, int>(ItemId.ItemUltraBall, 100),
                    new KeyValuePair<ItemId, int>(ItemId.ItemMasterBall, 200),
                    new KeyValuePair<ItemId, int>(ItemId.ItemRazzBerry, 20),
                    new KeyValuePair<ItemId, int>(ItemId.ItemRevive, 20),
                    new KeyValuePair<ItemId, int>(ItemId.ItemMaxRevive, 40),
                    new KeyValuePair<ItemId, int>(ItemId.ItemPotion, 0),
                    new KeyValuePair<ItemId, int>(ItemId.ItemSuperPotion, 0),
                    new KeyValuePair<ItemId, int>(ItemId.ItemHyperPotion, 50)
                };
            }
        }

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

        private void SetSetting(string value, [CallerMemberName] string key = null)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (key != null) configFile.AppSettings.Settings[key].Value = value;
            configFile.Save();
        }

        private void SetSetting(double value, [CallerMemberName] string key = null)
        {
            CultureInfo customCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (key != null) configFile.AppSettings.Settings[key].Value = value.ToString();
            configFile.Save();
        }
    }
}
