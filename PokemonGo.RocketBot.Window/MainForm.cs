using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PokemonGo.RocketBot.Logic;
using PokemonGo.RocketBot.Logic.Common;
using PokemonGo.RocketBot.Logic.Event;
using PokemonGo.RocketBot.Logic.Logging;
using PokemonGo.RocketBot.Logic.Service;
using PokemonGo.RocketBot.Logic.State;
using PokemonGo.RocketBot.Logic.Tasks;
using PokemonGo.RocketBot.Logic.Utils;
using PokemonGo.RocketBot.Window.Plugin;
using static System.Reflection.Assembly;

namespace PokemonGo.RocketBot.Window
{
    public partial class MainForm : Form
    {
        public static MainForm Instance;
        private static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);
        private static readonly string subPath = "";

        public MainForm()
        {
            InitializeComponent();
            Instance = this;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Logger.SetLogger(new ConsoleLogger(LogLevel.LevelUp), subPath);
            CheckVersion();
            if (boolNeedsSetup())
            {
                startStopBotToolStripMenuItem.Enabled = false;
                Logger.Write("First time here? Go to settings to set your basic info.");
            }
        }

        private async Task InitializePrograme()
        {
            var strCulture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

            var culture = CultureInfo.CreateSpecificCulture("en");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;

            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionEventHandler;

            var logger = new ConsoleLogger(LogLevel.LevelUp);
            Logger.SetLogger(logger, subPath);

            var profilePath = Path.Combine(Directory.GetCurrentDirectory(), subPath);
            var profileConfigPath = Path.Combine(profilePath, "config");
            var configFile = Path.Combine(profileConfigPath, "config.json");

            GlobalSettings settings;
            var boolNeedsSetup = false;

            if (File.Exists(configFile))
            {
                if (!VersionCheckState.IsLatest())
                    settings = GlobalSettings.Load(subPath, true);
                else
                    settings = GlobalSettings.Load(subPath);
            }
            else
            {
                settings = new GlobalSettings();
                settings.ProfilePath = profilePath;
                settings.ProfileConfigPath = profileConfigPath;
                settings.GeneralConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "config");
                settings.TranslationLanguageCode = strCulture;

                boolNeedsSetup = true;
            }


            var session = new Session(new ClientSettings(settings), new LogicSettings(settings));

            if (boolNeedsSetup)
            {
                menuStrip1.ShowItemToolTips = true;
                startStopBotToolStripMenuItem.ToolTipText = "Please goto settings and enter your basic info";
                return;
                /** if (GlobalSettings.PromptForSetup(session.Translation) && !settings.isGui)
                {
                    session = GlobalSettings.SetupSettings(session, settings, configFile);

                    if (!settings.isGui)
                    {
                        var fileName = Assembly.GetExecutingAssembly().Location;
                        System.Diagnostics.Process.Start(fileName);
                        Environment.Exit(0);
                    }
                }
                else
                {
                    GlobalSettings.Load(subPath);

                    Logger.Write("Press a Key to continue...",
                        LogLevel.Warning);
                    Console.ReadKey();
                    return;
                } **/
            }
            //Resources.ProgressBar.start("NecroBot is starting up", 10);

            session.Client.ApiFailure = new ApiFailureStrategy(session);
            //Resources.ProgressBar.fill(20);


            var machine = new StateMachine();
            var stats = new Statistics();

            //Resources.ProgressBar.fill(30);
            var strVersion = GetExecutingAssembly().GetName().Version.ToString(3);
            stats.DirtyEvent +=
                () =>
                    Console.Title = $"[Necrobot v{strVersion}] " +
                                    stats.GetTemplatedStats(
                                        session.Translation.GetTranslation(TranslationString.StatsTemplateString),
                                        session.Translation.GetTranslation(TranslationString.StatsXpTemplateString));
            //Resources.ProgressBar.fill(40);

            var aggregator = new StatisticsAggregator(stats);
            //Resources.ProgressBar.fill(50);
            var listener = new ConsoleEventListener();
            //Resources.ProgressBar.fill(60);

