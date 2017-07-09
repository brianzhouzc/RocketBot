#region using directives

using BrightIdeasSoftware;
using GeoCoordinatePortable;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.ToolTips;
using PoGo.NecroBot.Logic;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.Logging;
using PoGo.NecroBot.Logic.Model.Settings;
using PoGo.NecroBot.Logic.PoGoUtils;
using PoGo.NecroBot.Logic.Service;
using PoGo.NecroBot.Logic.Service.Elevation;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Tasks;
using PoGo.NecroBot.Logic.Utils;
using POGOProtos.Data;
using POGOProtos.Data.Raid;
using POGOProtos.Inventory.Item;
using POGOProtos.Map.Fort;
using POGOProtos.Map.Pokemon;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Helpers;
using RocketBot2.CommandLineUtility;
using RocketBot2.Helpers;
using RocketBot2.Models;
using RocketBot2.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

#endregion


namespace RocketBot2.Forms
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        #region INITIALIZE

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
        // layers
        internal readonly GMapOverlay _playerOverlay = new GMapOverlay("players");
        internal readonly GMapOverlay _playerRouteOverlay = new GMapOverlay("playerroutes");
        internal readonly GMapOverlay _pokemonsOverlay = new GMapOverlay("pokemons");
        internal readonly GMapOverlay _pokestopsOverlay = new GMapOverlay("pokestops");
        internal readonly GMapOverlay _searchAreaOverlay = new GMapOverlay("areas");

        private const int DefaultZoomLevel = 15;

        public static Session _session;

        public MainForm(string[] _args)
        {
            InitializeComponent();
            SynchronizationContext = SynchronizationContext.Current;
            Instance = this;
            args = _args;
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(ErrorHandler);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.splitContainer1.SplitterDistance = this.splitContainer1.Width / 100 * 45; // Splits left & right splitter panes @ 45%/55% of the window width
            this.splitContainer2.SplitterDistance = this.splitContainer2.Height / 100 * 45;// Always keeps the logger window @ 45%/55% of the window height
            this.Refresh(); // Force screen refresh before items are poppulated
            SetStatusText(Application.ProductName + " " + Application.ProductVersion);
            speedLable.Parent = GMapControl1;
            showMoreCheckBox.Parent = GMapControl1;
            followTrainerCheckBox.Parent = GMapControl1;
            togglePrecalRoute.Parent = GMapControl1;
            GMAPSatellite.Parent = GMapControl1;
            InitializeBot(null);
            if (!_settings.WebsocketsConfig.UseWebsocket) menuStrip1.Items.Remove(pokeEaseToolStripMenuItem);
            InitializePokemonForm();
            InitializeMap();
            VersionHelper.CheckVersion();
            btnRefresh.Enabled = false;
            //ConsoleHelper.HideConsoleWindow();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            TrayIcon.Visible = false;
            if (FormWindowState.Minimized == this.WindowState)
            {
                TrayIcon.BalloonTipIcon = ToolTipIcon.Info; //Shows the info icon so the user doesn't thing there is an error.
                TrayIcon.BalloonTipText = "RocketBot2 is minimized, click on this icon to restore";
                TrayIcon.BalloonTipTitle = "RocketBot2 is minimized";
                TrayIcon.Text = "RocketBot2 is minimized, click on this icon to restore";
                TrayIcon.Visible = true;
                TrayIcon.ShowBalloonTip(5000);
                Hide();
            }
        }

        private void TrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Maximized;
            this.Refresh();
        }
        #endregion

        #region INTERFACE

        private static DateTime LastClearLog = DateTime.Now;

        public static void ColoredConsoleWrite(Color color, string text)
        {
            if (text.Length <= 0)
                return;

            if (Instance.InvokeRequired)
            {
                Instance.Invoke(new Action<Color, string>(ColoredConsoleWrite), color, text);
                return;
            }

            if (LastClearLog.AddMinutes(20) < DateTime.Now)
            {
                Instance.logTextBox.Text = string.Empty;
                LastClearLog = DateTime.Now;
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
            Instance.Navigation_UpdatePositionEvent();

            Instance.togglePrecalRoute.Enabled = Instance._botStarted;
            Instance.followTrainerCheckBox.Enabled = Instance._botStarted;
        }

        public void SetStatusText(string text)
        {
            if (Instance.InvokeRequired)
            {
                Instance.Invoke(new Action<string>(SetStatusText), text);
                return;
            }
            Instance.Text = text;
            Instance.statusLabel.Text = text;
            Console.Title = text;

            SetState(true);

            if (checkBoxAutoRefresh.Checked)
                ReloadPokemonList().ConfigureAwait(false);
        }

        #endregion INTERFACE

        #region GMAP

        private void InitializeMap()
        {
            var lat = _session.Client.Settings.DefaultLatitude;
            var lng = _session.Client.Settings.DefaultLongitude;
            GMapControl1.MapProvider = GoogleMapProvider.Instance;
            GMapControl1.Manager.Mode = AccessMode.ServerOnly;
            GMapProvider.WebProxy = null;
            GMapControl1.Position = new PointLatLng(lat, lng);
            GMapControl1.DragButton = MouseButtons.Left;

            GMapControl1.MinZoom = 2;
            GMapControl1.MaxZoom = 18;
            
            GMapControl1.Overlays.Add(_searchAreaOverlay);
            GMapControl1.Overlays.Add(_pokestopsOverlay);
            GMapControl1.Overlays.Add(_pokemonsOverlay);
            GMapControl1.Overlays.Add(_playerOverlay);
            GMapControl1.Overlays.Add(_playerRouteOverlay);

            _playerMarker = new GMapMarkerTrainer(new PointLatLng(lat, lng), ResourceHelper.GetImage("PlayerLocation", null, null, 32, 32));
            _playerOverlay.Markers.Add(_playerMarker);
            _playerMarker.Position = new PointLatLng(lat, lng);
            _searchAreaOverlay.Polygons.Clear();
            S2GMapDrawer.DrawS2Cells(S2Helper.GetNearbyCellIds(lng, lat), _searchAreaOverlay);
            trackBar.Value = DefaultZoomLevel;
            GMapControl1.OnMapZoomChanged += delegate { trackBar.Value = (int)GMapControl1.Zoom; };
        }

        private void GMAPSatellite_CheckedChanged(object sender, EventArgs e)
        {
            if (GMAPSatellite.Checked)
                GMapControl1.MapProvider = GoogleSatelliteMapProvider.Instance;
            else
                GMapControl1.MapProvider = GoogleMapProvider.Instance;
        }

        private Task InitializePokestopsAndRoute()
        {
            //get optimized route
            var pokeStops = RouteOptimizeUtil.Optimize(_session.Forts.ToArray(), _session.Client.CurrentLatitude,
                    _session.Client.CurrentLongitude);

            SynchronizationContext.Post(o =>
            {
                _playerOverlay.Markers.Clear();
                _pokemonsOverlay.Markers.Clear();
                _pokestopsOverlay.Markers.Clear();
                _pokestopsOverlay.Routes.Clear();
                _playerLocations.Clear();

                _routePoints =
                    (from pokeStop in pokeStops
                     where pokeStop != null
                     select new PointLatLng(pokeStop.Latitude, pokeStop.Longitude)).ToList();

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


                    bool isRaid = false;
                    bool asBoss = false;

                    try
                    {
                        if (pokeStop.RaidInfo.RaidBattleMs > 0)
                            isRaid = true;
                    }
                    catch
                    {
                        isRaid = false;
                    }

                    try
                    {
                        if (!string.IsNullOrEmpty(pokeStop.RaidInfo.RaidPokemon.PokemonId.ToString()))
                            asBoss = true;
                    }
                    catch
                    {
                        asBoss = false;
                    }

                    int hg = 32;
                    int wg = 32;
                    Image ImgGymBoss = null;

                    if (asBoss)
                    {
                        hg = 48;
                        wg = 48;
                        ImgGymBoss = ResourceHelper.GetImage(null, pokeStop.RaidInfo.RaidPokemon, null, 32, 32);
                    }

                    string raid = isRaid ? "Raid" : null;

                    switch (pokeStop.Type)
                    {
                        case FortType.Checkpoint:
                            fort = ResourceHelper.GetImage("Pokestop", null, null, 32, 32);
                            break;
                        case FortType.Gym:
                            switch (pokeStop.OwnedByTeam)
                            { 
                                case POGOProtos.Enums.TeamColor.Neutral:
                                    if (asBoss)
                                        fort = ResourceHelper.CombineImages(ResourceHelper.GetImage("GymVide", null, null, hg, wg), ImgGymBoss);
                                    else
                                        fort = ResourceHelper.GetImage($"GymVide{raid}", null, null, hg, wg);
                                    break;
                                case POGOProtos.Enums.TeamColor.Blue:
                                    if (asBoss)
                                        fort = ResourceHelper.CombineImages(ResourceHelper.GetImage("GymBlue", null, null, hg, wg), ImgGymBoss);
                                    else
                                        fort = ResourceHelper.GetImage($"GymBlue{raid}", null, null, hg, wg);
                                    break;
                                case POGOProtos.Enums.TeamColor.Red:
                                    if (asBoss)
                                        fort = ResourceHelper.CombineImages(ResourceHelper.GetImage("GymRed", null, null, hg, wg), ImgGymBoss);
                                    else
                                        fort = ResourceHelper.GetImage($"GymRed{raid}", null, null, hg, wg);
                                    break;
                                case POGOProtos.Enums.TeamColor.Yellow:
                                    if (asBoss)
                                        fort = ResourceHelper.CombineImages(ResourceHelper.GetImage("GymYellow", null, null, hg, wg), ImgGymBoss);
                                    else
                                        fort = ResourceHelper.GetImage($"GymYellow{raid}", null, null, hg, wg);
                                    break;
                            }
                            break;
                        default:
                            fort = ResourceHelper.GetImage("Pokestop", null, null, 32, 32);
                            break;
                    }

                    var pokestopMarker = new GMapMarkerPokestops(pokeStopLoc, fort);
                    if (isRaid || asBoss)
                    {
                        pokestopMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                        pokestopMarker.ToolTip = new GMapBaloonToolTip(pokestopMarker);
                        if (isRaid)
                        {
                            var tm = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(pokeStop.RaidInfo.RaidBattleMs);
                            var time = tm - DateTime.UtcNow;
                            pokestopMarker.ToolTipText = $"Raid start in: {time.Hours}h {time.Minutes}m"; //{Math.Abs(time.Seconds)}s";
                        }
                        else
                        {
                            var tm = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(pokeStop.RaidInfo.RaidEndMs);
                            var time = tm - DateTime.UtcNow;
                            pokestopMarker.ToolTipText = $"Raid end in: {time.Hours}h {time.Minutes}m"; // {Math.Abs(time.Seconds)}s";
                        }
                    }
                    _pokestopsOverlay.Markers.Add(pokestopMarker);
                }
                Navigation_UpdatePositionEvent();
            }, null);
            return Task.CompletedTask;
        }

        private void Navigation_UpdatePositionEvent()
        {
            var latlng = new PointLatLng(_session.Client.CurrentLatitude, _session.Client.CurrentLongitude);
            _playerLocations.Add(latlng);

            SynchronizationContext.Post(o =>
            {
                _playerOverlay.Markers.Remove(_playerMarker);
                if (!_currentLatLng.IsEmpty)
                    _playerMarker = _currentLatLng.Lng < latlng.Lng
                        ? new GMapMarkerTrainer(latlng, ResourceHelper.GetImage("PlayerLocation2", null, null, 32, 32))
                        : new GMapMarkerTrainer(latlng, ResourceHelper.GetImage("PlayerLocation", null, null, 32, 32));
                _playerOverlay.Markers.Add(_playerMarker);
                if (followTrainerCheckBox.Checked)
                    GMapControl1.Position = latlng;
            }, null);
            _currentLatLng = latlng;
            UpdateMap();
        }

        private void UpdateMap()
        {
            SynchronizationContext.Post(o =>
            {
                _playerOverlay.Routes.Clear();
                var route = new GMapRoute(_playerLocations, "step")
                {
                    Stroke = new Pen(Color.FromArgb(0, 174, 0), 3) { DashStyle = DashStyle.Solid }
                };
                _playerOverlay.Routes.Add(route);
            }, null);
        }

        private void UpdateMap(FortData pokestop)
        {
            //TODO: Temporay load pokestops
            if (encounterPokemonsCount > 5 || encounterPokemonsCount == 0)
            {
                Task.Run(InitializePokestopsAndRoute).ConfigureAwait(false);
                encounterPokemonsCount = 0;
            }
            encounterPokemonsCount++;
            //

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
                               ResourceHelper.GetImage("Pokestop_looted", null, null, 32, 32));
                            //pokestopMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                            //pokestopMarker.ToolTip = new GMapBaloonToolTip(pokestopMarker);
                            _pokestopsOverlay.Markers.Add(pokestopMarker);
                        }
                    }
                }
                Navigation_UpdatePositionEvent();
            }, null);
        }

        private int encounterPokemonsCount;

        private void UpdateMap(List<GeoCoordinate> points)
        {
            SynchronizationContext.Post(o =>
            {
                _playerRouteOverlay.Routes.Clear();
                var routePointLatLngs = new List<PointLatLng>();
                foreach (var item in points)
                {
                    routePointLatLngs.Add(new PointLatLng(item.Latitude, item.Longitude));
                }
                var routes = new GMapRoute(routePointLatLngs, routePointLatLngs.ToString())
                {
                    Stroke = new Pen(Color.FromArgb(128, 0, 179, 253), 4) { DashStyle = DashStyle.Dash }
                };
                _playerRouteOverlay.Routes.Add(routes);
                /*/TODO: Deplaced temporary
                if (encounterPokemonsCount > 5 || encounterPokemonsCount == 0)
                {
                    Task.Run(InitializePokestopsAndRoute).ConfigureAwait(false);
                    encounterPokemonsCount = 0;
                }
                encounterPokemonsCount++;
                /*/
                Navigation_UpdatePositionEvent();
            }, null);
        }

        private void UpdateMap(List<MapPokemon> encounterPokemons)
        {
            SynchronizationContext.Post(o =>
            {
                foreach (var pokemon in encounterPokemons)
                {
                    var pkmImage = ResourceHelper.GetImage(null, null, pokemon, 25, 25);
                    var pointLatLng = new PointLatLng(pokemon.Latitude, pokemon.Longitude);
                    GMapMarker pkmMarker = new GMapMarkerTrainer(pointLatLng, pkmImage);
                    _pokemonsOverlay.Markers.Add(pkmMarker);
                }
                Navigation_UpdatePositionEvent();
            }, null);
        }

        private async void GMapControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!_botStarted) return;
            var pos = GMapControl1.FromLocalToLatLng(e.Location.X, e.Location.Y);
            await SetMoveToTargetTask.Execute(pos.Lat, pos.Lng);
        }

        private void TrackBar_Scroll(object sender, EventArgs e)
        {
            GMapControl1.Zoom = trackBar.Value;
        }

        #endregion

        #region EVENTS

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            TrayIcon.Visible = false;
            //TODO: Kills the application
            try
            {
                List<Control> listControls = new List<Control>();
                foreach (Control control in Instance.Controls)
                {
                    listControls.Add(control);
                }
                foreach (Control control in listControls)
                {
                    Instance.Controls.Remove(control);
                    control.Dispose();
                    GC.SuppressFinalize(control);
                }
                // kills
                Thread.CurrentThread.Abort(this);
            }
            catch
            {
                Thread.ResetAbort();
            }

            try
            {
                foreach (var process in Process.GetProcessesByName(Assembly.GetExecutingAssembly().GetName().Name))
                {
                    process.Kill();
                }
            }
            catch
            {
                //not implanted
            }
            //*/
        }

        private void PokeEaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://theunnamedorganisation.github.io/RocketBot/");
        }

        private void BtnPokeDex_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Form PokeDexForm = new PokeDexForm(_session);
            PokeDexForm.ShowDialog();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
             ReloadPokemonList().ConfigureAwait(false);
        }

        private void StartStopBotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_botStarted)
            {
                Environment.Exit(0);
                return;
            }
            startStopBotToolStripMenuItem.Text = @"■ Exit RocketBot2";
            _botStarted = true;
            btnPokeDex.Enabled = _botStarted;
            Task.Run(StartBot).ConfigureAwait(false);
        }

        private void TodoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Form settingsForm = new SettingsForm(ref _settings);
            settingsForm.ShowDialog();
            var newLocation = new PointLatLng(_settings.LocationConfig.DefaultLatitude, _settings.LocationConfig.DefaultLongitude);
            GMapControl1.Position = newLocation;
            _playerMarker.Position = newLocation;
            _playerLocations.Clear();
            _playerLocations.Add(newLocation);
            UpdateMap();
        }

        private void ShowConsoleToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread mThread = new Thread(delegate ()
            {
                var infoForm = new InfoForm();
                infoForm.ShowDialog();
            });
            mThread.SetApartmentState(ApartmentState.STA);
            mThread.Start();
        }

        private void AccountsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem en in accountsToolStripMenuItem.DropDownItems)
            {
                if (en.Text == _settings.Auth.CurrentAuthConfig.Username)
                    en.Enabled = false;
                else
                    en.Enabled = true;
            }
        }

        private void ShowMoreCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (showMoreCheckBox.Checked)
            {
                followTrainerCheckBox.Visible = true;
                togglePrecalRoute.Visible = true;
                GMAPSatellite.Visible = true;
            }
            else
            {
                followTrainerCheckBox.Visible = false;
                togglePrecalRoute.Visible = false;
                GMAPSatellite.Visible = false;
            }
        }

        private void FollowTrainerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (followTrainerCheckBox.Checked)
            {
                GMapControl1.CanDragMap = false;
                GMapControl1.Position = _currentLatLng;
            }
            else
            {
                GMapControl1.CanDragMap = true;
            }
        }

        private void TogglePrecalRoute_CheckedChanged(object sender, EventArgs e)
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

        private void CheckBoxAutoRefresh_CheckedChanged(object sender, EventArgs e)
        {
            if (Instance._botStarted)
            Instance.btnRefresh.Enabled = !Instance.checkBoxAutoRefresh.Checked;
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
                    olvPokemonList.SmallImageList.Images.Add(key, pokemon.Icon);
                }
                return key;
            };

            olvPokemonList.FormatRow += delegate (object sender, FormatRowEventArgs e)
            {
                var pok = e.Model as PokemonObject;
                e.Item.BackColor = pok.Favorited ? Color.LightYellow : e.Item.BackColor;
                if (olvPokemonList.Objects
                    .Cast<PokemonObject>()
                    .Select(i => i.PokemonId)
                    // ReSharper disable once PossibleNullReferenceException
                    .Count(p => p == pok.PokemonId) > 1)
                {
                    e.Item.BackColor = pok.Favorited ? Color.LightBlue : Color.LightGreen;
                }

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
                var pokemon = olvPokemonList.SelectedObjects.Cast<PokemonObject>().Select(o => o).First();
                var AllowEvolve = olvPokemonList.SelectedObjects.Cast<PokemonObject>().Select(o => o).All(o => o.AllowEvolve);
                var AllowTransfer = olvPokemonList.SelectedObjects.Cast<PokemonObject>().Select(o => o).All(o => o.AllowTransfer);
                var AllowPowerup = olvPokemonList.SelectedObjects.Cast<PokemonObject>().Select(o => o).All(o => o.AllowPowerup);
                var Favorited = olvPokemonList.SelectedObjects.Cast<PokemonObject>().Select(o => o).All(o => o.Favorited);
                var count = pokemons.Count();

                if (count < 1)
                {
                    e.Cancel = true;
                }

                ToolStripMenuItem item;

                if (AllowTransfer)
                {
                    item = new ToolStripMenuItem(Text = $"Transfer {count} Pokémons");
                    item.Click += delegate { TransferPokemon(pokemons); };
                    cmsPokemonList.Items.Add(item);
                }

                if (count != 1) return;

                if (AllowEvolve)
                {
                    item = new ToolStripMenuItem(Text = $"Evolve");
                    item.Click += delegate {
                        EvolvePokemon(pokemon.PokemonData);
                    };
                    cmsPokemonList.Items.Add(item);
                }

                if (AllowPowerup)
                {
                    item = new ToolStripMenuItem(Text = $"PowerUp");
                    item.Click += delegate { PowerUpPokemon(pokemons); };
                    cmsPokemonList.Items.Add(item);
                }

                item = new ToolStripMenuItem(Text = Favorited ? "Un-Favorite" : "Favorite");
                item.Click += delegate { FavoritedPokemon(pokemons, Favorited); };
                cmsPokemonList.Items.Add(item);

                item = new ToolStripMenuItem(Text = @"Rename");
                item.Click += delegate
                {
                    using (var form = count == 1 ? new NicknamePokemonForm(pokemon) : new NicknamePokemonForm())
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            NicknamePokemon(pokemons, form.txtNickname.Text);
                        }
                    }
                };
                cmsPokemonList.Items.Add(item);

                cmsPokemonList.Items.Add(new ToolStripSeparator());

                item = new ToolStripMenuItem(Text = @"Set Buddy");
                item.Click += delegate { SetBuddy_Click(pokemon); };
                cmsPokemonList.Items.Add(item);

                cmsPokemonList.Items.Add(new ToolStripSeparator());

                item = new ToolStripMenuItem(Text = @"Properties");
                item.Click += delegate
                {
                    PokemonProperties(pokemon);
                };
                cmsPokemonList.Items.Add(item);
            };
        }

        private async void SetBuddy_Click(PokemonObject pokemonObject)
        {
            await SelectBuddyPokemonTask.Execute(
                _session,
                _session.CancellationTokenSource.Token,
                pokemonObject.Id);
                await ReloadPokemonList().ConfigureAwait(false);
        }

        private void PokemonProperties(PokemonObject pokemonObject)
        {
            using (var form = new PokemonPropertiesForm(_session, pokemonObject))
            {
                form.ShowDialog();
            }
        }

        private void OlvPokemonList_ButtonClick(object sender, CellClickEventArgs e)
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

        private void OlvPokemonList_DoubleClick(object sender, EventArgs e)
        {
            var pokemonObject = olvPokemonList.SelectedObjects.Cast<PokemonObject>().Select(o => o).First();
            PokemonProperties(pokemonObject);
        }

        private async void FavoritedPokemon(IEnumerable<PokemonData> pokemons, bool fav)
        {
            foreach (var pokemon in pokemons)
            {
                await Task.Run(async () => { await FavoritePokemonTask.Execute(_session, pokemon.Id, !fav); });
            }
                await ReloadPokemonList().ConfigureAwait(false);
        }

        private async void TransferPokemon(IEnumerable<PokemonData> pokemons)
        {
            var _pokemons = new List<ulong>();
            string poketotransfer = null;
            foreach (var pokemon in pokemons)
            {
                _pokemons.Add(pokemon.Id);
                poketotransfer = $"{poketotransfer} [{_session.Translation.GetPokemonTranslation(pokemon.PokemonId)}]";
            }
            DialogResult result = MessageBox.Show($"Do you want to transfer {pokemons.Count()} Pokémon(s)?\n\r{poketotransfer}", $"Transfer {pokemons.Count()} Pokémon(s)", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                            await ReloadPokemonList().ConfigureAwait(false);
                        break;
                    case DialogResult.No:
                        await Task.Run(async () => { await UpgradeSinglePokemonTask.Execute(_session, pokemon.Id, false, 1 /* Only upgrade 1 time */); });
                            await ReloadPokemonList().ConfigureAwait(false);
                        break;
                }
            }
        }

        private void EvolvePokemon(PokemonData pokemon)
        {
            using (var form = new EvoleToPokemon())
            {
                PokemonEvoleTo pok = new PokemonEvoleTo(_session, pokemon);
                foreach (var to in pok.EvolutionBranchs)
                {
                    var item = new PictureBox();
                    item.Image = ResourceHelper.SetImageSize(ResourceHelper.GetPokemonImage((int)to.Pokemon, pokemon), item.Size.Height, item.Size.Width);
                    item.Click += async delegate
                    {
                        await Task.Run(async () => { await EvolveSpecificPokemonTask.Execute(_session, to.OriginPokemonId, to.Pokemon); });
                        await ReloadPokemonList().ConfigureAwait(false);
                        form.Close();
                    };
                    item.MouseLeave += delegate { item.BackColor = Color.Transparent; };
                    item.MouseEnter += delegate { item.BackColor = Color.LightGreen; };
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
                    Logger.Write($"\"{newName}\" is too long, please choose another name", LogLevel.Error);
                    if (pokemonDatas.Count() == 1)
                    {
                        SetState(true);
                        return;
                    }
                    continue;
                }
                await Task.Run(async () => { await RenameSinglePokemonTask.Execute(_session, pokemon.Id, nickname, _session.CancellationTokenSource.Token); });
                    await ReloadPokemonList().ConfigureAwait(false);
            }
        }

        private async Task ReloadPokemonList()
        {
            Instance.SetState(false);
            try
            {
                if (_session.Client.Download.ItemTemplates == null)
                    await _session.Client.Download.GetItemTemplates().ConfigureAwait(false);

                var templates = _session.Client.Download.ItemTemplates.Where(x => x.PokemonSettings != null)
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
                    var pokemonObject = new PokemonObject(_session, pokemon);
                    pokemonObjects.Add(pokemonObject);
                }

                var prevTopItem = Instance.olvPokemonList.TopItemIndex;
                Instance.olvPokemonList.SetObjects(pokemonObjects);
                Instance.olvPokemonList.TopItemIndex = prevTopItem;

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

                Instance.lblPokemonList.Text = _session.Translation.GetTranslation(TranslationString.AmountPkmSeenCaught, _totalData, _totalCaptures) +
                    $" | Storage: {_session.Client.Player.PlayerData.MaxPokemonStorage} ({pokemons.Count()} Pokémons, {_session.Inventory.GetEggs().Result.Count()} Eggs)";

                var items =
                    _session.Inventory.GetItems().Result
                    .Where(i => i != null)
                    .OrderBy(i => i.ItemId);

                var appliedItems =
                    _session.Inventory.GetAppliedItems().Result
                    .Where(aItems => aItems?.Item != null)
                    .SelectMany(aItems => aItems.Item)
                    .ToDictionary(item => item.ItemId, item => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(item.ExpireMs));

                await FlpItemsClean(items, appliedItems).ConfigureAwait(false);

                Instance.lblInventory.Text =
                        $"Types: {items.Count()} | Total: {_session.Inventory.GetTotalItemCount().Result} | Storage: {_session.Client.Player.PlayerData.MaxItemStorage}";
            }
            catch (ArgumentNullException)
            {
                Logger.Write("Please start the bot or wait until login is finished before loading Pokemon List",
                    LogLevel.Warning);
            }
            catch (Exception ex)
            {
                Logger.Write(ex.ToString(), LogLevel.Error);
            }
            Instance.SetState(true);
        }

        private static Task FlpItemsClean(IOrderedEnumerable<ItemData> items, Dictionary<ItemId, DateTime> appliedItems)
        {
            List<Control> listControls = new List<Control>();

            foreach (Control control in Instance.flpItems.Controls)
            {
                listControls.Add(control);
            }

            foreach (Control control in listControls)
            {
                Instance.flpItems.Controls.Remove(control);
                control.Dispose();
            }

            foreach (var item in items)
            {
                if (item.ItemId == ItemId.ItemIncubatorBasicUnlimited)
                {
                    ItemData extra = new ItemData()
                    {
                        ItemId = ItemId.ItemSpecialCamera
                    };
                    var extra_box = new ItemBox(extra);
                    extra_box.ItemClick += Instance.ItemBox_ItemClick;
                    Instance.flpItems.Controls.Add(extra_box);
                }

                var box = new ItemBox(item);
                if (appliedItems.ContainsKey(item.ItemId))
                {
                    box.expires = appliedItems[item.ItemId];
                }
                box.ItemClick += Instance.ItemBox_ItemClick;
                Instance.flpItems.Controls.Add(box);
            }
            return Task.CompletedTask;
        }

        private async void ItemBox_ItemClick(object sender, EventArgs e)
        {
            var item = (ItemData)sender;

            if (item.ItemId == ItemId.ItemIncubatorBasic
               || item.ItemId == ItemId.ItemIncubatorBasicUnlimited)
            {
                System.Windows.Forms.Form form = new EggsForm(_session);
                form.ShowDialog();
                return;
            }

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
                    await ReloadPokemonList().ConfigureAwait(false);
            }
        }

        private void SetState(bool state)
        {
            if (Instance.checkBoxAutoRefresh.Checked) state = false;
            Instance.btnRefresh.Enabled = state;
        }

        #endregion POKEMON LIST

        #region ROCKETBOT INIT -> START

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

            Console.Title = @"RocketBot2 Loading";
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
                    _botStarted = CheckMKillSwitch();
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
                            "You have selected PogoDev API but you have not provided an API Key, please press any key to exit and correct you auth.json, \r\n The Pogodev API key can be purchased at - https://talk.pogodev.org/d/51-api-hashing-service-by-pokefarmer",
                            LogLevel.Error
                        );
                        _botStarted = true;
                    }
                    try
                    {
                        HttpClient client = new HttpClient();
                        client.DefaultRequestHeaders.Add("X-AuthToken", apiCfg.AuthAPIKey);
                        var maskedKey = apiCfg.AuthAPIKey.Substring(0, 4) + "".PadLeft(apiCfg.AuthAPIKey.Length - 8, 'X') + apiCfg.AuthAPIKey.Substring(apiCfg.AuthAPIKey.Length - 4, 4);
                        HttpResponseMessage response = client.PostAsync("https://pokehash.buddyauth.com/api/v133_1/hash", null).Result;
                        string AuthKey = response.Headers.GetValues("X-AuthToken").FirstOrDefault();
                        string MaxRequestCount = response.Headers.GetValues("X-MaxRequestCount").FirstOrDefault();
                        DateTime AuthTokenExpiration = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local).AddSeconds(Convert.ToDouble(response.Headers.GetValues("X-AuthTokenExpiration").FirstOrDefault())).ToLocalTime();
                        TimeSpan Expiration = AuthTokenExpiration - DateTime.Now;
                        string Result = string.Format("Key: {0} RPM: {1} Expiration Date: {2}/{3}/{4}", maskedKey, MaxRequestCount, AuthTokenExpiration.Day, AuthTokenExpiration.Month, AuthTokenExpiration.Year);
                        Logger.Write(Result, LogLevel.Info, ConsoleColor.Green);
                    }
                    catch
                    {
                        Logger.Write("The HashKey is invalid or has expired, please press any key to exit and correct you auth.json, \r\nThe Pogodev API key can be purchased at - https://talk.pogodev.org/d/51-api-hashing-service-by-pokefarmer", LogLevel.Error);
                        _botStarted = true;
                    }
                }
                else if (apiCfg.UseLegacyAPI)
                {
                    Logger.Write(
                   "You bot will start after 15 seconds, You are running bot with Legacy API (0.45), but it will increase your risk of being banned and triggering captchas. Config Captchas in config.json to auto-resolve them",
                   LogLevel.Warning
               );

#if RELEASE
                    Thread.Sleep(15000);
#endif
                }
                else
                {
                    Logger.Write(
                         "At least 1 authentication method must be selected, please correct your auth.json.",
                         LogLevel.Error
                     );
                    _botStarted = true;
                }
            }

            _session = new Session(settings,
                new ClientSettings(settings, elevationService), logicSettings, elevationService, translation);

            ioc.Register<ISession>(_session);

            Logger.SetLoggerContext(_session);

            MultiAccountManager accountManager = new MultiAccountManager(settings, logicSettings.Bots);
            ioc.Register(accountManager);

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

                    //Logger.Write("Press a Key to continue...",
                    //    LogLevel.Warning);
                    //Console.ReadKey();
                    //return;
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
            onBotStarted?.Invoke(_session, aggregator);

            Resources.ProgressBar.Fill(50);
            var listener = new ConsoleEventListener();
            ConsoleEventListener.HumanWalkEvent += (humanWalkingEvent) =>
            {
                var speed = Math.Round(humanWalkingEvent.CurrentWalkingSpeed, 2);
                MainForm.SetSpeedLable("Current Speed: " + speed + " km/h");
            };
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

            _session.Navigation.WalkStrategy.GetRouteEvent +=
                (points) => _session.EventDispatcher.Send(new GetRouteEvent { Points = points });
            _session.Navigation.WalkStrategy.GetRouteEvent += UpdateMap;

            UseNearbyPokestopsTask.LootPokestopEvent +=
                pokestop => _session.EventDispatcher.Send(new LootPokestopEvent { Pokestop = pokestop });
            UseNearbyPokestopsTask.LootPokestopEvent += UpdateMap;

            CatchNearbyPokemonsTask.PokemonEncounterEvent +=
                mappokemons => _session.EventDispatcher.Send(new PokemonsEncounterEvent { EncounterPokemons = mappokemons });
            CatchNearbyPokemonsTask.PokemonEncounterEvent += UpdateMap;

            CatchIncensePokemonsTask.PokemonEncounterEvent +=
                mappokemons => _session.EventDispatcher.Send(new PokemonsEncounterEvent { EncounterPokemons = mappokemons });
            CatchIncensePokemonsTask.PokemonEncounterEvent += UpdateMap;

            CatchLurePokemonsTask.PokemonEncounterEvent +=
                mappokemons => _session.EventDispatcher.Send(new PokemonsEncounterEvent { EncounterPokemons = mappokemons });
            CatchLurePokemonsTask.PokemonEncounterEvent += UpdateMap;

            Resources.ProgressBar.Fill(100);

            if (settings.WebsocketsConfig.UseWebsocket)
            {
                var websocket = new WebSocketInterface(settings.WebsocketsConfig.WebSocketPort, _session);
                _session.EventDispatcher.EventReceived += evt => websocket.Listen(evt, _session);
            }

            ioc.Register<MultiAccountManager>(accountManager);

            if (accountManager.AccountsReadOnly.Count > 1)
            {
                foreach (var _bot in accountManager.AccountsReadOnly)
                {
                    var _item = new ToolStripMenuItem()
                    {
                        Text = _bot.Username
                    };
                    _item.Click += delegate
                    {
                        if (!Instance._botStarted)
                            _session.ReInitSessionWithNextBot(_bot);
                        accountManager.SwitchAccountTo(_bot);
                    };
                    accountsToolStripMenuItem.DropDownItems.Add(_item);
                }
            }
            else
            {
                menuStrip1.Items.Remove(accountsToolStripMenuItem);
            }

            var bot = accountManager.GetStartUpAccount();

            _session.ReInitSessionWithNextBot(bot);
            _machine = machine;
            _settings = settings;
            _excelConfigAllow = excelConfigAllow;

            if (_botStarted) startStopBotToolStripMenuItem.Text = @"■ Exit RocketBot2";
        }

        private Task StartBot()
        {
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
                HumanWalkSnipeTask.StartFastPokemapAsync(_session,
                    _session.CancellationTokenSource.Token).ConfigureAwait(false); // that need to keep data live
            }

            if (_session.LogicSettings.UseSnipeLocationServer ||
              _session.LogicSettings.HumanWalkingSnipeUsePogoLocationFeeder)
                SnipePokemonTask.AsyncStart(_session);


            if (_session.LogicSettings.DataSharingConfig.EnableSyncData)
            {
                BotDataSocketClient.StartAsync(_session, Properties.Resources.EncryptKey);
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

            _session.AnalyticsService.StartAsync(_session, _session.CancellationTokenSource.Token).ConfigureAwait(false);

            _session.EventDispatcher.EventReceived += evt => AnalyticsService.Listen(evt, _session);

            /*var trackFile = Path.GetTempPath() + "\\rocketbot2.io";

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
            }*/

            QuitEvent.WaitOne();
            return Task.CompletedTask;
            //return new Task(() => { });
        }

        #endregion

        #region PROGRAM CLIENT FUNCTIONS

        private static void EventDispatcher_EventReceived(IEvent evt)
        {
            throw new NotImplementedException();
        }

        private void ErrorHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Debug.WriteLine(e.ExceptionObject.ToString());
            ConsoleHelper.ShowConsoleWindow();
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

                            /*Logger.Write(strExitMsg + $"\n" + "Please press enter to continue", LogLevel.Error);
                            Console.ReadLine();*/
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
                                Logger.Write("Overriding Killswitch... you have been warned!", LogLevel.Warning);
                                return false;
                            }

                            Logger.Write("The bot will now close, please press enter to continue", LogLevel.Error);
                            //Console.ReadLine();
                            //Environment.Exit(0);
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

        #endregion
    }
}
