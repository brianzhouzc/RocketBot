using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
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
using PokemonGo.RocketBot.Logic.Utils;
using PokemonGo.RocketBot.Window.Helpers;
using PokemonGo.RocketBot.Window.Models;
using PokemonGo.RocketBot.Window.Plugin;
using POGOProtos.Data;
using POGOProtos.Inventory;
using POGOProtos.Inventory.Item;
using POGOProtos.Map.Fort;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketBot.Logic.Tasks;

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
        private readonly GMapOverlay _pokemonsOverlay = new GMapOverlay("pokemons");
        private readonly GMapOverlay _pokestopsOverlay = new GMapOverlay("pokestops");
        private readonly GMapOverlay _searchAreaOverlay = new GMapOverlay("areas");
        private ConsoleLogger _logger;
        private StateMachine _machine;
        private List<FortData> _pokeStops;
        private GlobalSettings _settings;

        public MainForm()
        {
            InitializeComponent();
            SynchronizationContext = SynchronizationContext.Current;
            Instance = this;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitializeBot();
            InitializePokemonForm();
            InitializeMap();
            CheckVersion();
            if (BoolNeedsSetup)
            {
                startStopBotToolStripMenuItem.Enabled = false;
                Logger.Write("First time here? Go to settings to set your basic info.");
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

            _playerMarker = new GMapMarkerTrainer(new PointLatLng(lat, lng),
                (Image)Properties.Resources.ResourceManager.GetObject("Trainer"));
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
            var configFile = Path.Combine(profileConfigPath, "config.json");

            BoolNeedsSetup = false;

            if (File.Exists(configFile))
            {
                /** if (!VersionCheckState.IsLatest())
                    settings = GlobalSettings.Load(subPath, true);
                else **/
                _settings = GlobalSettings.Load(subPath);
            }
            else
            {
                _settings = new GlobalSettings();
                _settings.ProfilePath = profilePath;
                _settings.ProfileConfigPath = profileConfigPath;
                _settings.GeneralConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "config");
                _settings.TranslationLanguageCode = strCulture;
                BoolNeedsSetup = true;
            }

            _session = new Session(new ClientSettings(_settings), new LogicSettings(_settings));

            if (BoolNeedsSetup)
            {
                menuStrip1.ShowItemToolTips = true;
                startStopBotToolStripMenuItem.ToolTipText = @"Please goto settings and enter your basic info";
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
            _session.Client.ApiFailure = new ApiFailureStrategy(_session);

            _machine = new StateMachine();
            var stats = new Statistics();

            var strVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

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
            RouteOptimizeUtil.RouteOptimizeEvent += Visualize;

            FarmPokestopsTask.LootPokestopEvent +=
                pokestop => _session.EventDispatcher.Send(new LootPokestopEvent { Pokestop = pokestop });
            FarmPokestopsTask.LootPokestopEvent += Update;
        }

        private async Task StartBot()
        {
            _machine.AsyncStart(new VersionCheckState(), _session);

            if (_settings.UseTelegramAPI)
            {
                _session.Telegram = new TelegramService(_settings.TelegramAPIKey, _session);
            }

            _settings.checkProxy();

            QuitEvent.WaitOne();
        }

        private async void Visualize(List<FortData> pokeStops)
        {
            _pokestopsOverlay.Markers.Clear();
            var routePoint =
                (from pokeStop in pokeStops
                 where pokeStop != null
                 select new PointLatLng(pokeStop.Latitude, pokeStop.Longitude)).ToList();
            _pokestopsOverlay.Routes.Clear();
            var route = new GMapRoute(routePoint, "Walking Path");
            route.Stroke = new Pen(Color.FromArgb(128, 0, 179, 253), 4);
            _pokestopsOverlay.Routes.Add(route);

            foreach (var pokeStop in pokeStops)
            {
                var pokeStopLoc = new PointLatLng(pokeStop.Latitude, pokeStop.Longitude);
                var pokestopMarker = new GMapMarkerPokestops(pokeStopLoc,
                    (Image)Properties.Resources.ResourceManager.GetObject("Pokestop"));
                _pokestopsOverlay.Markers.Add(pokestopMarker);
            }

        }

        private new void Update(FortData pokestop = null)
        {
            SynchronizationContext.Post(o =>
            {
                if (pokestop != null)
                {
                    var pokeStopLoc = new PointLatLng(pokestop.Latitude, pokestop.Longitude);

                    lock (_pokestopsOverlay.Markers)
                    {
                        for (int i = 0; i < _pokestopsOverlay.Markers.Count; i++)
                        {
                            var marker = _pokestopsOverlay.Markers[i];
                            if (marker.Position == pokeStopLoc)
                                _pokestopsOverlay.Markers.Remove(marker);
                        }
                    }

                    GMapMarker pokestopMarker = new GMapMarkerPokestops(pokeStopLoc,
                        (Image)Properties.Resources.ResourceManager.GetObject("Pokestop_looted"));
                    //pokestopMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                    //pokestopMarker.ToolTip = new GMapBaloonToolTip(pokestopMarker);
                    _pokestopsOverlay.Markers.Add(pokestopMarker);
                }

                var route = new GMapRoute(_playerLocations, "step");
                route.Stroke = new Pen(Color.FromArgb(175, 175, 175), 2);
                route.Stroke.DashStyle = DashStyle.Dot;
                _playerOverlay.Routes.Add(route);
                _playerOverlay.Routes.Clear();
                _playerOverlay.Routes.Add(route);
            }, null);
        }

        private void Navigation_UpdatePositionEvent(double lat, double lng)
        {
            var latlng = new PointLatLng(lat, lng);
            _playerLocations.Add(latlng);
            _playerMarker.Position = latlng;
            Update();
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
                Logger.Write("Your version is " + Assembly.GetExecutingAssembly().GetName().Version);
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

        #region BUTTONS

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ReloadPokemonList();
        }

        private void startStopBotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Run(() => StartBot());
        }

        private void todoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }

        #endregion BUTTONS

        #region POKEMON LIST

        private IEnumerable<Candy> _families;

        private void InitializePokemonForm()
        {
            //olvPokemonList.ButtonClick += PokemonListButton_Click;

            pkmnName.ImageGetter = delegate (object rowObject)
            {
                var pokemon = rowObject as PokemonObject;

                var key = pokemon.PokemonId.ToString();
                if (!olvPokemonList.SmallImageList.Images.ContainsKey(key))
                {
                    var img = GetPokemonImage((int)pokemon.PokemonId);
                    olvPokemonList.SmallImageList.Images.Add(key, img);
                }
                return key;
            };

            olvPokemonList.FormatRow += delegate (object sender, FormatRowEventArgs e)
            {
                var pok = e.Model as PokemonObject;
                if (olvPokemonList.Objects.Cast<PokemonObject>()
                    .Select(i => i.PokemonId)
                    .Where(p => p == pok.PokemonId)
                    .Count() > 1)
                    e.Item.BackColor = Color.LightGreen;

                foreach (OLVListSubItem sub in e.Item.SubItems)
                {
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

                var pokemons = olvPokemonList.SelectedObjects.Cast<PokemonObject>().Select(o => o.PokemonData);
                var canAllEvolve =
                    olvPokemonList.SelectedObjects.Cast<PokemonObject>()
                        .Select(o => o)
                        .Where(o => o.CanEvolve == false)
                        .Count() == 0;
                var count = pokemons.Count();

                if (count < 1)
                {
                    e.Cancel = true;
                }

                var pokemonObject = olvPokemonList.SelectedObjects.Cast<PokemonObject>().Select(o => o).First();

                var item = new ToolStripMenuItem();
                var separator = new ToolStripSeparator();
                item.Text = "Transfer " + count + " pokemon";
                item.Click += delegate { TransferPokemon(pokemons); };
                cmsPokemonList.Items.Add(item);

                item = new ToolStripMenuItem();
                item.Text = "Rename";
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
                    item = new ToolStripMenuItem();
                    item.Text = "Evolve " + count + " pokemon";
                    item.Click += delegate { EvolvePokemon(pokemons); };
                    cmsPokemonList.Items.Add(item);
                }

                if (count == 1)
                {
                    item = new ToolStripMenuItem();
                    item.Text = "PowerUp";
                    item.Click += delegate { PowerUpPokemon(pokemons); };
                    cmsPokemonList.Items.Add(item);

                    cmsPokemonList.Items.Add(separator);

                    item = new ToolStripMenuItem();
                    item.Text = "Transfer Clean Up (Keep highest IV)";
                    item.Click += delegate { CleanUpTransferPokemon(pokemonObject, "IV"); };
                    cmsPokemonList.Items.Add(item);

                    item = new ToolStripMenuItem();
                    item.Text = "Transfer Clean Up (Keep highest CP)";
                    item.Click += delegate { CleanUpTransferPokemon(pokemonObject, "CP"); };
                    cmsPokemonList.Items.Add(item);

                    item = new ToolStripMenuItem();
                    item.Text = "Evolve Clean Up (Highest IV)";
                    item.Click += delegate { CleanUpEvolvePokemon(pokemonObject, "IV"); };
                    cmsPokemonList.Items.Add(item);

                    item = new ToolStripMenuItem();
                    item.Text = "Evolve Clean Up (Highest CP)";
                    item.Click += delegate { CleanUpEvolvePokemon(pokemonObject, "CP"); };
                    cmsPokemonList.Items.Add(item);

                    cmsPokemonList.Items.Add(separator);
                }
            };
        }

        private async void TransferPokemon(IEnumerable<PokemonData> pokemons)
        {
            SetState(false);
            foreach (var pokemon in pokemons)
            {
                var transferPokemonResponse = await _session.Client.Inventory.TransferPokemon(pokemon.Id);
                if (transferPokemonResponse.Result == ReleasePokemonResponse.Types.Result.Success)
                {
                    Logger.Write(
                        $"{pokemon.PokemonId} was transferred. {transferPokemonResponse.CandyAwarded} candy awarded",
                        LogLevel.Transfer);
                }
                else
                {
                    Logger.Write($"{pokemon.PokemonId} could not be transferred", LogLevel.Error);
                }
            }
            ReloadPokemonList();
        }

        private async void PowerUpPokemon(IEnumerable<PokemonData> pokemons)
        {
            SetState(false);
            foreach (var pokemon in pokemons)
            {
                var evolvePokemonResponse = await _session.Client.Inventory.UpgradePokemon(pokemon.Id);
                if (evolvePokemonResponse.Result == UpgradePokemonResponse.Types.Result.Success)
                {
                    Logger.Write($"{pokemon.PokemonId} successfully upgraded.");
                }
                else
                {
                    Logger.Write($"{pokemon.PokemonId} could not be upgraded");
                }
            }
            ReloadPokemonList();
        }

        private async void EvolvePokemon(IEnumerable<PokemonData> pokemons)
        {
            SetState(false);
            foreach (var pokemon in pokemons)
            {
                var evolvePokemonResponse = await _session.Client.Inventory.EvolvePokemon(pokemon.Id);
                if (evolvePokemonResponse.Result == EvolvePokemonResponse.Types.Result.Success)
                {
                    Logger.Write(
                        $"{pokemon.PokemonId} successfully evolved into {evolvePokemonResponse.EvolvedPokemonData.PokemonId}\n{evolvePokemonResponse.ExperienceAwarded} experience awarded\n{evolvePokemonResponse.CandyAwarded} candy awarded");
                }
                else
                {
                    Logger.Write($"{pokemon.PokemonId} could not be evolved");
                }
            }
            ReloadPokemonList();
        }

        private void CleanUpTransferPokemon(PokemonObject pokemon, string type)
        {
            var ET = pokemon.EvolveTimes;
            var pokemonCount =
                olvPokemonList.Objects.Cast<PokemonObject>()
                    .Where(p => p.PokemonId == pokemon.PokemonId)
                    .Count();

            if (pokemonCount < ET)
            {
                ReloadPokemonList();
                return;
            }

            if (ET == 0)
                ET = 1;

            if (type.Equals("IV"))
            {
                var pokemons =
                    olvPokemonList.Objects.Cast<PokemonObject>()
                        .Where(p => p.PokemonId == pokemon.PokemonId)
                        .Select(p => p.PokemonData)
                        .OrderBy(p => p.Cp)
                        .OrderBy(PokemonInfo.CalculatePokemonPerfection)
                        .Take(pokemonCount - ET);

                TransferPokemon(pokemons);
            }
            else if (type.Equals("CP"))
            {
                var pokemons =
                    olvPokemonList.Objects.Cast<PokemonObject>()
                        .Where(p => p.PokemonId == pokemon.PokemonId)
                        .Select(p => p.PokemonData)
                        .OrderBy(PokemonInfo.CalculatePokemonPerfection)
                        .OrderBy(p => p.Cp)
                        .Take(pokemonCount - ET);

                TransferPokemon(pokemons);
            }
        }

        private void CleanUpEvolvePokemon(PokemonObject pokemon, string type)
        {
            var ET = pokemon.EvolveTimes;

            if (ET < 1)
            {
                ReloadPokemonList();
                return;
            }

            if (type.Equals("IV"))
            {
                var pokemons =
                    olvPokemonList.Objects.Cast<PokemonObject>()
                        .Where(p => p.PokemonId == pokemon.PokemonId)
                        .Select(p => p.PokemonData)
                        .OrderByDescending(p => p.Cp)
                        .OrderByDescending(PokemonInfo.CalculatePokemonPerfection)
                        .Take(ET);

                EvolvePokemon(pokemons);
            }
            else if (type.Equals("CP"))
            {
                var pokemons =
                    olvPokemonList.Objects.Cast<PokemonObject>()
                        .Where(p => p.PokemonId == pokemon.PokemonId)
                        .Select(p => p.PokemonData)
                        .OrderByDescending(PokemonInfo.CalculatePokemonPerfection)
                        .OrderByDescending(p => p.Cp)
                        .Take(ET);

                EvolvePokemon(pokemons);
            }
        }

        public async void NicknamePokemon(IEnumerable<PokemonData> pokemons, string nickname)
        {
            SetState(false);
            foreach (var pokemon in pokemons)
            {
                var newName = new StringBuilder(nickname);
                newName.Replace("{Name}", Convert.ToString(pokemon.PokemonId));
                newName.Replace("{CP}", Convert.ToString(pokemon.Cp));
                newName.Replace("{IV}", Convert.ToString(Math.Round(_session.Inventory.GetPerfect(pokemon))));
                newName.Replace("{IA}", Convert.ToString(pokemon.IndividualAttack));
                newName.Replace("{ID}", Convert.ToString(pokemon.IndividualDefense));
                newName.Replace("{IS}", Convert.ToString(pokemon.IndividualStamina));
                if (nickname.Length > 12)
                {
                    Logger.Write($"\"{newName}\" is too long, please choose another name");
                    if (pokemons.Count() == 1)
                    {
                        SetState(true);
                        return;
                    }
                    continue;
                }
                var response = await _session.Client.Inventory.NicknamePokemon(pokemon.Id, newName.ToString());
                if (response.Result == NicknamePokemonResponse.Types.Result.Success)
                {
                    Logger.Write($"Successfully renamed {pokemon.PokemonId} to \"{newName}\"");
                }
                else
                {
                    Logger.Write($"Failed renaming {pokemon.PokemonId} to \"{newName}\"");
                }
                await Task.Delay(1500);
            }
            await ReloadPokemonList();
        }

        private Image GetPokemonImage(int pokemonId)
        {
            return (Image)Properties.Resources.ResourceManager.GetObject("Pokemon_" + pokemonId);
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
                var appliedItems = new Dictionary<ItemId, DateTime>();
                var inventoryAppliedItems =
                    await _session.Inventory.GetAppliedItems();

                foreach (var aItems in inventoryAppliedItems)
                {
                    if (aItems != null && aItems.Item != null)
                    {
                        foreach (var item in aItems.Item)
                        {
                            appliedItems.Add(item.ItemId, Utils.FromUnixTimeUtc(item.ExpireMs));
                        }
                    }
                }

                PokemonObject.Initilize(itemTemplates);

                var pokemons =
                    inventory.InventoryDelta.InventoryItems.Select(i => i?.InventoryItemData?.PokemonData)
                        .Where(p => p != null && p?.PokemonId > 0)
                        .OrderByDescending(PokemonInfo.CalculatePokemonPerfection)
                        .OrderByDescending(key => key.Cp)
                        .OrderBy(key => key.PokemonId);
                _families = inventory.InventoryDelta.InventoryItems
                    .Select(i => i.InventoryItemData.Candy)
                    .Where(p => p != null && p.FamilyId > 0)
                    .OrderByDescending(p => p.FamilyId);

                var pokemonObjects = new List<PokemonObject>();
                foreach (var pokemon in pokemons)
                {
                    var pokemonObject = new PokemonObject(pokemon);
                    var family =
                        _families.Where(i => (int)i.FamilyId <= (int)pokemon.PokemonId)
                            .First();
                    pokemonObject.Candy = family.Candy_;
                    pokemonObjects.Add(pokemonObject);
                }

                var prevTopItem = olvPokemonList.TopItemIndex;
                olvPokemonList.SetObjects(pokemonObjects);
                olvPokemonList.TopItemIndex = prevTopItem;

                var pokemoncount =
                    inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonData)
                        .Where(p => p != null && p?.PokemonId > 0)
                        .Count();
                var eggcount =
                    inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonData)
                        .Where(p => p != null && p?.IsEgg == true)
                        .Count();
                lblPokemonList.Text = pokemoncount + eggcount + " / " + profile.PlayerData.MaxPokemonStorage + " (" +
                                      pokemoncount + " pokemon, " + eggcount + " eggs)";

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

                lblInventory.Text = itemscount + " / " + profile.PlayerData.MaxItemStorage;
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
                if (result == DialogResult.OK)
                {
                    SetState(false);
                    if (item.ItemId == ItemId.ItemLuckyEgg)
                    {
                        if (_session.Client == null)
                        {
                            Logger.Write($"Bot must be running first!", LogLevel.Warning);
                            SetState(true);
                            return;
                        }
                        var response = await _session.Client.Inventory.UseItemXpBoost();
                        if (response.Result == UseItemXpBoostResponse.Types.Result.Success)
                        {
                            Logger.Write($"Using a Lucky Egg");
                            Logger.Write($"Lucky Egg valid until: {DateTime.Now.AddMinutes(30)}");
                        }
                        else if (response.Result == UseItemXpBoostResponse.Types.Result.ErrorXpBoostAlreadyActive)
                        {
                            Logger.Write($"A Lucky Egg is already active!", LogLevel.Warning);
                        }
                        else if (response.Result == UseItemXpBoostResponse.Types.Result.ErrorLocationUnset)
                        {
                            Logger.Write($"Bot must be running first!", LogLevel.Error);
                        }
                        else
                        {
                            Logger.Write($"Failed using a Lucky Egg!", LogLevel.Error);
                        }
                    }
                    else if (item.ItemId == ItemId.ItemIncenseOrdinary)
                    {
                        if (_session.Client == null)
                        {
                            Logger.Write($"Bot must be running first!", LogLevel.Error);
                            SetState(true);
                            return;
                        }
                        var response = await _session.Client.Inventory.UseIncense(ItemId.ItemIncenseOrdinary);
                        if (response.Result == UseIncenseResponse.Types.Result.Success)
                        {
                            Logger.Write($"Incense valid until: {DateTime.Now.AddMinutes(30)}");
                        }
                        else if (response.Result == UseIncenseResponse.Types.Result.IncenseAlreadyActive)
                        {
                            Logger.Write($"An incense is already active!", LogLevel.Warning);
                        }
                        else if (response.Result == UseIncenseResponse.Types.Result.LocationUnset)
                        {
                            Logger.Write($"Bot must be running first!", LogLevel.Error);
                        }
                        else
                        {
                            Logger.Write($"Failed using an incense!", LogLevel.Error);
                        }
                    }
                    else
                    {
                        var response =
                            await
                                _session.Client.Inventory.RecycleItem(item.ItemId, decimal.ToInt32(form.numCount.Value));
                        if (response.Result == RecycleInventoryItemResponse.Types.Result.Success)
                        {
                            Logger.Write(
                                $"Recycled {decimal.ToInt32(form.numCount.Value)}x {item.ItemId.ToString().Substring(4)}", LogLevel.Recycling);
                        }
                        else
                        {
                            Logger.Write(
                                $"Unable to recycle {decimal.ToInt32(form.numCount.Value)}x {item.ItemId.ToString().Substring(4)}", LogLevel.Error);
                        }
                    }
                    ReloadPokemonList();
                }
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