            session.EventDispatcher.EventReceived += evt => listener.Listen(evt, session);
            session.EventDispatcher.EventReceived += evt => aggregator.Listen(evt, session);
            if (settings.UseWebsocket)
            {
                var websocket = new WebSocketInterface(settings.WebSocketPort, session);
                session.EventDispatcher.EventReceived += evt => websocket.Listen(evt, session);
            }

            var plugins = new PluginManager(new PluginInitializerInfo
            {
                Logger = logger,
                Session = session,
                Settings = settings,
                Statistics = stats
            });
            plugins.InitPlugins();

            //Resources.ProgressBar.fill(70);

            machine.SetFailureState(new LoginState());
            //Resources.ProgressBar.fill(80);

            Logger.SetLoggerContext(session);
            //Resources.ProgressBar.fill(90);

            session.Navigation.UpdatePositionEvent +=
                (lat, lng) => session.EventDispatcher.Send(new UpdatePositionEvent {Latitude = lat, Longitude = lng});
            session.Navigation.UpdatePositionEvent += Navigation_UpdatePositionEvent;

            //Resources.ProgressBar.fill(100);

            machine.AsyncStart(new VersionCheckState(), session);

            if (settings.UseTelegramAPI)
            {
                session.Telegram = new TelegramService(settings.TelegramAPIKey, session);
            }

            if (session.LogicSettings.UseSnipeLocationServer)
                SnipePokemonTask.AsyncStart(session);

            try
            {
                Console.Clear();
            }
            catch (IOException)
            {
            }

            settings.checkProxy();

            QuitEvent.WaitOne();
        }

        private static void Navigation_UpdatePositionEvent(double lat, double lng)
        {
            SaveLocationToDisk(lat, lng);
        }

        private static void SaveLocationToDisk(double lat, double lng)
        {
            var coordsPath = Path.Combine(Directory.GetCurrentDirectory(), subPath, "Config", "LastPos.ini");

            File.WriteAllText(coordsPath, $"{lat}:{lng}");
        }

        private static void UnhandledExceptionEventHandler(object obj, UnhandledExceptionEventArgs args)
        {
            Logger.Write("Exception caught, writing LogBuffer.", force: true);
            throw new Exception();
        }

        private void startStopBotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Run(() => InitializePrograme());
        }

        public static void ColoredConsoleWrite(Color color, string text)
        {
            if (text.Length <= 0)
                return;
            var message = text;
            if (Instance.InvokeRequired)
            {
                Instance.Invoke(new Action<Color, string>(ColoredConsoleWrite), color, message);
                return;
            }
            Instance.logTextBox.Select(Instance.logTextBox.Text.Length, 1); // Reset cursor to last
            Instance.logTextBox.SelectionColor = color;
            Instance.logTextBox.AppendText(message);
        }

        private static bool boolNeedsSetup()
        {
            var profilePath = Path.Combine(Directory.GetCurrentDirectory(), subPath);
            var profileConfigPath = Path.Combine(profilePath, "config");
            var configFile = Path.Combine(profileConfigPath, "config.json");
            return !File.Exists(configFile);
        }

        public void CheckVersion()
        {
            try
            {
                var match =
                    new Regex(
                        @"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]")
                        .Match(DownloadServerVersion());

                if (!match.Success) return;
                var gitVersion =
                    new Version(
                        string.Format(
                            "{0}.{1}.{2}.{3}",
                            match.Groups[1],
                            match.Groups[2],
                            match.Groups[3],
                            match.Groups[4]));
                // makes sense to display your version and say what the current one is on github
                Logger.Write("Your version is " + GetExecutingAssembly().GetName().Version);
                Logger.Write("Github version is " + gitVersion);
                Logger.Write("You can find it at www.GitHub.com/TheUnnameOrganization/RocketBot/releases");
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
                        "https://raw.githubusercontent.com/TheUnnameOrganization/RocketBot/Beta-Build/src/RocketBotGUI/Properties/AssemblyInfo.cs");
        }
    }
}