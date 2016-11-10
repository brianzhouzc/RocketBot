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
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketBot.Logic;
using PokemonGo.RocketBot.Logic.Common;
using PokemonGo.RocketBot.Logic.Event;
using PokemonGo.RocketBot.Logic.Logging;
using PokemonGo.RocketBot.Logic.PoGoUtils;
using PokemonGo.RocketBot.Logic.Service;
using PokemonGo.RocketBot.Logic.State;
using PokemonGo.RocketBot.Logic.Tasks;
using PokemonGo.RocketBot.Logic.Utils;
using PokemonGo.RocketBot.Window.Helpers;
using PokemonGo.RocketBot.Window.Models;
using PokemonGo.RocketBot.Window.Plugin;
using POGOProtos.Data;
using POGOProtos.Inventory;
using POGOProtos.Inventory.Item;
using POGOProtos.Map.Fort;
using POGOProtos.Map.Pokemon;
using Segment;
using Segment.Model;
using Logger = PokemonGo.RocketBot.Logic.Logging.Logger;

namespace PokemonGo.RocketBot.Window.Forms
{
    public partial class MainForm : Form
    {
        public static MainForm Instance;
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

        public MainForm()
        {
            InitializeComponent();
            SynchronizationContext = SynchronizationContext.Current;
            Instance = this;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Text = @"RocketBot v" + Application.ProductVersion;

            //User activity tracking, help us get more information to make RocketBot better
            //Everything is anonymous
            Analytics.Initialize("UzL1tnZa9Yw2qcJWRIbcwGFmWGuovXez");
            Analytics.Client.Identify(MachineIdHelper.GetMachineId(), new Traits());
            Analytics.Client.Track(MachineIdHelper.GetMachineId(), "App started");

            speedLable.Parent = gMapControl1;
            showMoreCheckBox.Parent = gMapControl1;
            followTrainerCheckBox.Parent = gMapControl1;
            togglePrecalRoute.Parent = gMapControl1;

            InitializeBot();
            InitializePokemonForm();
            InitializeMap();
            VersionHelper.CheckVersion();
            if (BoolNeedsSetup)
            {
                //startStopBotToolStripMenuItem.Enabled = false;
                Logger.Write("First time here? Go to settings to set your basic info.");
                GlobalSettings.Load("");
            }
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
            gMapControl1.Zoom = 15;

            gMapControl1.Overlays.Add(_searchAreaOverlay);
            gMapControl1.Overlays.Add(_pokestopsOverlay);
            gMapControl1.Overlays.Add(_pokemonsOverlay);
            gMapControl1.Overlays.Add(_playerOverlay);
            gMapControl1.Overlays.Add(_playerRouteOverlay);

            _playerMarker = new GMapMarkerTrainer(new PointLatLng(lat, lng),
                ResourceHelper.GetImage("Trainer_Front"));
            _playerOverlay.Markers.Add(_playerMarker);
            _playerMarker.Position = new PointLatLng(lat, lng);
            _searchAreaOverlay.Polygons.Clear();
            S2GMapDrawer.DrawS2Cells(
                S2Helper.GetNearbyCellIds(lng, lat),
                _searchAreaOverlay);
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
            stats.DirtyEvent +=
                () =>
                    SetStatusText(stats.GetTemplatedStats(
                        _session.Translation.GetTranslation(TranslationString.StatsTemplateString),
                        _session.Translation.GetTranslation(TranslationString.StatsXpTemplateString)));

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
            _session.Navigation.UpdatePositionEvent += Navigation_UpdatePositionEvent;

            RouteOptimizeUtil.RouteOptimizeEvent +=
                optimizedroute =>
                    _session.EventDispatcher.Send(new OptimizeRouteEvent { OptimizedRoute = optimizedroute });
            RouteOptimizeUtil.RouteOptimizeEvent += InitializePokestopsAndRoute;

            Navigation.GetHumanizeRouteEvent +=
                (route, destination) =>
                    _session.EventDispatcher.Send(new GetHumanizeRouteEvent { Route = route, Destination = destination });
            Navigation.GetHumanizeRouteEvent += UpdateMap;

            FarmPokestopsTask.LootPokestopEvent +=
                pokestop => _session.EventDispatcher.Send(new LootPokestopEvent { Pokestop = pokestop });
            FarmPokestopsTask.LootPokestopEvent += UpdateMap;

            CatchNearbyPokemonsTask.PokemonEncounterEvent +=
                mappokemons =>
                    _session.EventDispatcher.Send(new PokemonsEncounterEvent { EncounterPokemons = mappokemons });
            CatchNearbyPokemonsTask.PokemonEncounterEvent += UpdateMap;

            CatchIncensePokemonsTask.PokemonEncounterEvent +=
                mappokemons =>
                    _session.EventDispatcher.Send(new PokemonsEncounterEvent { EncounterPokemons = mappokemons });
            CatchIncensePokemonsTask.PokemonEncounterEvent += UpdateMap;
        }

        private async Task StartBot()
        {
            await _machine.AsyncStart(new VersionCheckState(), _session);

            if (_settings.UseTelegramApi)
            {
                _session.Telegram = new TelegramService(_settings.TelegramApiKey, _session);
            }

            _settings.CheckProxy();

            QuitEvent.WaitOne();
        }

