#region using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using System.Device.Location;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using POGOProtos.Data;
using POGOProtos.Inventory.Item;
using POGOProtos.Map.Fort;
using POGOProtos.Map.Pokemon;
using PokemonGo.RocketAPI.Helpers;
using PoGo.NecroBot.Logic.PoGoUtils;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.Logging;
using PoGo.NecroBot.Logic.Service;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Utils;
using PoGo.NecroBot.Logic.Model.Settings;
using PoGo.NecroBot.Logic.Service.Elevation;
using RocketBot2.Helpers;
using RocketBot2.Models;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic;
using System.Reflection;
using PoGo.NecroBot.Logic.Tasks;
using System.Net;
using RocketBot2.CommandLineUtility;
using System.Diagnostics;
using PokemonGo.RocketAPI;
using RocketBot2.Win32;
using System.Net.Http;
using PokemonGo.RocketAPI.Logging;
using POGOProtos.Enums;
using POGOProtos.Networking.Responses;

#endregion


namespace RocketBot2.Forms
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        public static MainForm Instance;
        public static SynchronizationContext SynchronizationContext;
        private static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);
        private static string _subPath = "";
        private static bool _enableJsonValidation = true;
        private static bool _excelConfigAllow = false;
        private static bool _ignoreKillSwitch;
        private bool _botStarted = false;

        private static readonly Uri StrKillSwitchUri =
            new Uri("https://raw.githubusercontent.com/TheUnnamedOrganisation/RocketBot/master/KillSwitch.txt");
        private static readonly Uri StrMasterKillSwitchUri =
            new Uri("https://raw.githubusercontent.com/TheUnnamedOrganisation/PoGo.NecroBot.Logic/master/MKS.txt");

        private GlobalSettings _settings;
        private StateMachine _machine;
        private PointLatLng _currentLatLng;
        private List<PointLatLng> _routePoints;
        private string[] args;

        private static GMapMarker _playerMarker;
        private readonly List<PointLatLng> _playerLocations = new List<PointLatLng>();
        private readonly GMapOverlay _playerOverlay = new GMapOverlay("players");
        private readonly GMapOverlay _playerRouteOverlay = new GMapOverlay("playerroutes");
        private readonly GMapOverlay _pokemonsOverlay = new GMapOverlay("pokemons");
        private readonly GMapOverlay _pokestopsOverlay = new GMapOverlay("pokestops");
        private readonly GMapOverlay _searchAreaOverlay = new GMapOverlay("areas");

        public static Session _session;

        public MainForm(string[] _args)
        {
            InitializeComponent();
            SynchronizationContext = SynchronizationContext.Current;
            Instance = this;
            args = _args;
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.ErrorHandler);
        }

        private void ErrorHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Debug.WriteLine(e.ExceptionObject.ToString());
            ConsoleHelper.ShowConsoleWindow();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SetStatusText(Application.ProductName + " " + Application.ProductVersion);
            speedLable.Parent = gMapControl1;
            showMoreCheckBox.Parent = gMapControl1;
            followTrainerCheckBox.Parent = gMapControl1;
            togglePrecalRoute.Parent = gMapControl1;
            InitializeBot(null);
            InitializePokemonForm();
            InitializeMap();
            VersionHelper.CheckVersion();
            btnRefresh.Enabled = false;
            ConsoleHelper.HideConsoleWindow();
        }

        private void InitializeMap()
        {
                var lat = _session.Client.Settings.DefaultLatitude;
                var lng = _session.Client.Settings.DefaultLongitude;
                gMapControl1.MapProvider = GoogleMapProvider.Instance;
                gMapControl1.Manager.Mode = AccessMode.ServerOnly;
                GMapProvider.WebProxy = null;
                gMapControl1.Position = new PointLatLng(lat, lng);
                gMapControl1.DragButton = MouseButtons.Left;

                gMapControl1.MinZoom = 1;
                gMapControl1.MaxZoom = 20;
                gMapControl1.Zoom = 17;

                gMapControl1.Overlays.Add(_searchAreaOverlay);
                gMapControl1.Overlays.Add(_pokestopsOverlay);
                gMapControl1.Overlays.Add(_pokemonsOverlay);
                gMapControl1.Overlays.Add(_playerOverlay);
                gMapControl1.Overlays.Add(_playerRouteOverlay);

                _playerMarker = new GMapMarkerTrainer(new PointLatLng(lat, lng), ResourceHelper.GetImage("PlayerLocation", 50, 50));
                _playerOverlay.Markers.Add(_playerMarker);
                _playerMarker.Position = new PointLatLng(lat, lng);
                _searchAreaOverlay.Polygons.Clear();
                S2GMapDrawer.DrawS2Cells(S2Helper.GetNearbyCellIds(lng, lat), _searchAreaOverlay);
        }

        private void InitializeBot(Action<ISession, StatisticsAggregator> onBotStarted)
        {
            var ioc = TinyIoC.TinyIoCContainer.Current;
            //Setup Logger for API
            APIConfiguration.Logger = new APILogListener();

            //Application.EnableVisualStyles();
            var strCulture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

            var culture = CultureInfo.CreateSpecificCulture("en");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;

            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionEventHandler;
            
            Console.Title = @"RocketBot2";
            Console.CancelKeyPress += (sender, eArgs) =>
            {
                QuitEvent.Set();
                eArgs.Cancel = true;
            };
             
            // Command line parsing
            var commandLine = new Arguments(args);
            // Look for specific arguments values
            if (commandLine["subpath"] != null && commandLine["subpath"].Length > 0)
            {
                _subPath = commandLine["subpath"];
            }
            if (commandLine["jsonvalid"] != null && commandLine["jsonvalid"].Length > 0)
            {
                switch (commandLine["jsonvalid"])
                {
                    case "true":
                        _enableJsonValidation = true;
                        break;

                    case "false":
                        _enableJsonValidation = false;
                        break;
                }
            }
            if (commandLine["killswitch"] != null && commandLine["killswitch"].Length > 0)
            {
                switch (commandLine["killswitch"])
                {
                    case "true":
                        _ignoreKillSwitch = false;
                        break;

                    case "false":
                        _ignoreKillSwitch = true;
                        break;
                }
            }

            bool excelConfigAllow = false;
            if (commandLine["provider"] != null && commandLine["provider"] == "excel")
            {
                excelConfigAllow = true;
            }

            var _fileName = $"RocketBot2-{DateTime.Today.ToString("dd-MM-yyyy")}-{DateTime.Now.ToString("HH-mm-ss")}.txt";

            Logger.AddLogger(new ConsoleLogger(LogLevel.Service), _subPath);
            Logger.AddLogger(new FileLogger(LogLevel.Service, _fileName), _subPath);
            Logger.AddLogger(new WebSocketLogger(LogLevel.Service), _subPath);

            var profilePath = Path.Combine(Directory.GetCurrentDirectory(), _subPath);
            var profileConfigPath = Path.Combine(profilePath, "config");
            var configFile = Path.Combine(profileConfigPath, "config.json");
            var excelConfigFile = Path.Combine(profileConfigPath, "config.xlsm");

            GlobalSettings settings;
            var boolNeedsSetup = false;

            if (File.Exists(configFile))
            {
                // Load the settings from the config file
                settings = GlobalSettings.Load(_subPath, _enableJsonValidation);
                if (excelConfigAllow)
                {
                    if (!File.Exists(excelConfigFile))
                    {
                        Logger.Write(
                            "Migrating existing json confix to excel config, please check the config.xlsm in your config folder"
                        );

                        ExcelConfigHelper.MigrateFromObject(settings, excelConfigFile);
                    }
                    else
                        settings = ExcelConfigHelper.ReadExcel(settings, excelConfigFile);

                    Logger.Write("Bot will run with your excel config, loading excel config");
                }
            }
            else
            {
                settings = new GlobalSettings
                {
                    ProfilePath = profilePath,
                    ProfileConfigPath = profileConfigPath,
                    GeneralConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "config"),
                    ConsoleConfig = { TranslationLanguageCode = strCulture }
                };

                boolNeedsSetup = true;
            }
            if (commandLine["latlng"] != null && commandLine["latlng"].Length > 0)
            {
                var crds = commandLine["latlng"].Split(',');
                try
                {
                    var lat = double.Parse(crds[0]);
                    var lng = double.Parse(crds[1]);
                    settings.LocationConfig.DefaultLatitude = lat;
                    settings.LocationConfig.DefaultLongitude = lng;
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                // Values are available here
                if (options.Init)
                {
                    settings.GenerateAccount(options.IsGoogle, options.Template, options.Start, options.End, options.Password);
                }
            }
            
            var lastPosFile = Path.Combine(profileConfigPath, "LastPos.ini");
            if (File.Exists(lastPosFile) && settings.LocationConfig.StartFromLastPosition)
            {
                var text = File.ReadAllText(lastPosFile);
                var crds = text.Split(':');
                try
                {
                    var lat = double.Parse(crds[0]);
                    var lng = double.Parse(crds[1]);
                    //If lastcoord is snipe coord, bot start from default location

                    if (LocationUtils.CalculateDistanceInMeters(lat, lng, settings.LocationConfig.DefaultLatitude, settings.LocationConfig.DefaultLongitude) < 2000)
                    {
                        settings.LocationConfig.DefaultLatitude = lat;
                        settings.LocationConfig.DefaultLongitude = lng;
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }

             if (!_ignoreKillSwitch)
             {
                 if (CheckMKillSwitch())
                 {
                     return;
                 }
                 _botStarted = CheckKillSwitch();
             }

            var logicSettings = new LogicSettings(settings);
            var translation = Translation.Load(logicSettings);
            TinyIoC.TinyIoCContainer.Current.Register<ITranslation>(translation);

            if (settings.GPXConfig.UseGpxPathing)
            {
                var xmlString = File.ReadAllText(settings.GPXConfig.GpxFile);
                var readgpx = new GpxReader(xmlString, translation);
                var nearestPt = readgpx.Tracks.SelectMany(
                        (trk, trkindex) =>
                            trk.Segments.SelectMany(
                                (seg, segindex) =>
                                    seg.TrackPoints.Select(
                                        (pt, ptindex) =>
                                            new
                                            {
                                                TrackPoint = pt,
                                                TrackIndex = trkindex,
                                                SegIndex = segindex,
                                                PtIndex = ptindex,
                                                Latitude = Convert.ToDouble(pt.Lat, CultureInfo.InvariantCulture),
                                                Longitude = Convert.ToDouble(pt.Lon, CultureInfo.InvariantCulture),
                                                Distance = LocationUtils.CalculateDistanceInMeters(
                                                    settings.LocationConfig.DefaultLatitude,
                                                    settings.LocationConfig.DefaultLongitude,
                                                    Convert.ToDouble(pt.Lat, CultureInfo.InvariantCulture),
                                                    Convert.ToDouble(pt.Lon, CultureInfo.InvariantCulture)
                                                )
                                            }
                                    )
                            )
                    )
                    .OrderBy(pt => pt.Distance)
                    .FirstOrDefault(pt => pt.Distance <= 5000);

                if (nearestPt != null)
                {
                    settings.LocationConfig.DefaultLatitude = nearestPt.Latitude;
                    settings.LocationConfig.DefaultLongitude = nearestPt.Longitude;
                    settings.LocationConfig.ResumeTrack = nearestPt.TrackIndex;
                    settings.LocationConfig.ResumeTrackSeg = nearestPt.SegIndex;
                    settings.LocationConfig.ResumeTrackPt = nearestPt.PtIndex;
                }
            }
            IElevationService elevationService = new ElevationService(settings);

            //validation auth.config
            if (boolNeedsSetup)
            {
                AuthAPIForm form = new AuthAPIForm(true);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    settings.Auth.APIConfig = form.Config;
                }
            }
            else
            {
                var apiCfg = settings.Auth.APIConfig;

                if (apiCfg.UsePogoDevAPI)
                {
                    if (string.IsNullOrEmpty(apiCfg.AuthAPIKey))
                    {
                        Logger.Write(
                            "You select pogodev API but not provide API Key, please press any key to exit and correct you auth.json, \r\n The Pogodev API key call be purchased at - https://talk.pogodev.org/d/51-api-hashing-service-by-pokefarmer",
                            LogLevel.Error
                        );

                        Console.ReadKey();
                        Environment.Exit(0);
                    }
                    //TODO - test api call to valida auth key
                }
                else if (apiCfg.UseLegacyAPI)
                {
                    Logger.Write(
                        "You bot will start after 15 second, You are running bot with  Legacy API (0.45) it will increase your risk to be banned and trigger captcha. Config captcha in config.json to auto resolve them",
                        LogLevel.Warning
                    );

#if RELEASE
                    Thread.Sleep(15000);
#endif
                }
                else
                {
                    Logger.Write(
                        "At least 1 authentication method is selected, please correct your auth.json, ",
                        LogLevel.Error
                    );
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }

            _session = new Session(settings,
                new ClientSettings(settings, elevationService), logicSettings, elevationService, translation);

            ioc.Register<ISession>(_session);

            Logger.SetLoggerContext(_session);

            MultiAccountManager accountManager = new MultiAccountManager(logicSettings.Bots);
            ioc.Register<MultiAccountManager>(accountManager);

            if (boolNeedsSetup)
            {
                StarterConfigForm configForm = new StarterConfigForm(_session, settings, elevationService, configFile);
                if (configForm.ShowDialog() == DialogResult.OK)
                {
                    var fileName = Assembly.GetEntryAssembly().Location;
                    Process.Start(fileName);
                    Environment.Exit(0);
                }

                //if (GlobalSettings.PromptForSetup(_session.Translation))
                //{
                //    _session = GlobalSettings.SetupSettings(_session, settings, elevationService, configFile);

                //    var fileName = Assembly.GetExecutingAssembly().Location;
                //    Process.Start(fileName);
                //    Environment.Exit(0);
                //}
                else
                {
                    GlobalSettings.Load(_subPath, _enableJsonValidation);
                    
                    Logger.Write("Press a Key to continue...",
                        LogLevel.Warning);
                    Console.ReadKey();
                    return;
                }

                if (excelConfigAllow)
                {
                    ExcelConfigHelper.MigrateFromObject(settings, excelConfigFile);
                }
            }

            Resources.ProgressBar.Start("RocketBot2 is starting up", 10);

            Resources.ProgressBar.Fill(20);

            var machine = new StateMachine();
            var stats = _session.RuntimeStatistics;

            Resources.ProgressBar.Fill(30);
            var strVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(4);
            stats.DirtyEvent +=
                () =>
                {
                    SetStatusText($"[RocketBot2 v{strVersion}] " +
                                    stats.GetTemplatedStats(
                                        _session.Translation.GetTranslation(TranslationString.StatsTemplateString),
                                        _session.Translation.GetTranslation(TranslationString.StatsXpTemplateString)));
                };

            Resources.ProgressBar.Fill(40);

            var aggregator = new StatisticsAggregator(stats);
            if (onBotStarted != null) onBotStarted(_session, aggregator);

            Resources.ProgressBar.Fill(50);
            var listener = new ConsoleEventListener();
            Resources.ProgressBar.Fill(60);
            var snipeEventListener = new SniperEventListener();

            _session.EventDispatcher.EventReceived += evt => listener.Listen(evt, _session);
            _session.EventDispatcher.EventReceived += evt => aggregator.Listen(evt, _session);
            _session.EventDispatcher.EventReceived += evt => snipeEventListener.Listen(evt, _session);

            Resources.ProgressBar.Fill(70);

            machine.SetFailureState(new LoginState());
            Resources.ProgressBar.Fill(80);

            Resources.ProgressBar.Fill(90);

            _session.Navigation.WalkStrategy.UpdatePositionEvent += 
                (session, lat, lng, speed) => _session.EventDispatcher.Send(new UpdatePositionEvent { Latitude = lat, Longitude = lng, Speed = speed });
            _session.Navigation.WalkStrategy.UpdatePositionEvent += LoadSaveState.SaveLocationToDisk;
            
            Navigation.GetHumanizeRouteEvent +=
                (points)  => _session.EventDispatcher.Send(new Logic.Event.GetHumanizeRouteEvent { Points = points} );
            Navigation.GetHumanizeRouteEvent += UpdateMap;

            UseNearbyPokestopsTask.LootPokestopEvent +=
                pokestop => _session.EventDispatcher.Send(new Logic.Event.LootPokestopEvent { Pokestop = pokestop });
            UseNearbyPokestopsTask.LootPokestopEvent += UpdateMap;

            CatchNearbyPokemonsTask.PokemonEncounterEvent +=
                mappokemons => _session.EventDispatcher.Send(new Logic.Event.PokemonsEncounterEvent { EncounterPokemons = mappokemons });
            CatchNearbyPokemonsTask.PokemonEncounterEvent += UpdateMap;

            CatchIncensePokemonsTask.PokemonEncounterEvent +=
                mappokemons => _session.EventDispatcher.Send(new Logic.Event.PokemonsEncounterEvent { EncounterPokemons = mappokemons });
            CatchIncensePokemonsTask.PokemonEncounterEvent += UpdateMap;

            CatchLurePokemonsTask.PokemonEncounterEvent +=
                         mappokemons => _session.EventDispatcher.Send(new Logic.Event.PokemonsEncounterEvent { EncounterPokemons = mappokemons });
            CatchLurePokemonsTask.PokemonEncounterEvent += UpdateMap;

            Resources.ProgressBar.Fill(100);

            //TODO: temporary
            if (settings.Auth.APIConfig.UseLegacyAPI)
            {
                Logger.Write("The PoGoDev Community Has Updated The Hashing Service To Be Compatible With 0.57.4 So We Have Updated Our Code To Be Compliant. Unfortunately During This Update Niantic Has Also Attempted To Block The Legacy .45 Service Again So At The Moment Only Hashing Service Users Are Able To Login Successfully. Please Be Patient As Always We Will Attempt To Keep The Bot 100% Free But Please Realize We Have Already Done Quite A Few Workarounds To Keep .45 Alive For You Guys.  Even If We Are Able To Get Access Again To The .45 API Again It Is Over 3 Months Old So Is Going To Be More Detectable And Cause Captchas. Please Consider Upgrading To A Paid API Key To Avoid Captchas And You Will  Be Connecting Using Latest Version So Less Detectable So More Safe For You In The End.", LogLevel.Warning);
                Logger.Write("The bot will now close", LogLevel.Error);
                Console.ReadKey();
                Environment.Exit(0);
                return;
            }
            //

            if (settings.WebsocketsConfig.UseWebsocket)
            {
                var websocket = new WebSocketInterface(settings.WebsocketsConfig.WebSocketPort, _session);
                _session.EventDispatcher.EventReceived += evt => websocket.Listen(evt, _session);
            }

            ioc.Register<MultiAccountManager>(accountManager);

            var bot = accountManager.GetStartUpAccount();

            if (accountManager.Accounts.Count > 1)
            {
                foreach (var _bot in accountManager.Accounts)
                {
                    var _item = new ToolStripMenuItem();
                    _item.Text = _bot.Username;
                    _item.Click += delegate
                    {                       
                        if (!_botStarted) _session.ReInitSessionWithNextBot(_bot);
                        accountManager.SwitchAccountTo(_bot);
                    };

                    if (_item.Text == bot.Username)
                    {
                        _session.ReInitSessionWithNextBot(_bot);
                        _item.Enabled = false;
                    }
                    accountsToolStripMenuItem.DropDownItems.Add(_item);
                }
            }
            else
            {
                _session.ReInitSessionWithNextBot(bot);
                menuStrip1.Items.Remove(accountsToolStripMenuItem);
            }

            _machine = machine;
            _settings = settings;
            _excelConfigAllow = excelConfigAllow;
        }

#pragma warning disable 1998
        private async Task StartBot()
        {
#pragma warning disable 4014
            _machine.AsyncStart(new Logic.State.VersionCheckState(), _session, _subPath, _excelConfigAllow);
         
            try
            {
                Console.Clear();
            }
            catch (IOException)
            {
            }

            if (_settings.TelegramConfig.UseTelegramAPI)
                _session.Telegram = new TelegramService(_settings.TelegramConfig.TelegramAPIKey, _session);
            if (_session.LogicSettings.EnableHumanWalkingSnipe &&
                            _session.LogicSettings.HumanWalkingSnipeUseFastPokemap)
            {
                // jjskuld - Ignore CS4014 warning for now.
                //#pragma warning disable 4014
                HumanWalkSnipeTask.StartFastPokemapAsync(_session,
                    _session.CancellationTokenSource.Token).ConfigureAwait(false); // that need to keep data live
                //#pragma warning restore 4014
            }

            if (_session.LogicSettings.UseSnipeLocationServer ||
              _session.LogicSettings.HumanWalkingSnipeUsePogoLocationFeeder)
                SnipePokemonTask.AsyncStart(_session);


            if (_session.LogicSettings.DataSharingConfig.EnableSyncData)
            {
                BotDataSocketClient.StartAsync(_session);
                _session.EventDispatcher.EventReceived += evt => BotDataSocketClient.Listen(evt, _session);
            }
            _settings.CheckProxy(_session.Translation);

            if (_session.LogicSettings.ActivateMSniper)
            {
                ServicePointManager.ServerCertificateValidationCallback +=
                    (sender, certificate, chain, sslPolicyErrors) => true;
                //temporary disable MSniper connection because site under attacking.
                //MSniperServiceTask.ConnectToService();
                //_session.EventDispatcher.EventReceived += evt => MSniperServiceTask.AddToList(evt);
            }
            var trackFile = Path.GetTempPath() + "\\rocketbot2.io";

            if (!File.Exists(trackFile) || File.GetLastWriteTime(trackFile) < DateTime.Now.AddDays(-1))
            {
                Thread.Sleep(10000);
                Thread mThread = new Thread(delegate ()
                {
                    var infoForm = new InfoForm();
                    infoForm.ShowDialog();
                });
                File.WriteAllText(trackFile, DateTime.Now.Ticks.ToString());
                mThread.SetApartmentState(ApartmentState.STA);

                mThread.Start();
            }

            QuitEvent.WaitOne();
#pragma warning restore 4014
        }
#pragma warning restore 1998

        private void InitializePokestopsAndRoute(List<FortData> pokeStops)
        {
            SynchronizationContext.Post(o =>
            {
                _pokestopsOverlay.Routes.Clear();
                _pokestopsOverlay.Markers.Clear();
                _routePoints =
                    (from pokeStop in pokeStops
                     where pokeStop != null
                     select new PointLatLng(pokeStop.Latitude, pokeStop.Longitude)).ToList();

                togglePrecalRoute.Enabled = true;
                if (togglePrecalRoute.Checked)
                {
                    var route = new GMapRoute(_routePoints, "Walking Path")
                    {
                        Stroke = new Pen(Color.FromArgb(128, 0, 179, 253), 4)
                    };
                     _pokestopsOverlay.Routes.Add(route);
                }

                foreach (var pokeStop in pokeStops)
                {
                    var pokeStopLoc = new PointLatLng(pokeStop.Latitude, pokeStop.Longitude);
                    Image fort = null;
                    switch (pokeStop.Type)
                    {
                        case FortType.Checkpoint:
                            fort = ResourceHelper.GetImage("Pokestop");
                            break;
                        case FortType .Gym:
                            switch (pokeStop.OwnedByTeam)
                            {
                                case POGOProtos.Enums.TeamColor.Neutral:
                                    fort = ResourceHelper.GetImage("GymVide");
                                    break;
                                case POGOProtos.Enums.TeamColor.Blue:
                                    fort = ResourceHelper.GetImage("GymBlue");
                                    break;
                                case POGOProtos.Enums.TeamColor.Red:
                                    fort = ResourceHelper.GetImage("GymRed");
                                    break;
                                case POGOProtos.Enums.TeamColor.Yellow:
                                    fort = ResourceHelper.GetImage("GymYellow");
                                    break;
                            }
                            break;
                        default:
                            fort = ResourceHelper.GetImage("Pokestop");
                            break;
                    }
                    var pokestopMarker = new GMapMarkerPokestops(pokeStopLoc, fort);
                    _pokestopsOverlay.Markers.Add(pokestopMarker);
                }
            }, null);
        }
		
        private void Navigation_UpdatePositionEvent(double lat, double lng)
        {
            var latlng = new PointLatLng(lat, lng);
            _playerLocations.Add(latlng);

            SynchronizationContext.Post(o =>
            {
                _playerOverlay.Markers.Remove(_playerMarker);
                if (!_currentLatLng.IsEmpty)
                    _playerMarker = _currentLatLng.Lng < latlng.Lng
                        ? new GMapMarkerTrainer(latlng, ResourceHelper.GetImage("PlayerLocation2", 50, 50))
                        : new GMapMarkerTrainer(latlng, ResourceHelper.GetImage("PlayerLocation", 50, 50));
                _playerOverlay.Markers.Add(_playerMarker);
                if (followTrainerCheckBox.Checked)
                    gMapControl1.Position = latlng;
            }, null);
            _currentLatLng = latlng;
            UpdateMap();
        }

        private void showMoreCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (showMoreCheckBox.Checked)
            {
                followTrainerCheckBox.Visible = true;
                togglePrecalRoute.Visible = true;
            }
            else
            {
                followTrainerCheckBox.Visible = false;
                togglePrecalRoute.Visible = false;
            }
        }

        private void followTrainerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (followTrainerCheckBox.Checked)
            {
                gMapControl1.CanDragMap = false;
                gMapControl1.Position = _currentLatLng;
            }
            else
            {
                gMapControl1.CanDragMap = true;
            }
        }

        private void togglePrecalRoute_CheckedChanged(object sender, EventArgs e)
        {
            SynchronizationContext.Post(o =>
            {
                if (togglePrecalRoute.Checked)
                {
                    _pokestopsOverlay.Routes.Clear();
                    var route = new GMapRoute(_routePoints, "Walking Path")
                    {
                        Stroke = new Pen(Color.FromArgb(128, 0, 179, 253), 4)
                    };
                    _pokestopsOverlay.Routes.Add(route);
                    return;
                }
                _pokestopsOverlay.Routes.Clear();
            }, null);
        }

        #region UPDATEMAP

        private void UpdateMap()
        {
            SynchronizationContext.Post(o =>
            {
                var route = new GMapRoute(_playerLocations, "step")
                {
                    Stroke = new Pen(Color.FromArgb(175, 175, 175), 2) { DashStyle = DashStyle.Dot }
                };
                _playerOverlay.Routes.Clear();
                _playerOverlay.Routes.Add(route);
            }, null);
        }

        private int encounterPokemonsCount;
        private void UpdateMap(List<GeoCoordinate> points)
        {
            SynchronizationContext.Post(o =>
            {
                var routePointLatLngs = new List<PointLatLng>();
                foreach (var item in points)
                {
                    routePointLatLngs.Add(new PointLatLng(item.Latitude, item.Longitude));
                }
                var routes = new GMapRoute(routePointLatLngs, routePointLatLngs.ToString())
                {
                    Stroke = new Pen(Color.FromArgb(128, 0, 179, 253), 4) { DashStyle = DashStyle.Dash }
                };

                if (encounterPokemonsCount > 5 || encounterPokemonsCount == 0)
                {
                    _playerOverlay.Markers.Clear();
                    _pokemonsOverlay.Markers.Clear();
                    _playerLocations.Clear();
                    Navigation_UpdatePositionEvent(_session.Client.CurrentLatitude,
                        _session.Client.CurrentLongitude);
                    //get optimized route
                    var _pokeStops = Logic.Utils.RouteOptimizeUtil.Optimize(_session.Forts.ToArray(), _session.Client.CurrentLatitude,
                        _session.Client.CurrentLongitude);
                    InitializePokestopsAndRoute(_pokeStops);
                    encounterPokemonsCount = 0;
                }

                encounterPokemonsCount++;
                _playerRouteOverlay.Routes.Clear();
                _playerRouteOverlay.Routes.Add(routes);
            }, null);
        }

        private void UpdateMap(FortData pokestop)
        {
            SynchronizationContext.Post(o =>
            {
                var pokeStopLoc = new PointLatLng(pokestop.Latitude, pokestop.Longitude);
                lock (_pokestopsOverlay.Markers)
                {
                    for (var i = 0; i < _pokestopsOverlay.Markers.Count; i++)
                    {
                        var marker = _pokestopsOverlay.Markers[i];
                        if (marker.Position == pokeStopLoc)
                        {
                            _pokestopsOverlay.Markers.Remove(marker);
                             var pokestopMarker = new GMapMarkerPokestops(pokeStopLoc,
                                ResourceHelper.GetImage("Pokestop_looted"));
                            //pokestopMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                            //pokestopMarker.ToolTip = new GMapBaloonToolTip(pokestopMarker);
                            _pokestopsOverlay.Markers.Add(pokestopMarker);
                        }
                    }
                }
            }, null);
        }

        private void UpdateMap(List<MapPokemon> encounterPokemons)
        {
            SynchronizationContext.Post(o =>
            {
                foreach (var pokemon in encounterPokemons)
                {
                    var pkmImage = ResourceHelper.GetImage("Pokemon_" + pokemon.PokemonId.GetHashCode(), 36, 36);
                    var pointLatLng = new PointLatLng(pokemon.Latitude, pokemon.Longitude);
                    GMapMarker pkmMarker = new GMapMarkerTrainer(pointLatLng, pkmImage);
                    _pokemonsOverlay.Markers.Add(pkmMarker);
                }
            }, null);
        }

        #endregion

        #region INTERFACE

        public static void ColoredConsoleWrite(Color color, string text)
        {
            if (text.Length <= 0)
                return;

            if (Instance.InvokeRequired)
            {
                Instance.Invoke(new Action<Color, string>(ColoredConsoleWrite), color, text);
                return;
            }

            if (text.Contains("Error with API request type: DownloadRemoteConfigVersion"))
            {
                Instance.logTextBox.SelectionColor = Color.Yellow;
                Instance.logTextBox.AppendText($"Warning: with API request type: DownloadRemoteConfigVersion. Please wait...\r\n");
                Instance.logTextBox.ScrollToCaret();
                return;
            }

            if (text.Contains("PokemonGo.RocketAPI.Exceptions.CaptchaException:"))
            {
                Instance.logTextBox.SelectionColor = Color.Yellow;
                Instance.logTextBox.AppendText($"Warning: with CaptchaException not login conected\r\nPlease refresh Inventory list.\r\n");
                Instance.logTextBox.ScrollToCaret();
                return;
            }

            if (text.Contains("PoGo.NecroBot.Logic.Strategies.Walk.BaseWalkStrategy.<DoWalk>"))
            {
                Instance.logTextBox.SelectionColor = Color.Red;
                Instance.logTextBox.AppendText($"Error: with BaseWalkStrategy quota depassed\r\nPlease close RocketBot and wait.\r\n");
                Instance.logTextBox.ScrollToCaret();
                return;
            }
 
            Instance.logTextBox.SelectionColor = color;
            Instance.logTextBox.AppendText(text + $"\r\n");
            Instance.logTextBox.ScrollToCaret();
        }

        public static void SetSpeedLable(string text)
        {
            if (Instance.InvokeRequired)
            {
                Instance.Invoke(new Action<string>(SetSpeedLable), text);
                return;
            }
            Instance.speedLable.Text = text;
            Instance.Navigation_UpdatePositionEvent(_session.Client.CurrentLatitude, _session.Client.CurrentLongitude);
            Instance.showMoreCheckBox.Enabled = Instance._botStarted;
        }

        public async void SetStatusText(string text)
        {
            if (Instance.InvokeRequired)
            {
                Instance.Invoke(new Action<string>(SetStatusText), text);
                return;
            }
            Instance.Text = text;
            Instance.statusLabel.Text = text;
            Console.Title = text;

            if (checkBoxAutoRefresh.Checked)
                await ReloadPokemonList().ConfigureAwait(false);
        }

        #endregion INTERFACE

        #region EVENTS

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await ReloadPokemonList().ConfigureAwait(false);
        }

        private void startStopBotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_botStarted)
            {
                Environment.Exit(0);
                return;
            }
            startStopBotToolStripMenuItem.Text = @"■ Exit RocketBot2";
            _botStarted = true;
            btnRefresh.Enabled = true;
            pokeEaseToolStripMenuItem.Enabled = true;
            Task.Run(StartBot);
        }

        private void todoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Form settingsForm = new SettingsForm(ref _settings);
            settingsForm.ShowDialog();
            var newLocation = new PointLatLng(_settings.LocationConfig.DefaultLatitude, _settings.LocationConfig.DefaultLongitude);
            gMapControl1.Position = newLocation;
            _playerMarker.Position = newLocation;
            _playerLocations.Clear();
            _playerLocations.Add(newLocation);
            UpdateMap();
        }

        private void pokeEaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pokeEaseToolStripMenuItem.Text.Equals(@"Show PokeEase"))
            {
                if (ConsoleHelper.ShowConsoleWindowPokeEase())
                {
                    pokeEaseToolStripMenuItem.Text = @"Hide PokeEase";
                    return;
                }

                var profilePath = Path.Combine(Directory.GetCurrentDirectory(), _subPath);
                var profileConfigPath = Path.Combine(profilePath, "PokeEase");
                var exeFile = Path.Combine(profileConfigPath, "RocketBot2.exe");
                Process.Start(exeFile);
                pokeEaseToolStripMenuItem.Text = @"Hide PokeEase";
                return;
            }
                pokeEaseToolStripMenuItem.Text = @"Show PokeEase";
                ConsoleHelper.HideConsoleWindowPokeEase();
        }

        private void showConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showConsoleToolStripMenuItem.Text.Equals(@"Show Console"))
            {
                showConsoleToolStripMenuItem.Text = @"Hide Console";
                ConsoleHelper.ShowConsoleWindow();
                return;
            }
            showConsoleToolStripMenuItem.Text = @"Show Console";
            ConsoleHelper.HideConsoleWindow();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread mThread = new Thread(delegate ()
            {
                var infoForm = new InfoForm();
                infoForm.ShowDialog();
            });
            mThread.SetApartmentState(ApartmentState.STA);
            mThread.Start();
        }

        private void accountsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem en in accountsToolStripMenuItem.DropDownItems)
            {
                if (en.Text == _settings.Auth.CurrentAuthConfig.Username)
                    en.Enabled = false;
                else
                    en.Enabled = true;
            }
        }

        #endregion EVENTS

        #region POKEMON LIST

        private void InitializePokemonForm()
        {
            //olvPokemonList.ButtonClick += PokemonListButton_Click;

            pkmnName.ImageGetter = delegate (object rowObject)
            {
                var pokemon = rowObject as PokemonObject;
                // ReSharper disable once PossibleNullReferenceException
                var key = pokemon.PokemonId.ToString();
                if (!olvPokemonList.SmallImageList.Images.ContainsKey(key))
                {
                    var img = ResourceHelper.GetPokemonImage((int)pokemon.PokemonId);
                    olvPokemonList.SmallImageList.Images.Add(key, img);
                }
                return key;
            };

            olvPokemonList.FormatRow += delegate (object sender, FormatRowEventArgs e)
            {
                var pok = e.Model as PokemonObject;
                if (olvPokemonList.Objects
                    .Cast<PokemonObject>()
                    .Select(i => i.PokemonId)
                    // ReSharper disable once PossibleNullReferenceException
                    .Count(p => p == pok.PokemonId) > 1)
                    e.Item.BackColor = Color.LightGreen;

                var text = string.IsNullOrEmpty(pok.Nickname) ? _session.Translation.GetPokemonTranslation(pok.PokemonId) : pok.Nickname;
                e.Item.Text = pok.Favorited ? $"★ {text}" : text;

                foreach (OLVListSubItem sub in e.Item.SubItems)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    if (sub.Text.Equals("Evolve") && !pok.AllowEvolve)
                    {
                        sub.CellPadding = new Rectangle(100, 100, 0, 0);
                    }
                    if (sub.Text.Equals("Transfer") && !pok.AllowTransfer)
                    {
                        sub.CellPadding = new Rectangle(100, 100, 0, 0);
                    }
                    if (sub.Text.Equals("Power Up") && !pok.AllowPowerup)
                    {
                        sub.CellPadding = new Rectangle(100, 100, 0, 0);
                    }
                }
            };

            cmsPokemonList.Opening += delegate (object sender, CancelEventArgs e)
            {
                e.Cancel = false;
                cmsPokemonList.Items.Clear();

                var pokemons = olvPokemonList.SelectedObjects.Cast<PokemonObject>().Select(o => o.PokemonData).ToList();
                var AllowEvolve = olvPokemonList.SelectedObjects.Cast<PokemonObject>().Select(o => o).All(o => o.AllowEvolve);
                var AllowTransfer = olvPokemonList.SelectedObjects.Cast<PokemonObject>().Select(o => o).All(o => o.AllowTransfer);
                var AllowPowerup = olvPokemonList.SelectedObjects.Cast<PokemonObject>().Select(o => o).All(o => o.AllowPowerup);
                var Favorited = olvPokemonList.SelectedObjects.Cast<PokemonObject>().Select(o => o).All(o => o.Favorited);
                var count = pokemons.Count();

                if (count < 1)
                {
                    e.Cancel = true;
                }

                var item = new ToolStripMenuItem();
                var separator = new ToolStripSeparator();

                if (AllowTransfer)
                {
                    item.Text = $"Transfer {count} Pokémons";
                    item.Click += delegate { TransferPokemon(pokemons); };
                    cmsPokemonList.Items.Add(item);
                }

                if (count != 1) return;

                if (AllowEvolve)
                {
                    item = new ToolStripMenuItem { Text = $"Evolve" };
                    item.Click += delegate {
                        var pokemon = olvPokemonList.SelectedObjects.Cast<PokemonObject>().Select(o => o).First();
                        EvolvePokemon(pokemon.PokemonData);
                    };
                    cmsPokemonList.Items.Add(item);
                }

                if (AllowPowerup)
                {
                    item = new ToolStripMenuItem { Text = $"PowerUp" };
                    item.Click += delegate { PowerUpPokemon(pokemons); };
                    cmsPokemonList.Items.Add(item);
                }

                item = new ToolStripMenuItem { Text = Favorited ? "Un-Favorite" : "Favorite" };
                item.Click += delegate { FavoritedPokemon(pokemons, Favorited); };
                cmsPokemonList.Items.Add(item);

                item = new ToolStripMenuItem { Text = @"Rename" };
                item.Click += delegate
                {
                    var pokemonObject = olvPokemonList.SelectedObjects.Cast<PokemonObject>().Select(o => o).First();
                    using (var form = count == 1 ? new NicknamePokemonForm(pokemonObject) : new NicknamePokemonForm())
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            NicknamePokemon(pokemons, form.txtNickname.Text);
                        }
                    }
                };
                cmsPokemonList.Items.Add(item);

         };
        }

        private void olvPokemonList_ButtonClick(object sender, CellClickEventArgs e)
        {
            try
            {
                var pokemon = e.Model as PokemonObject;
                var cName = olvPokemonList.AllColumns[e.ColumnIndex].AspectToStringFormat;
                if (cName.Equals("Transfer"))
                {
                    // ReSharper disable once PossibleNullReferenceException
                    TransferPokemon(new List<PokemonData> { pokemon.PokemonData });
                }
                else if (cName.Equals("Power Up"))
                {
                    // ReSharper disable once PossibleNullReferenceException
                    PowerUpPokemon(new List<PokemonData> { pokemon.PokemonData });
                }
                else if (cName.Equals("Evolve"))
                {
                    // ReSharper disable once PossibleNullReferenceException
                    EvolvePokemon(pokemon.PokemonData);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex.ToString(), LogLevel.Error);
            }
        }

        private async void FavoritedPokemon(IEnumerable<PokemonData> pokemons, bool fav)
        {
            foreach (var pokemon in pokemons)
            {
                await Task.Run(async () => { await FavoritePokemonTask.Execute(_session, pokemon.Id, !fav); });
            }
            if (!checkBoxAutoRefresh.Checked)
                await ReloadPokemonList().ConfigureAwait(false);
        }

        private async void TransferPokemon(IEnumerable<PokemonData> pokemons)
        {
            var _pokemons = new List<ulong>();
            string poketotransfert = null;
            foreach (var pokemon in pokemons)
            {
                _pokemons.Add(pokemon.Id);
                poketotransfert = $"{poketotransfert} [{_session.Translation.GetPokemonTranslation(pokemon.PokemonId)}]";
            }
            DialogResult result = MessageBox.Show($"Do you want to tranfert {pokemons.Count()} Pokémons?\n\r {poketotransfert}", $"Tranfert {pokemons.Count()} Pokémons", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            switch (result)
            {
                case DialogResult.Yes:
                    {
                        await Task.Run(async () =>
                     {
                         await TransferPokemonTask.Execute(
                             _session, _session.CancellationTokenSource.Token, _pokemons
                         );
                     });
                        if (!checkBoxAutoRefresh.Checked)
                            await ReloadPokemonList().ConfigureAwait(false);
                    }
                    break;
            }
        }

        private async void PowerUpPokemon(IEnumerable<PokemonData> pokemons)
        {
            foreach (var pokemon in pokemons)
            {
                DialogResult result = MessageBox.Show($"Full Power Up {_session.Translation.GetPokemonTranslation(pokemon.PokemonId)}?", $"{Application.ProductName} - Max Power Up", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (result)
                {
                    case DialogResult.Yes:
                        await Task.Run(async () => { await UpgradeSinglePokemonTask.Execute(_session, pokemon.Id, true /* upgrade x times */); });
                        break;
                    case DialogResult.No:
                        await Task.Run(async () => { await UpgradeSinglePokemonTask.Execute(_session, pokemon.Id, false, 1 /* Only upgrade 1 time */); });
                        break;
                }
            }
            if (!checkBoxAutoRefresh.Checked)
                await ReloadPokemonList().ConfigureAwait(false);
        }

        private void EvolvePokemon(PokemonData pokemon)
        {
            using (var form = new EvoleToPokemon())
            {
                PokemonEvoleTo pok = new PokemonEvoleTo(_session, pokemon);
                foreach (var to in pok.EvolutionBranchs)
                {
                    var item = new PictureBox();
                    item.Image = ResourceHelper.ResizeImage(ResourceHelper.GetPokemonImage((int)to.Pokemon), item, true);
                    item.Click += async delegate
                    {
                        await Task.Run(async () => { await EvolveSpecificPokemonTask.Execute(_session, to.OriginPokemonId, to.Pokemon); });
                        if (!checkBoxAutoRefresh.Checked)
                            await ReloadPokemonList().ConfigureAwait(false);
                        form.Close();
                    };
                    form.flpPokemonToEvole.Controls.Add(item);
                }
                form.ShowDialog();
            }
        }

        public async void NicknamePokemon(IEnumerable<PokemonData> pokemons, string nickname)
        {
            var pokemonDatas = pokemons as IList<PokemonData> ?? pokemons.ToList();
            foreach (var pokemon in pokemonDatas)
            {
                var newName = new StringBuilder(nickname);
                newName.Replace("{Name}", Convert.ToString(pokemon.PokemonId));
                newName.Replace("{CP}", Convert.ToString(pokemon.Cp));
                newName.Replace("{IV}",
                    Convert.ToString(Math.Round(_session.Inventory.GetPerfect(pokemon)), CultureInfo.InvariantCulture));
                newName.Replace("{IA}", Convert.ToString(pokemon.IndividualAttack));
                newName.Replace("{ID}", Convert.ToString(pokemon.IndividualDefense));
                newName.Replace("{IS}", Convert.ToString(pokemon.IndividualStamina));
                if (nickname.Length > 12)
                {
                    Logger.Write($"\"{newName}\" is too long, please choose another name");
                    if (pokemonDatas.Count() == 1)
                    {
                        SetState(true);
                        return;
                    }
                    continue;
                }
                await Task.Run(async () => { await RenameSinglePokemonTask.Execute(_session, pokemon.Id, nickname, _session.CancellationTokenSource.Token); });
                if (!checkBoxAutoRefresh.Checked)
                    await ReloadPokemonList().ConfigureAwait(false);
            }
        }

        private async Task ReloadPokemonList()
        {
            if (!_botStarted) return;
            SetState(false);
            try
            {
                if (_session.Client.Download.ItemTemplates == null)
                      await _session.Client.Download.GetItemTemplates().ConfigureAwait(false);

                var templates =  _session.Client.Download.ItemTemplates.Where(x => x.PokemonSettings != null)
                        .Select(x => x.PokemonSettings)
                        .ToList();

                PokemonObject.Initilize(_session, templates);

                var pokemons = 
                   _session.Inventory.GetPokemons().Result
                   .Where(p => p != null && p.PokemonId > 0)
                   .OrderByDescending(PokemonInfo.CalculatePokemonPerfection)
                   .ThenByDescending(key => key.Cp)
                   .OrderBy(key => key.PokemonId);

                var pokemonObjects = new List<PokemonObject>();

                foreach (var pokemon in pokemons)
                {
                    var pokemonObject = new PokemonObject(pokemon);
                    //pokemonObject.Candy = PokemonInfo.GetCandy(_session, pokemon);
                    pokemonObjects.Add(pokemonObject);
                }

                var prevTopItem = olvPokemonList.TopItemIndex;
                olvPokemonList.SetObjects(pokemonObjects);
                olvPokemonList.TopItemIndex = prevTopItem;

                var PokeDex = _session.Inventory.GetPokeDexItems().Result;
                var _totalUniqueEncounters = PokeDex.Select(
                    i => new
                    {
                        Pokemon = i.InventoryItemData.PokedexEntry.PokemonId,
                        Captures = i.InventoryItemData.PokedexEntry.TimesCaptured
                    }
                );
                var _totalCaptures = _totalUniqueEncounters.Count(i => i.Captures > 0);
                var _totalData = PokeDex.Count();

                lblPokemonList.Text = _session.Translation.GetTranslation(TranslationString.AmountPkmSeenCaught, _totalData, _totalCaptures) +
                    $" / Storage: {_session.Client.Player.PlayerData.MaxPokemonStorage} ({pokemons.Count()} Pokémons, {_session.Inventory.GetEggs().Result.Count()} Eggs)";

                var items =
                    _session.Inventory.GetItems().Result
                    .Where(i => i != null)
                    .OrderBy(i => i.ItemId);

                var appliedItems =
                    _session.Inventory.GetAppliedItems().Result
                    .Where(aItems => aItems?.Item != null)
                    .SelectMany(aItems => aItems.Item)
                    .ToDictionary(item => item.ItemId, item => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(item.ExpireMs));

                flpItems.Controls.Clear();

                foreach (var item in items)
                {
                    var box = new ItemBox(item);
                    if (appliedItems.ContainsKey(item.ItemId))
                    {
                        box.expires = appliedItems[item.ItemId];
                        box.Enabled = false;
                    }
                         box.ItemClick += ItemBox_ItemClick;
                    flpItems.Controls.Add(box);
                }

                lblInventory.Text =
                        $"Types: {items.Count()} / Total: {_session.Inventory.GetTotalItemCount().Result} / Storage: {_session.Client.Player.PlayerData.MaxItemStorage}";
            }
            catch (ArgumentNullException)
            {
                Logger.Write("Please start the bot or wait until login is finished before loading Pokemon List",
                    LogLevel.Warning);
                SetState(true);
                return;
            }
            catch (Exception ex)
            {
                Logger.Write(ex.ToString(), LogLevel.Error);
            }
            SetState(true);
        }

        private async void ItemBox_ItemClick(object sender, EventArgs e)
        {
            var item = (ItemData)sender;

            using (var form = new ItemForm(item))
            {
                var result = form.ShowDialog();
                if (result != DialogResult.OK) return;
                switch (item.ItemId)
                {
                    case ItemId.ItemLuckyEgg:
                        {
                            await Task.Run(async () => { await UseLuckyEggTask.Execute(_session); });
                        }
                        break;
                    case ItemId.ItemIncenseOrdinary:
                        {
                            await Task.Run(async () => { await UseIncenseTask.Execute(_session); });
                        }
                        break;
                    default:
                        {
                            await Task.Run(async () => { await RecycleItemsTask.DropItem(_session, item.ItemId, decimal.ToInt32(form.numCount.Value)); });
                        }
                        break;
                }
                if (!checkBoxAutoRefresh.Checked)
                    await ReloadPokemonList().ConfigureAwait(false);
            }
        }

        private void SetState(bool state)
        {
            btnRefresh.Enabled = state;
        }

        #endregion POKEMON LIST

        //**** Program functions
        private static void EventDispatcher_EventReceived(IEvent evt)
        {
            throw new NotImplementedException();
        }

        private static bool CheckMKillSwitch()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var responseContent = client.GetAsync(StrMasterKillSwitchUri).Result;
                    if (responseContent.StatusCode != HttpStatusCode.OK)
                        return true;

                    var strResponse1 = responseContent.Content.ReadAsStringAsync().Result;

                    if (string.IsNullOrEmpty(strResponse1))
                        return true;

                    var strSplit1 = strResponse1.Split(';');

                    if (strSplit1.Length > 1)
                    {
                        var strStatus1 = strSplit1[0];
                        var strReason1 = strSplit1[1];
                        var strExitMsg = strSplit1[2];

                        if (strStatus1.ToLower().Contains("disable"))
                        {
                            Logger.Write(strReason1 + $"\n", LogLevel.Warning);

                            Logger.Write(strExitMsg + $"\n" + "Please press enter to continue", LogLevel.Error);
                            Console.ReadLine();
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                        return false;


                }
                catch (Exception ex)
                {
                    Logger.Write(ex.Message, LogLevel.Error);
                }
            }

            return false;
        }

        private static bool CheckKillSwitch()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var responseContent = client.GetAsync(StrKillSwitchUri).Result;
                    if (responseContent.StatusCode != HttpStatusCode.OK)
                        return true;

                    var strResponse = responseContent.Content.ReadAsStringAsync().Result;
                    if (string.IsNullOrEmpty(strResponse))
                        return true;

                    var strSplit = strResponse.Split(';');

                    if (strSplit.Length > 1)
                    {
                        var strStatus = strSplit[0];
                        var strReason = strSplit[1];

                        if (strStatus.ToLower().Contains("disable"))
                        {
                            Logger.Write(strReason + $"\n", LogLevel.Warning);

                            if (PromptForKillSwitchOverride(strReason))
                            {
                                // Override
                                Logger.Write("Overriding killswitch... you have been warned!", LogLevel.Warning);
                                return false;
                            }

                            Logger.Write("The bot will now close, please press enter to continue", LogLevel.Error);
                            Console.ReadLine();
                            Environment.Exit(0);
                            return true;
                        }
                    }
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    Logger.Write(ex.Message, LogLevel.Error);
                }
            }

            return false;
        }

        private static void UnhandledExceptionEventHandler(object obj, UnhandledExceptionEventArgs args)
        {
            Logger.Write("Exception caught, writing LogBuffer.", force: true);
            //throw new Exception();
        }

        public static bool PromptForKillSwitchOverride(string strReason)
        {
            Logger.Write("Do you want to override killswitch to bot at your own risk? Y/N", LogLevel.Warning);

            /*while (true)
              {
                  var strInput = Console.ReadLine().ToLower();

                  switch (strInput)
                  {
                      case "y":
                          // Override killswitch
                          return true;

                      case "n":
                          return false;

                      default:
                          Logger.Write("Enter y or n", LogLevel.Error);
                          continue;
                  }
              }*/
            DialogResult result = MessageBox.Show($"{strReason} \n\r Do you want to override killswitch to bot at your own risk? Y/N", $"{Application.ProductName} - Old API detected", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            switch (result)
            {
                case DialogResult.Yes: return true;
                case DialogResult.No: return false;
            }
            return false;
        }
    }
}
