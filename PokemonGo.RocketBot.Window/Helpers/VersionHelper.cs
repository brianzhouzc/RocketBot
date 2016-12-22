using System;
using System.Net;
using System.Windows.Forms;
using PoGo.NecroBot.Logic.Forms_Gui.Logging;
using PoGo.NecroBot.Logic.Forms_Gui.State;
using PoGo.NecroBot.Logic.Forms_Gui.Utils;

namespace PokemonGo.RocketBot.Window.Helpers
{
    public class VersionHelper
    {
        private static readonly Uri StrKillSwitchUri =
     new Uri("https://raw.githubusercontent.com/Necrobot-Private/NecroBot/master/KillSwitch.txt");

        public static void CheckVersion()
        {           
            try
            {
                Logger.Write("You can find it at https://github.com/TheUnnameOrganization/RocketBot/releases");
                Logger.Write("Your version is " + Application.ProductVersion);
     //           VersionCheckState.IsLatest();
                Logger.Write("GitHub version is " + VersionCheckState.RemoteVersion);
            }
            catch (Exception)
            {
                Logger.Write("Unable to check for updates now...", LogLevel.Error);
            }
        }

        public static bool CheckKillSwitch()
        {
            using (var wC = new WebClient())
            {
                try
                {
                    var strResponse = WebClientExtensions.DownloadString(wC, StrKillSwitchUri);

                    if (strResponse == null)
                        return false;

                    var strSplit = strResponse.Split(';');

                    if (strSplit.Length > 1)
                    {
                        var strStatus = strSplit[0];
                        var strReason = strSplit[1];

                        if (strStatus.ToLower().Contains("disable"))
                        {
                            Logger.Write(strReason + $"\n", LogLevel.Warning);
                            Logger.Write("The bot will now close.", LogLevel.Error);
                            return true;
                        }
                    }
                    else
                        return false;
                }
                catch (WebException)
                {
                    // ignored
                }
            }
            return false;
        }
    }
}