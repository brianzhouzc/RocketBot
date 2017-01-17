#region using directives

using System;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.Logging;
using PoGo.NecroBot.Logic.Utils;
using PoGo.NecroBot.Logic.State;
using System.Windows.Forms;

#endregion

namespace RocketBot2.Logic.State
{
    public class VersionCheckState : IState
    {
        public const string VersionUri =
            "https://raw.githubusercontent.com/TheUnnamedOrganisation/RocketBot/RockeBot2/Properties/AssemblyInfo.cs";

        public const string LatestReleaseApi =
            "https://api.github.com/repos/TheUnnamedOrganisation/RocketBot/releases/latest";

        public static Version RemoteVersion;

        public async Task<IState> Execute(ISession session, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await CleanupOldFiles();

            if( !session.LogicSettings.CheckForUpdates )
            {
                session.EventDispatcher.Send( new UpdateEvent
                {
                    Message = session.Translation.GetTranslation( TranslationString.CheckForUpdatesDisabled, Assembly.GetExecutingAssembly().GetName().Version.ToString( 3 ) )
                } );

                return new LoginState();
            }

            var autoUpdate = session.LogicSettings.AutoUpdate;
            var isLatest = IsLatest();
            if ( isLatest )
            {
                session.EventDispatcher.Send(new UpdateEvent
                {
                    Message =
                        session.Translation.GetTranslation(TranslationString.GotUpToDateVersion, Assembly.GetExecutingAssembly().GetName().Version.ToString(4))
                });
                return new LoginState();
            }
            
            if ( !autoUpdate )
            {
                Logger.Write( "New update detected, would you like to update? Y/N", LogLevel.Update );

                /*
                var boolBreak = false;
                while( !boolBreak )
                {
                    var strInput = Console.ReadLine().ToLower();

                    switch( strInput )
                    {
                        case "y":
                            boolBreak = true;
                            break;
                        case "n":
                            Logger.Write( "Update Skipped", LogLevel.Update );
                            return new LoginState();
                        default:
                            Logger.Write( session.Translation.GetTranslation( TranslationString.PromptError, "Y", "N" ), LogLevel.Error );
                            continue;
                    }
                }
                */
                DialogResult result = MessageBox.Show("New update detected, would you like to update? Y/N", Application.ProductName + " - Update", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                switch (result)
                {
                    case DialogResult.Yes: { break; }
                    case DialogResult.No:
                        {
                            Logger.Write("Update Skipped", LogLevel.Update);
                            return new LoginState();
                        }
                }
            }

            session.EventDispatcher.Send(new UpdateEvent
            {
                Message = session.Translation.GetTranslation(TranslationString.DownloadingUpdate)
            });
            var remoteReleaseUrl =
                $"https://github.com/TheUnnamedOrganisation/RocketBot/releases/download/v{RemoteVersion}/";
            const string zipName = "Release.zip";
            var downloadLink = remoteReleaseUrl + zipName;
            var baseDir = Directory.GetCurrentDirectory();
            var downloadFilePath = Path.Combine(baseDir, zipName);
            var tempPath = Path.Combine(baseDir, "tmp");
            var extractedDir = Path.Combine(tempPath, "Release");
            var destinationDir = baseDir + Path.DirectorySeparatorChar;
            Logger.Write(downloadLink, LogLevel.Info);

            if (!DownloadFile(downloadLink, downloadFilePath))
                return new LoginState();

            session.EventDispatcher.Send(new UpdateEvent
            {
                Message = session.Translation.GetTranslation(TranslationString.FinishedDownloadingRelease)
            });

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
                    if (file.Name.Contains("vshost") || file.Name.Contains(".gpx.old"))
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

        public static bool DownloadFile(string url, string dest)
        {
            using (var client = new WebClient())
            {
                try
                {
                    client.DownloadFile(url, dest);
                    Logger.Write(dest, LogLevel.Info);
                }
                catch
                {
                    // ignored
                }
                return true;
            }
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
                if (old.Contains("vshost") || old.Contains(".gpx") || old.Contains("config.json") || old.Contains("config.xlsm") || old.Contains("auth.json") ||  old.Contains("SessionStats.db") || old.Contains("LastPos.ini")) continue;
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
                    File.Copy(file, dest, true);
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