using System;
using System.Windows.Forms;
using PoGo.NecroBot.Logic.Logging;

namespace RocketBot2.Helpers
{
    public class VersionHelper
    {
        public static void CheckVersion()
        {           
            try
            {
                Logger.Write("You can find it at https://github.com/TheUnnamedOrganisation/RocketBot/releases", LogLevel.Info, ConsoleColor.Yellow);
                Logger.Write("Your version is " + Application.ProductVersion, LogLevel.Info, ConsoleColor.Magenta);
                Logic.State.VersionCheckState.IsLatest();
                Logger.Write("GitHub version is " + Logic.State.VersionCheckState.RemoteVersion, LogLevel.Update, ConsoleColor.Blue);
            }
            catch (Exception)
            {
                Logger.Write("Unable to check for updates now...", LogLevel.Error);
            }
        }
    }
}