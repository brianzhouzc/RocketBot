#region

using System.Configuration;
using System.Globalization;
using System.Runtime.CompilerServices;
using PokemonGo.RocketAPI.Enums;

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
        public bool EvolveAllGivenPokemons => GetSetting() != string.Empty ? System.Convert.ToBoolean(GetSetting(), CultureInfo.InvariantCulture) : false;


        public AuthType AuthType => (GetSetting() != string.Empty ? GetSetting() : "Ptc") == "Ptc" ? AuthType.Ptc : AuthType.Google;
        public string PtcUsername => GetSetting() != string.Empty ? GetSetting() : "username";
        public string PtcPassword => GetSetting() != string.Empty ? GetSetting() : "password";

        public double DefaultLatitude => GetSetting() != string.Empty ? double.Parse(GetSetting(), CultureInfo.InvariantCulture) : 51.22640;

        //Default Amsterdam Central Station if another location is not specified
        public double DefaultLongitude => GetSetting() != string.Empty ? double.Parse(GetSetting(), CultureInfo.InvariantCulture) : 6.77874;

        //Default Amsterdam Central Station if another location is not specified

        // LEAVE EVERYTHING ALONE


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
    }
}
