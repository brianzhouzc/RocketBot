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
using GeoCoordinatePortable;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using POGOProtos.Data;
using POGOProtos.Inventory;
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
using RocketBot2.Logic.Tasks;
using RocketBot2.Logic.Event;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic;
using System.Reflection;
using PoGo.NecroBot.Logic.Tasks;
using System.Net;
using RocketBot2.CommandLineUtility;
using System.Diagnostics;
using RocketBot2.Logic.Utils;

#endregion


namespace RocketBot2.Forms
{
    public partial class MainForm : Form
    {
        public static MainForm Instance;
        public static SynchronizationContext SynchronizationContext;
        private static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);
        private static string _subPath = "";
        private static bool _enableJsonValidation = true;
        private static bool _excelConfigAllow = false;
        private static bool _ignoreKillSwitch;

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
            gMapControl1.Zoom = 16;

            gMapControl1.Overlays.Add(_searchAreaOverlay);
            gMapControl1.Overlays.Add(_pokestopsOverlay);
            gMapControl1.Overlays.Add(_pokemonsOverlay);
            gMapControl1.Overlays.Add(_playerOverlay);
            gMapControl1.Overlays.Add(_playerRouteOverlay);

            _playerMarker = new GMapMarkerTrainer(new PointLatLng(lat, lng), ResourceHelper.GetImage("Trainer_Front"));
            _playerOverlay.Markers.Add(_playerMarker);
            _playerMarker.Position = new PointLatLng(lat, lng);
            _searchAreaOverlay.Polygons.Clear();
            S2GMapDrawer.DrawS2Cells(S2Helper.GetNearbyCellIds(lng, lat), _searchAreaOverlay);
        }

        private void InitializeBot(Action<ISession, StatisticsAggregator> onBotStarted)
        {
            var ioc = TinyIoC.TinyIoCContainer.Current;

            //Application.EnableVisualStyles();
            var strCulture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

            var culture = CultureInfo.CreateSpecificCulture("en");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;

            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionEventHandler;
            /*
            Console.Title = @"NecroBot2";
            Console.CancelKeyPress += (sender, eArgs) =>
            {
                QuitEvent.Set();
                eArgs.Cancel = true;
            };
             */
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

            Logger.AddLogger(new ConsoleLogger(LogLevel.Service), _subPath);
            Logger.AddLogger(new FileLogger(LogLevel.Service), _subPath);
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

            var lastPosFile = Path.Combine(profileConfigPath, "LastPos.ini");
            if (File.Exists(lastPosFile) && settings.LocationConfig.StartFromLastPosition)
            {
                var text = File.ReadAllText(lastPosFile);
                var crds = text.Split(':');
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

            if (!_ignoreKillSwitch)
            {
                /*if (CheckKillSwitch() || CheckMKillSwitch())
                {
                    return;
                }*/
                CheckKillSwitch();
                CheckMKillSwitch();
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

                        //Console.ReadKey();
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
                    //Console.ReadKey();
                    Environment.Exit(0);
                }
            }

            _session = new Session(
                new ClientSettings(settings, elevationService), logicSettings, elevationService,
                translation
            );
            ioc.Register<ISession>(_session);

            Logger.SetLoggerContext(_session);

            if (boolNeedsSetup)
            {
                StarterConfigForm configForm = new StarterConfigForm(_session, settings, elevationService, configFile);
                if (configForm.ShowDialog() == DialogResult.OK)
                {
                    var fileName = Assembly.GetExecutingAssembly().Location;
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
                    /*
                    Logger.Write("Press a Key to continue...",
                        LogLevel.Warning);
                    Console.ReadKey();*/
                    return;
                }

                if (excelConfigAllow)
                {
                    ExcelConfigHelper.MigrateFromObject(settings, excelConfigFile);
                }
            }

            //ProgressBar.Start("NecroBot2 is starting up", 10);

            if (settings.WebsocketsConfig.UseWebsocket)
            {
                var websocket = new WebSocketInterface(settings.WebsocketsConfig.WebSocketPort, _session);
                _session.EventDispatcher.EventReceived += evt => websocket.Listen(evt, _session);
            }

            //ProgressBar.Fill(20);

            var machine = new StateMachine();
            var stats = _session.RuntimeStatistics;

            //ProgressBar.Fill(30);
            var strVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(4);
            stats.DirtyEvent +=
                () =>
                    SetStatusText($"[RocketBot2 v{strVersion}] " +
                                    stats.GetTemplatedStats(
                                        _session.Translation.GetTranslation(TranslationString.StatsTemplateString),
                                        _session.Translation.GetTranslation(TranslationString.StatsXpTemplateString)));
            //ProgressBar.Fill(40);

            var aggregator = new StatisticsAggregator(stats);
            if (onBotStarted != null) onBotStarted(_session, aggregator);

            //ProgressBar.Fill(50);
            var listener = new ConsoleEventListener();
            //ProgressBar.Fill(60);
            var snipeEventListener = new SniperEventListener();

            _session.EventDispatcher.EventReceived += evt => listener.Listen(evt, _session);
            _session.EventDispatcher.EventReceived += evt => aggregator.Listen(evt, _session);
            _session.EventDispatcher.EventReceived += evt => snipeEventListener.Listen(evt, _session);

            //ProgressBar.Fill(70);

            machine.SetFailureState(new LoginState());
            //ProgressBar.Fill(80);

            //ProgressBar.Fill(90);

            _session.Navigation.WalkStrategy.UpdatePositionEvent += 
                (session, lat, lng) => _session.EventDispatcher.Send(new UpdatePositionEvent { Latitude = lat, Longitude = lng });
            _session.Navigation.WalkStrategy.UpdatePositionEvent += LoadSaveState.SaveLocationToDisk;


            Navigation.GetHumanizeRouteEvent +=
                (route, destination, pokestops) => _session.EventDispatcher.Send(new GetHumanizeRouteEvent { Route = route, Destination = destination, pokeStops = pokestops });
            Navigation.GetHumanizeRouteEvent += UpdateMap;

            UseNearbyPokestopsTask.LootPokestopEvent +=
                pokestop => _session.EventDispatcher.Send(new LootPokestopEvent { Pokestop = pokestop });
            UseNearbyPokestopsTask.LootPokestopEvent += UpdateMap;

            CatchNearbyPokemonsTask.PokemonEncounterEvent +=
                mappokemons => _session.EventDispatcher.Send(new PokemonsEncounterEvent { EncounterPokemons = mappokemons });
            CatchNearbyPokemonsTask.PokemonEncounterEvent += UpdateMap;

            CatchIncensePokemonsTask.PokemonEncounterEvent +=
                mappokemons => _session.EventDispatcher.Send(new PokemonsEncounterEvent { EncounterPokemons = mappokemons });
            CatchIncensePokemonsTask.PokemonEncounterEvent += UpdateMap;

            //ProgressBar.Fill(100);

            var accountManager = new MultiAccountManager(logicSettings.Bots);

            accountManager.Add(settings.Auth.AuthConfig);

            ioc.Register<MultiAccountManager>(accountManager);

            var bot = accountManager.GetStartUpAccount();

            _session.ReInitSessionWithNextBot(bot);

            _machine = machine;
            _settings = settings;
            _excelConfigAllow = excelConfigAllow;
        }

        private async Task StartBot()
        {
            await _machine.AsyncStart(new Logic.State.VersionCheckState(), _session, _subPath, _excelConfigAllow);

            /*try
            {
                Console.Clear();
            }
            catch (IOException)
            {
            }*/

            if (_settings.TelegramConfig.UseTelegramAPI)
                _session.Telegram = new TelegramService(_settings.TelegramConfig.TelegramAPIKey, _session);

            if (_session.LogicSettings.UseSnipeLocationServer ||
                _session.LogicSettings.HumanWalkingSnipeUsePogoLocationFeeder)
                await SnipePokemonTask.AsyncStart(_session);

            if (_session.LogicSettings.EnableHumanWalkingSnipe &&
                _session.LogicSettings.HumanWalkingSnipeUseFastPokemap)
            {
                // jjskuld - Ignore CS4014 warning for now.
                #pragma warning disable 4014
                await HumanWalkSnipeTask.StartFastPokemapAsync(_session,
                    _session.CancellationTokenSource.Token); // that need to keep data live
                #pragma warning restore 4014
            }

            if (_session.LogicSettings.DataSharingConfig.EnableSyncData)
            {
                await BotDataSocketClient.StartAsync(_session);
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
        }

        private void InitializePokestopsAndRoute(List<FortData> pokeStops)
        {
            SynchronizationContext.Post(o =>
            {
                var routePoint =
                    (from pokeStop in pokeStops
                     where pokeStop != null
                     select new PointLatLng(pokeStop.Latitude, pokeStop.Longitude)).ToList();

                _routePoints = routePoint;
                togglePrecalRoute.Enabled = true;
                if (togglePrecalRoute.Checked)
                {
                    var _route = new GMapRoute(routePoint, "Walking Path")
                    {
                        Stroke = new Pen(Color.FromArgb(128, 0, 179, 253), 4)
                    };
                    _pokestopsOverlay.Routes.Add(_route);
                }

                foreach (var pokeStop in pokeStops)
                {
                    var pokeStopLoc = new PointLatLng(pokeStop.Latitude, pokeStop.Longitude);
                    var pokestopMarker = new GMapMarkerPokestops(pokeStopLoc,
                        ResourceHelper.GetImage("Pokestop"));
                    _pokestopsOverlay.Markers.Add(pokestopMarker);
                }
            }, null);
        }
		
        private void Navigation_UpdatePositionEvent(double lat, double lng)
        {
            var latlng = new PointLatLng(lat, lng);
            _playerLocations.Add(latlng);
            var currentlatlng = _currentLatLng;
            SynchronizationContext.Post(o =>
            {
                _playerOverlay.Markers.Remove(_playerMarker);
                if (!currentlatlng.IsEmpty)
                    _playerMarker = currentlatlng.Lng < latlng.Lng
                        ? new GMapMarkerTrainer(latlng, ResourceHelper.GetImage("Trainer_Right"))
                        : new GMapMarkerTrainer(latlng, ResourceHelper.GetImage("Trainer_Left"));
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
            if (togglePrecalRoute.Checked)
            {
                _pokestopsOverlay.Routes.Clear();
                var route = new GMapRoute(_routePoints, "Walking Path")
                {
                    Stroke = new Pen(Color.FromArgb(128, 0, 179, 253), 4)
                };
                _pokestopsOverlay.Routes.Add(route);
            }
            else
            {
                _pokestopsOverlay.Routes.Clear();
            }
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

        private void UpdateMap(List<GeoCoordinate> route, GeoCoordinate destination, List<FortData> pokeStops)
        {
            var routePointLatLngs = new List<PointLatLng>();
            foreach (var item in route)
            {
                routePointLatLngs.Add(new PointLatLng(item.Latitude, item.Longitude));
            }
            var routes = new GMapRoute(routePointLatLngs, routePointLatLngs.ToString())
            {
                Stroke = new Pen(Color.FromArgb(128, 0, 179, 253), 4) { DashStyle = DashStyle.Dash }
            };
            _pokestopsOverlay.Markers.Clear();
            _pokestopsOverlay.Routes.Clear();
            _playerRouteOverlay.Routes.Clear();
            _playerOverlay.Markers.Clear();
            _playerOverlay.Routes.Clear();
            _pokemonsOverlay.Markers.Clear();
            _playerLocations.Clear();
            //get optimized route
            var _pokeStops = RouteOptimizeUtil.Optimize(pokeStops.ToArray(), _session.Client.CurrentLatitude,
                _session.Client.CurrentLongitude);
            _playerRouteOverlay.Routes.Add(routes);
            InitializePokestopsAndRoute(_pokeStops);
            /* Logger.Write("new call");
             List<PointLatLng> routePointLatLngs = new List<PointLatLng>();
             Logger.Write("new route size: " +route.Count);
             PointLatLng destinationPointLatLng = new PointLatLng(destination.Latitude, destination.Longitude);
             foreach (var item in route)
             {
                 routePointLatLngs.Add(new PointLatLng(item.Latitude, item.Longitude));
             }

             List<PointLatLng> routePointsDistinct = new List<PointLatLng>(_routePoints.Distinct());

             int listPosition;
             for (listPosition = 0; listPosition < routePointsDistinct.Count; listPosition++)
             {
                 Logger.Write("listPosition: " + listPosition);
                 var item = routePointsDistinct[listPosition];
                 if (item == destinationPointLatLng)
                     break;
             }

             if (listPosition == 0)
                 return;

             //routePointsDistinct.Remove(destinationPointLatLng);
             routePointsDistinct.InsertRange(listPosition, routePointLatLngs);
             //routePointsDistinct.Remove(routePointsDistinct[listPosition - 1]);


             _pokestopsOverlay.Routes.Clear();
             var routes = new GMapRoute(routePointsDistinct, "Walking Path")
             {
                 Stroke = new Pen(Color.FromArgb(128, 0, 179, 253), 4)
             };
             _pokestopsOverlay.Routes.Add(routes);*/
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
                            _pokestopsOverlay.Markers.Remove(marker);
                    }
                }

                GMapMarker pokestopMarker = new GMapMarkerPokestops(pokeStopLoc,
                    ResourceHelper.GetImage("Pokestop_looted"));
                //pokestopMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                //pokestopMarker.ToolTip = new GMapBaloonToolTip(pokestopMarker);
                _pokestopsOverlay.Markers.Add(pokestopMarker);
            }, null);
        }

        private void UpdateMap(List<MapPokemon> encounterPokemons)
        {
            SynchronizationContext.Post(o =>
            {
                foreach (var pokemon in encounterPokemons)
                {
                    var pkmImage = ResourceHelper.GetImage("Pokemon_" + pokemon.PokemonId.GetHashCode(), 50, 50);
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
            var message = text;
            if (Instance.InvokeRequired)
            {
                Instance.Invoke(new Action<Color, string>(ColoredConsoleWrite), color, message);
                return;
            }
            Instance.logTextBox.SelectionColor = color;
            Instance.logTextBox.AppendText(message + "\n");
            Instance.logTextBox.Select(Instance.logTextBox.Text.Length, +1);
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
        }

        public static void SetStatusText(string text)
        {
            if (Instance.InvokeRequired)
            {
                Instance.Invoke(new Action<string>(SetStatusText), text);
                return;
            }
            Instance.Text = text;
            Instance.statusLabel.Text = text;
        }

        #endregion INTERFACE

        #region EVENTS

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await ReloadPokemonList();
        }

        private void startStopBotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (startStopBotToolStripMenuItem.Text.Equals(@"■ Exit RocketBot2"))
            {
                Environment.Exit(0);
            }
            else
            {
                startStopBotToolStripMenuItem.Text = @"■ Exit RocketBot2";
                btnRefresh.Enabled = true;
                //settingToolStripMenuItem.Enabled = false;
                Task.Run(StartBot);
            }
        }

        private void todoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form settingsForm = new SettingsForm(ref _settings);
            settingsForm.ShowDialog();
            var newLocation = new PointLatLng(_settings.LocationConfig.DefaultLatitude, _settings.LocationConfig.DefaultLongitude);
            gMapControl1.Position = newLocation;
            _playerMarker.Position = newLocation;
            _playerLocations.Clear();
            _playerLocations.Add(newLocation);
            UpdateMap();
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

                    e.Item.Text = _session.Translation.GetPokemonTranslation(pok.PokemonId);
                                   
                foreach (OLVListSubItem sub in e.Item.SubItems)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    if (sub.Text.Equals("Evolve") && !pok.CanEvolve)
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
                var canAllEvolve =
                    olvPokemonList.SelectedObjects.Cast<PokemonObject>().Select(o => o).All(o => o.CanEvolve);
                var count = pokemons.Count();

                if (count < 1)
                {
                    e.Cancel = true;
                }

                var pokemonObject = olvPokemonList.SelectedObjects.Cast<PokemonObject>().Select(o => o).First();

                var item = new ToolStripMenuItem();
                var separator = new ToolStripSeparator();
                item.Text = $"Transfer {count} pokemon";
                item.Click += delegate { TransferPokemon(pokemons); };
                cmsPokemonList.Items.Add(item);

                item = new ToolStripMenuItem { Text = @"Rename" };
                item.Click += delegate
                {
                    using (var form = count == 1 ? new NicknamePokemonForm(pokemonObject) : new NicknamePokemonForm())
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            NicknamePokemon(pokemons, form.txtNickname.Text);
                        }
                    }
                };
                cmsPokemonList.Items.Add(item);

                if (canAllEvolve)
                {
                    item = new ToolStripMenuItem { Text = $"Evolve {count} pokemon" };
                    item.Click += delegate { EvolvePokemon(pokemons); };
                    cmsPokemonList.Items.Add(item);
                }

                if (count != 1) return;
                item = new ToolStripMenuItem { Text = @"PowerUp" };
                item.Click += delegate { PowerUpPokemon(pokemons); };
                cmsPokemonList.Items.Add(item);
                cmsPokemonList.Items.Add(separator);

                item = new ToolStripMenuItem { Text = @"Transfer Clean Up (Keep highest IV)" };
                item.Click += delegate { CleanUpTransferPokemon(pokemonObject, "IV"); };
                cmsPokemonList.Items.Add(item);

                item = new ToolStripMenuItem { Text = @"Transfer Clean Up (Keep highest CP)" };
                item.Click += delegate { CleanUpTransferPokemon(pokemonObject, "CP"); };
                cmsPokemonList.Items.Add(item);

                item = new ToolStripMenuItem { Text = @"Evolve Clean Up (Highest IV)" };
                item.Click += delegate { CleanUpEvolvePokemon(pokemonObject, "IV"); };
                cmsPokemonList.Items.Add(item);

                item = new ToolStripMenuItem { Text = @"Evolve Clean Up (Highest CP)" };
                item.Click += delegate { CleanUpEvolvePokemon(pokemonObject, "CP"); };
                cmsPokemonList.Items.Add(item);

                cmsPokemonList.Items.Add(separator);
            };
        }

        private async void olvPokemonList_ButtonClick(object sender, CellClickEventArgs e)
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
                    EvolvePokemon(new List<PokemonData> { pokemon.PokemonData });
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex.ToString(), LogLevel.Error);
                await ReloadPokemonList();
            }
        }

        private async void TransferPokemon(IEnumerable<PokemonData> pokemons)
        {
            SetState(false);
            var _pokemons = new List<ulong>();
            foreach (var pokemon in pokemons)
            {
                _pokemons.Add(pokemon.Id);
            }
            await TransferPokemonTask.Execute(_session, _session.CancellationTokenSource.Token, _pokemons);
            await ReloadPokemonList();
        }

        private async void PowerUpPokemon(IEnumerable<PokemonData> pokemons)
        {
            SetState(false);
            foreach (var pokemon in pokemons)
            {
                await LevelUpSpecificPokemonTask.Execute(_session, pokemon.Id);
            }
            await ReloadPokemonList();
        }

        private async void EvolvePokemon(IEnumerable<PokemonData> pokemons)
        {
            SetState(false);
            foreach (var pokemon in pokemons)
            {
                await EvolveSpecificPokemonTask.Execute(_session, pokemon.Id);
            }
            await ReloadPokemonList();
        }

        private async void CleanUpTransferPokemon(PokemonObject pokemon, string type)
        {
            var et = pokemon.EvolveTimes;
            var pokemonCount =
                olvPokemonList.Objects
                    .Cast<PokemonObject>()
                    .Count(p => p.PokemonId == pokemon.PokemonId);

            if (pokemonCount < et)
            {
                await ReloadPokemonList();
                return;
            }

            if (et == 0)
                et = 1;

            if (type.Equals("IV"))
            {
                var pokemons =
                    olvPokemonList.Objects.Cast<PokemonObject>()
                        .Where(p => p.PokemonId == pokemon.PokemonId)
                        .Select(p => p.PokemonData)
                        .OrderBy(p => p.Cp)
                        .ThenBy(PokemonInfo.CalculatePokemonPerfection)
                        .Take(pokemonCount - et);

                TransferPokemon(pokemons);
            }
            else if (type.Equals("CP"))
            {
                var pokemons =
                    olvPokemonList.Objects.Cast<PokemonObject>()
                        .Where(p => p.PokemonId == pokemon.PokemonId)
                        .Select(p => p.PokemonData)
                        .OrderBy(PokemonInfo.CalculatePokemonPerfection)
                        .ThenBy(p => p.Cp)
                        .Take(pokemonCount - et);

                TransferPokemon(pokemons);
            }
        }

        private async void CleanUpEvolvePokemon(PokemonObject pokemon, string type)
        {
            var et = pokemon.EvolveTimes;

            if (et < 1)
            {
                await ReloadPokemonList();
                return;
            }

            if (type.Equals("IV"))
            {
                var pokemons =
                    olvPokemonList.Objects.Cast<PokemonObject>()
                        .Where(p => p.PokemonId == pokemon.PokemonId)
                        .Select(p => p.PokemonData)
                        .OrderByDescending(p => p.Cp)
                        .ThenByDescending(PokemonInfo.CalculatePokemonPerfection)
                        .Take(et);

                EvolvePokemon(pokemons);
            }
            else if (type.Equals("CP"))
            {
                var pokemons =
                    olvPokemonList.Objects.Cast<PokemonObject>()
                        .Where(p => p.PokemonId == pokemon.PokemonId)
                        .Select(p => p.PokemonData)
                        .OrderByDescending(PokemonInfo.CalculatePokemonPerfection)
                        .ThenByDescending(p => p.Cp)
                        .Take(et);

                EvolvePokemon(pokemons);
            }
        }

        public async void NicknamePokemon(IEnumerable<PokemonData> pokemons, string nickname)
        {
            SetState(false);
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
                await RenameSinglePokemonTask.Execute(_session, pokemon.Id, nickname,_session.CancellationTokenSource.Token);
            }
            await ReloadPokemonList();
        }

        private async Task ReloadPokemonList()
        {
            SetState(false);
            try
            {
                var itemTemplates = await _session.Client.Download.GetItemTemplates();
                var inventory =  _session.Inventory.GetCachedInventory();
                var profile = await _session.Client.Player.GetPlayer();
                var inventoryAppliedItems =  _session.Inventory.GetAppliedItems();

                var appliedItems = 
                    inventoryAppliedItems
                    .Where(aItems => aItems?.Item != null)
                    .SelectMany(aItems => aItems.Item)
                    .ToDictionary(item => item.ItemId, item => TimeHelper.FromUnixTimeUtc(item.ExpireMs));

                PokemonObject.Initilize(itemTemplates);

                var pokemons = 
                    _session.Inventory.GetPokemons()
                    .Where(p => p != null && p.PokemonId > 0)
                    .OrderByDescending(PokemonInfo.CalculatePokemonPerfection)
                    .ThenByDescending(key => key.Cp)
                    .OrderBy(key => key.PokemonId);
                                                   
                var pokemonObjects = new List<PokemonObject>();

                foreach (var pokemon in pokemons)
                {
                    var pokemonObject = new PokemonObject(pokemon);
                    pokemonObject.Candy = PokemonInfo.GetCandy(_session, pokemon);
                    pokemonObjects.Add(pokemonObject);
                }

                var prevTopItem = olvPokemonList.TopItemIndex;
                olvPokemonList.SetObjects(pokemonObjects);
                olvPokemonList.TopItemIndex = prevTopItem;

                var pokemoncount = pokemons.Count();

                var eggcount = _session.Inventory.GetEggs().Count();

                lblPokemonList.Text =
                    $"{pokemoncount + eggcount} / {profile.PlayerData.MaxPokemonStorage} ({pokemoncount} pokemon, {eggcount} eggs)";

                var items = 
                    _session.Inventory.GetItems()
                    .Where(i => i != null)
                    .OrderBy(i => i.ItemId);

                var itemscount = items.Count() +1;
                   
                    flpItems.Controls.Clear();
                    foreach (var item in items)
                    {
                        var box = new ItemBox(item);
                        if (appliedItems.ContainsKey(item.ItemId))
                            box.expires = appliedItems[item.ItemId];
                        box.ItemClick += ItemBox_ItemClick;
                        flpItems.Controls.Add(box);
                    }
            lblInventory.Text = itemscount + @" / " + profile.PlayerData.MaxItemStorage;
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
                SetState(false);
                switch (item.ItemId)
                {
                    case ItemId.ItemLuckyEgg:
                        {
                            if (_session.Client == null)
                            {
                                Logger.Write($"Bot must be running first!", LogLevel.Warning);
                                SetState(true);
                                return;
                            }
                            await UseLuckyEggTask.Execute(_session);
                        }
                        break;
                    case ItemId.ItemIncenseOrdinary:
                        {
                            if (_session.Client == null)
                            {
                                Logger.Write($"Bot must be running first!", LogLevel.Error);
                                SetState(true);
                                return;
                            }
                            await UseIncenseTask.Execute(_session);
                        }
                        break;
                    default:
                        {
                            await RecycleSpecificItemTask.Execute(_session, item.ItemId, decimal.ToInt32(form.numCount.Value));
                        }
                        break;
                }
                await ReloadPokemonList();
            }
        }

        private void SetState(bool state)
        {
            btnRefresh.Enabled = state;
            olvPokemonList.Enabled = state;
            flpItems.Enabled = state;
        }

        #endregion POKEMON LIST

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
           // Thread.Sleep(10000);
            Thread mThread = new Thread(delegate ()
            {
                 var infoForm = new InfoForm();
                infoForm.ShowDialog();
            });
            mThread.SetApartmentState(ApartmentState.STA);
             mThread.Start();
        }

        //**** Program functions
        private static void EventDispatcher_EventReceived(IEvent evt)
        {
            throw new NotImplementedException();
        }

        private static bool CheckMKillSwitch()
        {
            using (var wC = new WebClient())
            {
                try
                {
                    var strResponse1 = WebClientExtensions.DownloadString(wC, StrMasterKillSwitchUri);

                    if (strResponse1 == null)
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

                            Logger.Write(strExitMsg + $"\n" + "Please close bot to continue", LogLevel.Error);
                            //Console.ReadLine();
                            return true;
                        }
                        else
                            return false;
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

        private static bool CheckKillSwitch()
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

                            if (PromptForKillSwitchOverride(strReason))
                            {
                                // Override
                                Logger.Write("Overriding killswitch... you have been warned!", LogLevel.Warning);
                                return false;
                            }

                            Logger.Write("The bot will now close", LogLevel.Error);
                            Instance.startStopBotToolStripMenuItem.Text = @"■ Exit RocketBot2";
                            //Console.ReadLine();
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

        private static void UnhandledExceptionEventHandler(object obj, UnhandledExceptionEventArgs args)
        {
            Logger.Write("Exception caught, writing LogBuffer.", force: true);
            //throw new Exception();
        }

        public static bool PromptForKillSwitchOverride(string strReason)
        {
            Logger.Write("Do you want to override killswitch to bot at your own risk? Y/N", LogLevel.Warning);

            /*
            while (true)
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
            }
            */
            DialogResult result = MessageBox.Show("Do you want to override killswitch to bot at your own risk? Y/N \n\r" + strReason, Application.ProductName + " - Use Old API detected", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            switch (result)
            {
                case DialogResult.Yes: return true;
                case DialogResult.No: return false;
            }
            return false;
        }
    }
}
