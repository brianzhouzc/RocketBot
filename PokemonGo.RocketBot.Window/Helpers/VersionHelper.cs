using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PokemonGo.RocketBot.Logic.Logging;

namespace PokemonGo.RocketBot.Window.Helpers
{
    public class VersionHelper
    {
        public static void CheckVersion()
        {
            try
            {
                Logger.Write("You can find it at www.GitHub.com/TheUnnameOrganization/RocketBot/releases",LogLevel.Update);
                Logger.Write("Your version is " + Application.ProductVersion);
                var match =
                    new Regex(
                       @"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]")
                        .Match(DownloadServerVersion());

                if (!match.Success) return;
                var gitVersion =
                    new Version(
                        $"{match.Groups[1]}.{match.Groups[2]}.{match.Groups[3]}.{match.Groups[4]}");
                // makes sense to display your version and say what the current one is on github
                Logger.Write("Github version is " + gitVersion);
            }
            catch (Exception)
            {
                Logger.Write("Unable to check for updates now...", LogLevel.Error);
            }
        }

        private static string DownloadServerVersion()
        {
            using (var wC = new WebClient())
                return
                    wC.DownloadString(
                        "https://raw.githubusercontent.com/TheUnnameOrganization/RocketBot/master/PokemonGo.RocketBot.Window/Properties/AssemblyInfo.cs");
        }
    }
}