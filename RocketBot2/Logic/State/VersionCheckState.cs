#region using directives

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Media;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.Logging;
using PoGo.NecroBot.Logic.Utils;
using PoGo.NecroBot.Logic.Forms;
using PoGo.NecroBot.Logic.State;

#endregion

namespace RocketBot2.Logic.State
{
    public class VersionCheckState : IState
    {
        public const string VersionUri =
            "https://raw.githubusercontent.com/TheUnnamedOrganisation/RocketBot/master/RocketBot2/Properties/AssemblyInfo.cs";

        public const string LatestReleaseApi =
            "https://api.github.com/repos/TheUnnamedOrganisation/RocketBot/releases/latest";

        public static Version RemoteVersion;

        public async Task<IState> Execute(ISession session, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await CleanupOldFiles();

            if (!session.LogicSettings.CheckForUpdates)
            {
                session.EventDispatcher.Send(new UpdateEvent
                {
                    Message = session.Translation.GetTranslation(TranslationString.CheckForUpdatesDisabled,
                        Assembly.GetExecutingAssembly().GetName().Version.ToString(3))
                });

                return new LoginState();
            }

            var autoUpdate = session.LogicSettings.AutoUpdate;
            var isLatest = IsLatest();
            if (isLatest)
            {
                session.EventDispatcher.Send(new UpdateEvent
                {
                    Message =
                        session.Translation.GetTranslation(TranslationString.GotUpToDateVersion, Assembly.GetExecutingAssembly().GetName().Version.ToString(4))
                });
                return new LoginState();
            }

            SystemSounds.Asterisk.Play();

            var remoteReleaseUrl =
                $"https://github.com/TheUnnamedOrganisation/RocketBot/releases/download/v{RemoteVersion}/";
            string zipName = "RocketBot2.zip";
            var downloadLink = remoteReleaseUrl + zipName;

            var baseDir = Directory.GetCurrentDirectory();
            var downloadFilePath = Path.Combine(baseDir, zipName);
            var tempPath = Path.Combine(baseDir, "tmp");
            var extractedDir = Path.Combine(tempPath, "RocketBot2");
            var destinationDir = baseDir + Path.DirectorySeparatorChar;
            bool updated = false;
            AutoUpdateForm autoUpdateForm = new AutoUpdateForm()
            {
                Session = session,
                DownloadLink = downloadLink,
                Destination = downloadFilePath,
                AutoUpdate = true,
                CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                LatestVersion = $"{RemoteVersion}"
            };

            updated = (autoUpdateForm.ShowDialog() == DialogResult.OK);


            if (!updated)
            {
                Logger.Write("Update Skipped", LogLevel.Update);
                return new LoginState();

            }

            if (!UnpackFile(downloadFilePath, extractedDir))
                return new LoginState();

            session.EventDispatcher.Send(new UpdateEvent
            {
                Message = session.Translation.GetTranslation(TranslationString.FinishedUnpackingFiles)
            });

            if (!MoveAllFiles(extractedDir, destinationDir))
                return new LoginState();

            session.EventDispatcher.Send(new UpdateEvent
            {
                Message = session.Translation.GetTranslation(TranslationString.UpdateFinished)
            });

            Process.Start(Assembly.GetEntryAssembly().Location);
            Environment.Exit(-1);
            return null;
        }

        public static async Task CleanupOldFiles()
        {
            var tmpDir = Path.Combine(Directory.GetCurrentDirectory(), "tmp");

            if (Directory.Exists(tmpDir))
            {
                Directory.Delete(tmpDir, true);
            }

            var di = new DirectoryInfo(Directory.GetCurrentDirectory());
            var files = di.GetFiles("*.old", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                try
                {
                    if (file.Name.Contains("vshost") || file.Name.Contains(".gpx.old") || file.Name.Contains("chromedriver.exe.old"))
                        continue;
                    File.Delete(file.FullName);
                }
                catch (Exception e)
                {
                    Logger.Write(e.ToString());
                }
            }
            await Task.Delay(200);
        }

       

        private static string DownloadServerVersion()
        {
            using (var wC = new NecroWebClient())
            {
                return wC.DownloadString(VersionUri);
            }
        }

        private static JObject GetJObject(string filePath)
        {
            return JObject.Parse(File.ReadAllText(filePath));
        }


        public static bool IsLatest()
        {
            try
            {
                var regex = new Regex(@"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]");
                var match = regex.Match(DownloadServerVersion());

                if (!match.Success)
                    return false;

                var gitVersion = new Version($"{match.Groups[1]}.{match.Groups[2]}.{match.Groups[3]}.{match.Groups[4]}");
                RemoteVersion = gitVersion;
                if (gitVersion > Assembly.GetExecutingAssembly().GetName().Version)
                    return false;
            }
            catch (Exception)
            {
                return true; //better than just doing nothing when git server down
            }

            return true;
        }

        public static bool MoveAllFiles(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            var oldfiles = Directory.GetFiles(destFolder);
            foreach (var old in oldfiles)
            {
                if (old.Contains("vshost") || old.Contains(".gpx") || old.Contains("config.json") ||
                    old.Contains("config.xlsm") || old.Contains("auth.json") || old.Contains("SessionStats.db") ||
                    old.Contains("LastPos.ini") || old.Contains("chromedriver.exe") || old.Contains("accounts.db")) continue;
                if (File.Exists(old + ".old")) continue;
                File.Move(old, old + ".old");
            }

            try
            {
                var files = Directory.GetFiles(sourceFolder);
                foreach (var file in files)
                {
                    if (file.Contains("vshost") || file.Contains(".gpx")) continue;
                    var name = Path.GetFileName(file);
                    var dest = Path.Combine(destFolder, name);
                    try {
                        File.Copy(file, dest, true);
                    }
                    catch(Exception )
                    {
                        Logger.Write($"Error occurred while copy {file}, This seem like chromedriver.exe is being locked, you need manually copy after you close all chrome instance or ignore it");
                    }
                }

                var folders = Directory.GetDirectories(sourceFolder);

                foreach (var folder in folders)
                {
                    var name = Path.GetFileName(folder);
                    if (name == null) continue;
                    var dest = Path.Combine(destFolder, name);
                    MoveAllFiles(folder, dest);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool UnpackFile(string sourceTarget, string destPath)
        {
            var source = sourceTarget;
            var dest = destPath;
            try
            {
                ZipFile.ExtractToDirectory(source, dest);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}