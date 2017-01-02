using System;
using System.Net;
using System.Windows.Forms;
using PoGo.NecroBot.Logic.Logging;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Utils;

namespace NecroBot2.Helpers
{
    public class VersionHelper
    {
        public static void CheckVersion()
        {           
            try
            {
                Logger.Write("You can find it at https://github.com/Furtif/NecroBot/releases");
                Logger.Write("Your version is " + Application.ProductVersion);
                Logic.State.VersionCheckState.IsLatest();
                Logger.Write("GitHub version is " + Logic.State.VersionCheckState.RemoteVersion);
            }
            catch (Exception)
            {
                Logger.Write("Unable to check for updates now...", LogLevel.Error);
            }
        }
    }
}