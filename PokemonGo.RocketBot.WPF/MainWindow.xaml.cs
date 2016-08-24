using System.Windows;
using System.Windows.Input;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using PokemonGo.RocketBot.Logic;
using PokemonGo.RocketBot.Logic.Common;
using PokemonGo.RocketBot.Logic.Event;
using PokemonGo.RocketBot.Logic.Logging;
using PokemonGo.RocketBot.Logic.Service;
using PokemonGo.RocketBot.Logic.State;
using PokemonGo.RocketBot.Logic.Tasks;
using PokemonGo.RocketBot.Logic.Utils;
using PokemonGo.RocketBot.WPF.Plugin;
using Color = System.Drawing.Color;

namespace PokemonGo.RocketBot.WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public static MainWindow Instance;
        public static SynchronizationContext SynchronizationContext;
        private static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);
        private static readonly string subPath = "";
        private static Session _session;
        public static bool BoolNeedsSetup;
        private static GMapMarker _playerMarker;
        private readonly List<PointLatLng> _playerLocations = new List<PointLatLng>();

        private readonly GMapOverlay _playerOverlay = new GMapOverlay("players");
        private readonly GMapOverlay _playerRouteOverlay = new GMapOverlay("playerroutes");
        private readonly GMapOverlay _pokemonsOverlay = new GMapOverlay("pokemons");
        private readonly GMapOverlay _pokestopsOverlay = new GMapOverlay("pokestops");
        private readonly GMapOverlay _searchAreaOverlay = new GMapOverlay("areas");

        private PointLatLng _currentLatLng;
        private ConsoleLogger _logger;
        private StateMachine _machine;
        private List<PointLatLng> _routePoints;
        private GlobalSettings _settings;


        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeBot();
            GlobalSettings.Load("");
            Logger.Write("Loaded");
        }

        private void InitializeBot()
        {
            var strCulture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

            var culture = CultureInfo.CreateSpecificCulture("en");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;

            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionEventHandler;

            _logger = new ConsoleLogger(LogLevel.LevelUp);
            Logger.SetLogger(_logger, subPath);

            var profilePath = Path.Combine(Directory.GetCurrentDirectory(), subPath);
            var profileConfigPath = Path.Combine(profilePath, "config");
            var authFile = Path.Combine(profileConfigPath, "auth.json");
            var configFile = Path.Combine(profileConfigPath, "config.json");

            BoolNeedsSetup = false;

            if (File.Exists(configFile))
            {
                /** if (!VersionCheckState.IsLatest())
                    settings = GlobalSettings.Load(subPath, true);
                else **/
                _settings = GlobalSettings.Load(subPath, true);
                _settings.Auth.Load(authFile);
            }
            else
            {
                _settings = new GlobalSettings
                {
                    ProfilePath = profilePath,
                    ProfileConfigPath = profileConfigPath,
                    GeneralConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "config"),
                    TranslationLanguageCode = strCulture
                };
                BoolNeedsSetup = true;
            }

            _session = new Session(new ClientSettings(_settings), new LogicSettings(_settings));

            _session.Client.ApiFailure = new ApiFailureStrategy(_session);

            _machine = new StateMachine();
            var stats = new Statistics();

            // var strVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(3); NOT USED ATM

            //Status bar
            /*stats.DirtyEvent +=
                () =>
                    SetStatusText(stats.GetTemplatedStats(
                        _session.Translation.GetTranslation(TranslationString.StatsTemplateString),
                        _session.Translation.GetTranslation(TranslationString.StatsXpTemplateString))); */

            var aggregator = new StatisticsAggregator(stats);
            var listener = new ConsoleEventListener();

            _session.EventDispatcher.EventReceived += evt => listener.Listen(evt, _session);
            _session.EventDispatcher.EventReceived += evt => aggregator.Listen(evt, _session);

            if (_settings.UseWebsocket)
            {
                var websocket = new WebSocketInterface(_settings.WebSocketPort, _session);
                _session.EventDispatcher.EventReceived += evt => websocket.Listen(evt, _session);
            }

            var plugins = new PluginManager(new PluginInitializerInfo
            {
                Logger = _logger,
                Session = _session,
                Settings = _settings,
                Statistics = stats
            });
            plugins.InitPlugins();
            _machine.SetFailureState(new LoginState());
            Logger.SetLoggerContext(_session);

            _session.Navigation.UpdatePositionEvent +=
                (lat, lng) => _session.EventDispatcher.Send(new UpdatePositionEvent { Latitude = lat, Longitude = lng });
            //  _session.Navigation.UpdatePositionEvent += Navigation_UpdatePositionEvent;

            RouteOptimizeUtil.RouteOptimizeEvent +=
                optimizedroute =>
                    _session.EventDispatcher.Send(new OptimizeRouteEvent { OptimizedRoute = optimizedroute });
            // RouteOptimizeUtil.RouteOptimizeEvent += InitializePokestopsAndRoute;

            Navigation.GetHumanizeRouteEvent +=
                (route, destination) =>
                    _session.EventDispatcher.Send(new GetHumanizeRouteEvent { Route = route, Destination = destination });
            // Navigation.GetHumanizeRouteEvent += UpdateMap;

            FarmPokestopsTask.LootPokestopEvent +=
                pokestop => _session.EventDispatcher.Send(new LootPokestopEvent { Pokestop = pokestop });
            //FarmPokestopsTask.LootPokestopEvent += UpdateMap;

            CatchNearbyPokemonsTask.PokemonEncounterEvent +=
                mappokemons =>
                    _session.EventDispatcher.Send(new PokemonsEncounterEvent { EncounterPokemons = mappokemons });
            //CatchNearbyPokemonsTask.PokemonEncounterEvent += UpdateMap;

            CatchIncensePokemonsTask.PokemonEncounterEvent +=
                mappokemons =>
                    _session.EventDispatcher.Send(new PokemonsEncounterEvent { EncounterPokemons = mappokemons });
            //CatchIncensePokemonsTask.PokemonEncounterEvent += UpdateMap;
        }

        private void StartB(object sender, RoutedEventArgs e)
        {
            Logger.Write("Started");
            Task.Run(StartBot);
        }

        private async Task StartBot()
        {
            Logger.Write("Task1");
            await _machine.AsyncStart(new VersionCheckState(), _session);
            Logger.Write("Task2");

            if (_settings.UseTelegramApi)
            {
                _session.Telegram = new TelegramService(_settings.TelegramApiKey, _session);
            }

            _settings.CheckProxy();

            QuitEvent.WaitOne();
        }

        public static void ColoredConsoleWrite(Color color, string text)
        {
            if (text.Length <= 0)
                return;

            BrushConverter bc = new BrushConverter();
            TextRange tr = new TextRange(Instance.LogTextBox.Document.ContentEnd,
                Instance.LogTextBox.Document.ContentEnd)
            { Text = text };
            try
            {
                tr.ApplyPropertyValue(TextElement.ForegroundProperty,
                    bc.ConvertFromString(color.ToString()));
            }
            catch (FormatException) { }
        }
        private static void UnhandledExceptionEventHandler(object obj, UnhandledExceptionEventArgs args)
        {
            Logger.Write("Exception caught, writing LogBuffer.", force: true);
            throw new Exception();
        }
    }
}