        private void InitializePokestopsAndRoute(List<FortData> pokeStops)
        {
            SynchronizationContext.Post(o =>
            {
                _pokestopsOverlay.Markers.Clear();
                _pokestopsOverlay.Routes.Clear();
                _playerOverlay.Markers.Clear();
                _playerOverlay.Routes.Clear();
                _playerLocations.Clear();
                var routePoint =
                    (from pokeStop in pokeStops
                     where pokeStop != null
                     select new PointLatLng(pokeStop.Latitude, pokeStop.Longitude)).ToList();

                _routePoints = routePoint;
                togglePrecalRoute.Enabled = true;

                var route = new GMapRoute(routePoint, "Walking Path")
                {
                    Stroke = new Pen(Color.FromArgb(128, 0, 179, 253), 4)
                };
                _pokestopsOverlay.Routes.Add(route);

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

        private void UpdateMap(List<GeoCoordinate> route, GeoCoordinate destination)
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
            _playerRouteOverlay.Routes.Add(routes);
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
                _pokemonsOverlay.Markers.Clear();

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
            Instance.logTextBox.SelectionStart = Instance.logTextBox.Text.Length;
            Instance.logTextBox.ScrollToCaret();
            Instance.logTextBox.SelectionColor = color;
            Instance.logTextBox.AppendText(message);
        }

        public static void SetSpeedLable(string text)
        {
            if (Instance.InvokeRequired)
            {
                Instance.Invoke(new Action<string>(SetSpeedLable), text);
                return;
            }
            Instance.speedLable.Text = text;
        }

        public static void SetStatusText(string text)
        {
            if (Instance.InvokeRequired)
            {
                Instance.Invoke(new Action<string>(SetStatusText), text);
                return;
            }
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
            startStopBotToolStripMenuItem.Enabled = false;
            Task.Run(StartBot);
        }

        private void todoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form settingsForm = new SettingsForm(ref _settings);
            settingsForm.ShowDialog();
            var newLocation = new PointLatLng(_settings.DefaultLatitude, _settings.DefaultLongitude);
            gMapControl1.Position = newLocation;
            _playerMarker.Position = newLocation;
            _playerLocations.Clear();
            _playerLocations.Add(newLocation);
            UpdateMap();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Analytics.Client.Track(MachineIdHelper.GetMachineId(), "App stopped");
            Analytics.Dispose();
            //Environment.Exit(0);
        }

        #endregion EVENTS

        #region POKEMON LIST

        private IEnumerable<Candy> _families;

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
            foreach (var pokemon in pokemons)
            {
                await TransferSpecificPokemonTask.Execute(_session, pokemon.Id);
            }
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
                await RenameSpecificPokemonTask.Execute(_session, pokemon, nickname);
            }
            await ReloadPokemonList();
        }

        private async Task ReloadPokemonList()
        {
            SetState(false);
            try
            {
                await _session.Inventory.RefreshCachedInventory();
                var itemTemplates = await _session.Client.Download.GetItemTemplates();
                var inventory = await _session.Inventory.GetCachedInventory();
                var profile = await _session.Client.Player.GetPlayer();
                var inventoryAppliedItems =
                    await _session.Inventory.GetAppliedItems();

                var appliedItems =
                    inventoryAppliedItems.Where(aItems => aItems?.Item != null)
                        .SelectMany(aItems => aItems.Item)
                        .ToDictionary(item => item.ItemId, item => TimeHelper.FromUnixTimeUtc(item.ExpireMs));

                PokemonObject.Initilize(itemTemplates);

                var pokemons =
                    inventory.InventoryDelta.InventoryItems.Select(i => i?.InventoryItemData?.PokemonData)
                        .Where(p => p != null && p.PokemonId > 0)
                        .OrderByDescending(PokemonInfo.CalculatePokemonPerfection)
                        .ThenByDescending(key => key.Cp)
                        .OrderBy(key => key.PokemonId);
                _families = inventory.InventoryDelta.InventoryItems
                    .Select(i => i.InventoryItemData.Candy)
                    .Where(p => p != null && p.FamilyId > 0)
                    .OrderByDescending(p => p.FamilyId);

                var pokemonObjects = new List<PokemonObject>();
                foreach (var pokemon in pokemons)
                {
                    var pokemonObject = new PokemonObject(pokemon);
                    var family = _families.First(i => (int)i.FamilyId <= (int)pokemon.PokemonId);
                    pokemonObject.Candy = family.Candy_;
                    pokemonObjects.Add(pokemonObject);
                }

                var prevTopItem = olvPokemonList.TopItemIndex;
                olvPokemonList.SetObjects(pokemonObjects);
                olvPokemonList.TopItemIndex = prevTopItem;

                var pokemoncount =
                    inventory.InventoryDelta.InventoryItems
                        .Select(i => i.InventoryItemData?.PokemonData)
                        .Count(p => p != null && p.PokemonId > 0);
                var eggcount =
                    inventory.InventoryDelta.InventoryItems
                        .Select(i => i.InventoryItemData?.PokemonData)
                        .Count(p => p != null && p.IsEgg);
                lblPokemonList.Text =
                    $"{pokemoncount + eggcount} / {profile.PlayerData.MaxPokemonStorage} ({pokemoncount} pokemon, {eggcount} eggs)";

                var items =
                    inventory.InventoryDelta.InventoryItems
                        .Select(i => i.InventoryItemData?.Item)
                        .Where(i => i != null)
                        .OrderBy(i => i.ItemId);
                var itemscount =
                    inventory.InventoryDelta.InventoryItems
                        .Select(i => i.InventoryItemData?.Item)
                        .Where(i => i != null)
                        .Sum(i => i.Count) + 1;

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
                            await
                                RecycleSpecificItemTask.Execute(_session, item.ItemId, decimal.ToInt32(form.numCount.Value));
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
    }
}