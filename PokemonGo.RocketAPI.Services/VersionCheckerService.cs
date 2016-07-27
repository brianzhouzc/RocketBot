using System;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PokemonGo.RocketAPI.Services
{
    public class VersionCheckerService : IVersionCheckerService
    {
        private readonly IConsoleWriterService _console;
        private readonly string _assemblyInfoUrl;

        public VersionCheckerService(
            IConsoleWriterService consoleWriterService, 
            string assemblyInfoUrl)
        {
            _console = consoleWriterService;
            _assemblyInfoUrl = assemblyInfoUrl;
        }

        public void CheckVersion()
        {
            try
            {
                var serverVersion = DownloadServerVersion();
                var regex = new Regex(@"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]");
                var match = regex.Match(serverVersion);

                if (!match.Success)
                {
                    return;
                }

                var gitVersionString = string.Format(
                    "{0}.{1}.{2}.{3}",
                    match.Groups[1],
                    match.Groups[2],
                    match.Groups[3],
                    match.Groups[4]);

                var gitVersion = new Version(gitVersionString);

                if (gitVersion <= Assembly.GetEntryAssembly().GetName().Version)
                {
                    _console.Write(ConsoleColor.Green, "Awesome! You have already got the newest version! " + Assembly.GetExecutingAssembly().GetName().Version);
                    return;
                }
                else
                {
                    // makes sense to display your version and say what the current one is on github
                    _console.Write(ConsoleColor.Red, "Your version is " + Assembly.GetEntryAssembly().GetName().Version);
                    _console.Write(ConsoleColor.Red, "Github version is " + gitVersion);
                    _console.Write(ConsoleColor.Red, "You can find it at https://github.com/DetectiveSquirrel/Pokemon-Go-Rocket-API");
                }
            }
            catch (Exception)
            {
                _console.Write(ConsoleColor.Red, "Unable to check for updates now...");
            }
        }

        private string DownloadServerVersion()
        {
            using (var client = new WebClient())
            {
                return client.DownloadString(_assemblyInfoUrl);
            }
        }
    }
}
