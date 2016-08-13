using BrightIdeasSoftware;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using POGOProtos.Data;
using POGOProtos.Data.Player;
using POGOProtos.Enums;
using POGOProtos.Inventory;
using POGOProtos.Inventory.Item;
using POGOProtos.Map.Fort;
using POGOProtos.Map.Pokemon;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using static System.Reflection.Assembly;

namespace PokemonGo.RocketAPI.Window
{
    public partial class MainForm : Form
    {
        public static MainForm Instance;
        public static SynchronizationContext SynchronizationContext;

        //delay between actions, to similate human operation
        private const int ACTIONDELAY = 1500;

        public static Settings ClientSettings;
        private static int _currentlevel = -1;
        private static int _totalExperience;
        private static int _totalPokemon;
        private static bool _stopping;
        private static bool _forceUnbanning;
        private static bool _farmingStops;
        private static bool _farmingPokemons;
        private static readonly DateTime TimeStarted = DateTime.Now;
        public static DateTime InitSessionDateTime = DateTime.Now;

        private static bool _botStarted;
        private readonly GMapOverlay _playerOverlay = new GMapOverlay("players");
        private readonly GMapOverlay _pokemonsOverlay = new GMapOverlay("pokemons");
        private readonly GMapOverlay _pokestopsOverlay = new GMapOverlay("pokestops");

        private readonly GMapOverlay _searchAreaOverlay = new GMapOverlay("areas");

        private Client _client;
        private Client _client2;
        private bool _initialized;
        private LocationManager _locationManager;

        private GMarkerGoogle _playerMarker;

        private IEnumerable<FortData> _pokeStops;
        private IEnumerable<WildPokemon> _wildPokemons;

        public MainForm()
        {
            InitializeComponent();
            SynchronizationContext = SynchronizationContext.Current;
            ClientSettings = Settings.Instance;
            //Client.OnConsoleWrite += Client_OnConsoleWrite;
            Instance = this;

            Text = @"RocketBot v" + GetExecutingAssembly().GetName().Version;
            CenterToScreen();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            gMapControl1.MapProvider = GoogleMapProvider.Instance;
            gMapControl1.Manager.Mode = AccessMode.ServerOnly;
            GMapProvider.WebProxy = null;
            gMapControl1.Position = new PointLatLng(ClientSettings.DefaultLatitude, ClientSettings.DefaultLongitude);
            gMapControl1.DragButton = MouseButtons.Left;

            gMapControl1.MinZoom = 1;
            gMapControl1.MaxZoom = 20;
            gMapControl1.Zoom = 15;

            gMapControl1.Overlays.Add(_searchAreaOverlay);
            gMapControl1.Overlays.Add(_pokestopsOverlay);
            gMapControl1.Overlays.Add(_pokemonsOverlay);
            gMapControl1.Overlays.Add(_playerOverlay);

            _playerMarker =
                new GMarkerGoogle(new PointLatLng(ClientSettings.DefaultLatitude, ClientSettings.DefaultLongitude),
                    GMarkerGoogleType.orange_small);
            _playerOverlay.Markers.Add(_playerMarker);

            InitializeMap();
            InitializePokemonForm();
            CheckVersion();
        }

        public void Restart()
        {
            InitializeMap();
            InitializePokemonForm();
        }

        private void InitializeMap()
        {
            _playerMarker.Position = new PointLatLng(ClientSettings.DefaultLatitude, ClientSettings.DefaultLongitude);
            _searchAreaOverlay.Polygons.Clear();
            S2GMapDrawer.DrawS2Cells(
                S2Helper.GetNearbyCellIds(ClientSettings.DefaultLongitude, ClientSettings.DefaultLatitude),
                _searchAreaOverlay);
        }

        public static void ResetMap()
        {
            Instance.gMapControl1.Position = new PointLatLng(ClientSettings.DefaultLatitude,
                ClientSettings.DefaultLongitude);
            Instance._playerMarker.Position = new PointLatLng(ClientSettings.DefaultLatitude,
                ClientSettings.DefaultLongitude);
            Instance._searchAreaOverlay.Polygons.Clear();
            S2GMapDrawer.DrawS2Cells(
                S2Helper.GetNearbyCellIds(ClientSettings.DefaultLongitude, ClientSettings.DefaultLatitude),
                Instance._searchAreaOverlay);
        }

        public static double GetRuntime()
        {
            return (DateTime.Now - TimeStarted).TotalSeconds / 3600;
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
                ColoredConsoleWrite(Color.Green, "Your version is " + GetExecutingAssembly().GetName().Version);
                ColoredConsoleWrite(Color.Green, "Github version is " + gitVersion);
                ColoredConsoleWrite(Color.Green,
                    "You can find it at www.GitHub.com/TheUnnameOrganization/RocketBot/releases");
            }
            catch (Exception)
            {
                ColoredConsoleWrite(Color.Red, "Unable to check for updates now...");
            }
        }

        private static string DownloadServerVersion()
        {
            using (var wC = new WebClient())
                return
                    wC.DownloadString(
                        "https://raw.githubusercontent.com/TheUnnameOrganization/RocketBot/Beta-Build/src/RocketBotGUI/Properties/AssemblyInfo.cs");
        }

        public static void ColoredConsoleWrite(Color color, string text)
        {
            if (Instance.InvokeRequired)
            {
                Instance.Invoke(new Action<Color, string>(ColoredConsoleWrite), color, text);
                return;
            }

            Instance.logTextBox.Select(Instance.logTextBox.Text.Length, 1); // Reset cursor to last

            var textToAppend = "[" + DateTime.Now.ToString("HH:mm:ss tt") + "] " + text + "\r\n";
            Instance.logTextBox.SelectionColor = color;
            Instance.logTextBox.AppendText(textToAppend);

            var syncRoot = new object();
            lock (syncRoot) // Added locking to prevent text file trying to be accessed by two things at the same time
            {
                var dir = AppDomain.CurrentDomain.BaseDirectory + @"\Logs";
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.AppendAllText(dir + @"\" + DateTime.Today.ToString("yyyyMMdd") + ".txt",
                    "[" + DateTime.Now.ToString("HH:mm:ss tt") + "] " + text + "\r\n");
            }
        }

        public void ConsoleClear()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ConsoleClear));
                return;
            }

            logTextBox.Clear();
        }

        public void SetStatusText(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(SetStatusText), text);
                return;
            }

            statusLabel.Text = text;
        }

        private async Task EvolvePokemons(Client client)
        {
            var inventory = await client.Inventory.GetInventory();
            var pokemons =
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonData)
                    .Where(p => p != null && p?.PokemonId > 0);

            await EvolveAllGivenPokemons(client, pokemons);
        }

        private async Task EvolveAllGivenPokemons(Client client, IEnumerable<PokemonData> pokemonToEvolve)
        {
            var excludedPokemon = Settings.Instance.ExcludedPokemonEvolve;
            foreach (var pokemon in pokemonToEvolve)
            {
                if (excludedPokemon.Contains(pokemon.PokemonId))
                {
                    ColoredConsoleWrite(Color.Orange,
                        $"{pokemon.PokemonId} excluded for evolving");
                    continue;
                }

                var countOfEvolvedUnits = 0;
                var xpCount = 0;

                EvolvePokemonResponse evolvePokemonOutProto;
                do
                {
                    evolvePokemonOutProto = await client.Inventory.EvolvePokemon(pokemon.Id);
                    //todo: someone check whether this still works

                    if (evolvePokemonOutProto.Result == EvolvePokemonResponse.Types.Result.Success)
                    {
                        countOfEvolvedUnits++;
                        xpCount += evolvePokemonOutProto.ExperienceAwarded;
                        _totalExperience += evolvePokemonOutProto.ExperienceAwarded;
                    }
                } while (evolvePokemonOutProto.Result == EvolvePokemonResponse.Types.Result.Success);

                if (countOfEvolvedUnits > 0)
                    ColoredConsoleWrite(Color.Cyan,
                        $"Evolved {countOfEvolvedUnits} pieces of {pokemon.PokemonId} for {xpCount}xp");

                await Task.Delay(3000);
            }
        }

        private async Task Execute()
        {
            _client = new Client(ClientSettings, new ApiFailureStrategy());
            _locationManager = new LocationManager(_client, ClientSettings.TravelSpeed);
            try
            {
                switch (ClientSettings.AuthType)
                {
                    case AuthType.Ptc:
                        ColoredConsoleWrite(Color.Green, "Login Type: Pokemon Trainers Club");
                        break;

                    case AuthType.Google:
                        ColoredConsoleWrite(Color.Green, "Login Type: Google");
                        break;
                }

                await _client.Login.DoLogin();
                var profile = await _client.Player.GetPlayer();
                //var settings = await _client.Download.GetSettings();
                //var mapObjects = await _client.Map.GetMapObjects();
                var inventory = await _client.Inventory.GetInventory();
                var pokemons =
                    inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonData)
                        .Where(p => p != null && p?.PokemonId > 0);

                if (!_initialized)
                {
                    new Thread(async () =>
                    {
                        while (true)
                        {
                            if (_botStarted)
                            {
                                var stats =
                                    inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PlayerStats)
                                        .Where(i => i != null)
                                        .ToArray();
                                short hoursLeft = 0;
                                short minutesLeft = 0;
                                var secondsLeft = 0;
                                double xpSec = 0;
                                var v = stats.First();
                                if (v != null)
                                {
                                    var XpDiff = GetXpDiff(_client, v.Level);
                                    //Calculating the exp needed to level up
                                    float expNextLvl = v.NextLevelXp - v.Experience;
                                    //Calculating the exp made per second
                                    xpSec = Math.Round(_totalExperience / GetRuntime()) / 60 / 60;
                                    //Calculating the seconds left to level up
                                    if (xpSec != 0)
                                        secondsLeft = Convert.ToInt32(expNextLvl / xpSec);
                                    //formatting data to make an output like DateFormat
                                    while (secondsLeft > 60)
                                    {
                                        secondsLeft -= 60;
                                        if (minutesLeft < 60)
                                        {
                                            minutesLeft++;
                                        }
                                        else
                                        {
                                            minutesLeft = 0;
                                            hoursLeft++;
                                        }
                                    }

                                    SetStatusText(
                                        string.Format(
                                            profile.PlayerData.Username +
                                            " | Level: {0:0} - ({2:0} / {3:0}) | Runtime {1} | Stardust: {4:0}", v.Level,
                                            _getSessionRuntimeInTimeFormat(), v.Experience - v.PrevLevelXp - XpDiff,
                                            v.NextLevelXp - v.PrevLevelXp - XpDiff,
                                            profile.PlayerData.Currencies.ToArray()[1].Amount) +
                                        " | XP/Hour: " + Math.Round(_totalExperience / GetRuntime()) + " | Pokemon/Hour: " +
                                        Math.Round(_totalPokemon / GetRuntime()) + " | NextLevel in: " + hoursLeft + ":" +
                                        minutesLeft +
                                        ":" + secondsLeft);
                                }
                            }
                            await Task.Delay(1000);
                        }
                    }).Start();
                    _initialized = true;
                }

                // Write the players ingame details
                ColoredConsoleWrite(Color.Yellow, "----------------------------");
                /*// dont actually want to display info but keeping here incase people want to \O_O/
                 * if (ClientSettings.AuthType == AuthType.Ptc)
                {
                    ColoredConsoleWrite(Color.Cyan, "Account: " + ClientSettings.PtcUsername);
                    ColoredConsoleWrite(Color.Cyan, "Password: " + ClientSettings.PtcPassword + "\n");
                }
                else
                {
                    ColoredConsoleWrite(Color.Cyan, "Email: " + ClientSettings.GoogleUsername);
                    ColoredConsoleWrite(Color.Cyan, "Password: " + ClientSettings.GooglePassword + "\n");
                }*/
                var lat2 = Convert.ToString(ClientSettings.DefaultLatitude);
                var longit2 = Convert.ToString(ClientSettings.DefaultLongitude);
                ColoredConsoleWrite(Color.DarkGray, "Name: " + profile.PlayerData.Username);
                ColoredConsoleWrite(Color.DarkGray, "Team: " + profile.PlayerData.Team);
                if (profile.PlayerData.Currencies.ToArray()[0].Amount > 0)
                    // If player has any pokecoins it will show how many they have.
                    ColoredConsoleWrite(Color.DarkGray,
                        "Pokecoins: " + profile.PlayerData.Currencies.ToArray()[0].Amount);
                ColoredConsoleWrite(Color.DarkGray,
                    "Stardust: " + profile.PlayerData.Currencies.ToArray()[1].Amount + "\n");
                ColoredConsoleWrite(Color.DarkGray, "Latitude: " + ClientSettings.DefaultLatitude);
                ColoredConsoleWrite(Color.DarkGray, "Longitude: " + ClientSettings.DefaultLongitude);
                try
                {
                    ColoredConsoleWrite(Color.DarkGray,
                        "Country: " + CallAPI("country", lat2.Replace(',', '.'), longit2.Replace(',', '.')));
                    ColoredConsoleWrite(Color.DarkGray,
                        "Area: " + CallAPI("place", lat2.Replace(',', '.'), longit2.Replace(',', '.')));
                }
                catch (Exception)
                {
                    ColoredConsoleWrite(Color.DarkGray, "Unable to get Country/Place");
                }

                ColoredConsoleWrite(Color.Yellow, "----------------------------");

                // I believe a switch is more efficient and easier to read.
                switch (ClientSettings.TransferType)
                {
                    case "Leave Strongest":
                        await TransferAllButStrongestUnwantedPokemon(_client);
                        break;

                    case "All":
                        await TransferAllGivenPokemons(_client, pokemons, ClientSettings.TransferIvThreshold);
                        break;

                    case "Duplicate":
                        await TransferDuplicatePokemon(_client);
                        break;

                    case "IV Duplicate":
                        await TransferDuplicateIVPokemon(_client);
                        break;

                    case "CP/IV Duplicate":
                        await TransferDuplicateCPIVPokemon(_client);
                        break;

                    case "CP":
                        await TransferAllWeakPokemon(_client, ClientSettings.TransferCpThreshold);
                        break;

                    case "IV":
                        await TransferAllGivenPokemons(_client, pokemons, ClientSettings.TransferIvThreshold);
                        break;

                    default:
                        ColoredConsoleWrite(Color.DarkGray, "Transfering pokemon disabled");
                        break;
                }

                if (ClientSettings.EvolveAllGivenPokemons)
                    await EvolveAllGivenPokemons(_client, pokemons);
                if (ClientSettings.Recycler)
                    await RecycleItems(_client);
                //client.RecycleItems(client);
                //await Task.Delay(5000);

                String incubatorMode = ClientSettings.UseIncubatorsMode.ToLower();
                switch (incubatorMode)
                {
                    case "only unlimited":
                        await UseIncubators(_client, incubatorMode);
                        break;
                    case "all incubators":
                        await UseIncubators(_client, incubatorMode);
                        break;
                    default:
                        ColoredConsoleWrite(Color.DarkGray, "Using incubators disabled");
                        break;
                }

                await PrintLevel(_client);

                await ExecuteFarmingPokestopsAndPokemons(_client);

                while (_forceUnbanning)
                    await Task.Delay(25);

                // await ForceUnban(client);
                //if (!_stopping)
                //{
                //    ColoredConsoleWrite(Color.Red, $"No nearby useful locations found. Please wait 5 seconds.");
                //    await Task.Delay(5000);
                //    Execute();
                //}
                //else
                //{
                //    ConsoleClear();
                //    _pokestopsOverlay.Routes.Clear();
                //    _pokestopsOverlay.Markers.Clear();
                //    ColoredConsoleWrite(Color.Red, $"Bot successfully stopped.");
                //    startStopBotToolStripMenuItem.Text = "Start";
                //    _stopping = false;
                //    _botStarted = false;
                //    _pokeStops = null;
                //}
                if (_stopping)
                {
                    ConsoleClear();
                    _pokestopsOverlay.Routes.Clear();
                    _pokestopsOverlay.Markers.Clear();
                    ColoredConsoleWrite(Color.Red, $"Bot successfully stopped.");
                    startStopBotToolStripMenuItem.Text = "Start";
                    _stopping = false;
                    _botStarted = false;
                    _pokeStops = null;
                }
            }
            catch (TaskCanceledException)
            {
                ColoredConsoleWrite(Color.Red, "Task Canceled Exception - Restarting");
                //if (!_stopping) Execute();
            }
            catch (UriFormatException)
            {
                ColoredConsoleWrite(Color.Red, "System URI Format Exception - Restarting");
                //if (!_stopping) Execute();
            }
            catch (ArgumentOutOfRangeException)
            {
                ColoredConsoleWrite(Color.Red, "ArgumentOutOfRangeException - Restarting");
                //if (!_stopping) Execute();
            }
            catch (ArgumentNullException)
            {
                ColoredConsoleWrite(Color.Red, "Argument Null Refference - Restarting");
                //if (!_stopping) Execute();
            }
            catch (NullReferenceException)
            {
                ColoredConsoleWrite(Color.Red, "Null Refference - Restarting");
                //if (!_stopping) Execute();
            }
            catch (AccessTokenExpiredException)
            {
                ColoredConsoleWrite(Color.Red, "Access Token Expired - Restarting");
                //if (!_stopping) Execute();
            }
            catch (GoogleException)
            {
                ColoredConsoleWrite(Color.Red, "Please check your google login information again");
            }
            catch (LoginFailedException)
            {
                ColoredConsoleWrite(Color.Red, "Login failed, please check your ptc login information again");
            }
            catch (InvalidResponseException)
            {
                ColoredConsoleWrite(Color.Red, "Invalid response - Restarting");
                //if (!_stopping) Execute();
            }
            catch (Exception ex)
            {
                ColoredConsoleWrite(Color.Red, ex.ToString());
                //if (!_stopping) Execute();
            }
        }

        private static string CallAPI(string elem, string lat, string lon)
        {
            using (
                var reader =
                    XmlReader.Create(@"http://api.geonames.org/findNearby?lat=" + lat + "&lng=" + lon +
                                     "&username=pokemongobot"))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (elem)
                        {
                            case "country":
                                if (reader.Name == "countryName")
                                {
                                    return reader.ReadString();
                                }
                                break;

                            case "place":
                                if (reader.Name == "name")
                                {
                                    return reader.ReadString();
                                }
                                break;

                            default:
                                return "N/A";
                                break;
                        }
                    }
                }
            }
            return "Error";
        }

        private async Task ExecuteCatchAllNearbyPokemons(Client client)
        {
            var mapObjects = await client.Map.GetMapObjects();

            var pokemons = mapObjects.Item1.MapCells.SelectMany(i => i.CatchablePokemons);
            var inventory2 = await client.Inventory.GetInventory();
            var pokemons2 = inventory2.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.PokemonData)
                .Where(p => p != null && p?.PokemonId > 0)
                .ToArray();
            var excludedPokemon = Settings.Instance.ExcludedPokemonCatch;

            foreach (var pokemon in pokemons)
            {
                if (_forceUnbanning || _stopping)
                    break;

                if (excludedPokemon.Contains(pokemon.PokemonId))
                {
                    ColoredConsoleWrite(Color.Orange,
                        $"Encountered {pokemon.PokemonId} but is excluded for catching.");
                    continue;
                }

                _farmingPokemons = true;

                await _locationManager.Update(pokemon.Latitude, pokemon.Longitude);

                string pokemonName;
                if (ClientSettings.Language == "german")
                {
                    var name_english = Convert.ToString(pokemon.PokemonId);
                    var request =
                        (HttpWebRequest)
                            WebRequest.Create("http://boosting-service.de/pokemon/index.php?pokeName=" + name_english);
                    var response = (HttpWebResponse)request.GetResponse();
                    pokemonName = new StreamReader(response.GetResponseStream()).ReadToEnd();
                }
                else
                    pokemonName = Convert.ToString(pokemon.PokemonId);

                await client.Player.UpdatePlayerLocation(pokemon.Latitude, pokemon.Longitude);
                UpdatePlayerLocation(pokemon.Latitude, pokemon.Longitude);
                UpdateMap();
                var encounterPokemonResponse =
                    await client.Encounter.EncounterPokemon(pokemon.EncounterId, pokemon.SpawnPointId);

                if (encounterPokemonResponse.Status == EncounterResponse.Types.Status.PokemonInventoryFull)
                {
                    ColoredConsoleWrite(Color.Orange,
                        $"Unable to catch pokemon, inventory is full!");
                    _farmingPokemons = false;
                    break;
                }

                var pokemonCp = encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp;
                var pokemonIv = Math.Round(Perfect(encounterPokemonResponse?.WildPokemon?.PokemonData));
                CatchPokemonResponse caughtPokemonResponse;
                ColoredConsoleWrite(Color.Green, $"Encounter a {pokemonName} with {pokemonCp} CP and {pokemonIv}% IV");
                do
                {
                    if (ClientSettings.RazzBerryMode.ToLower().Equals("cp"))
                        if (pokemonCp > ClientSettings.RazzBerrySetting)
                            await UseRazzBerry(client, pokemon.EncounterId, pokemon.SpawnPointId);
                    if (ClientSettings.RazzBerryMode.ToLower().Equals("probability"))
                        if (encounterPokemonResponse.CaptureProbability.CaptureProbability_.First() <
                            ClientSettings.RazzBerrySetting)
                            await UseRazzBerry(client, pokemon.EncounterId, pokemon.SpawnPointId);
                    caughtPokemonResponse =
                        await
                            CatchPokemon(pokemon.EncounterId, pokemon.SpawnPointId, pokemon.Latitude, pokemon.Longitude,
                                ItemId.ItemPokeBall, pokemonCp);
                    ; //note: reverted from settings because this should not be part of settings but part of logic
                } while (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed ||
                         caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchEscape);

                if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchSuccess)
                {
                    var c = Color.LimeGreen;
                    if (pokemonIv >= 80)
                    {
                        c = Color.Yellow;
                    }
                    ColoredConsoleWrite(c, $"We caught a {pokemonName} with {pokemonCp} CP and {pokemonIv}% IV");
                    //foreach (int xp in caughtPokemonResponse.CaptureAward.Xp)
                    //    TotalExperience += xp;
                    _totalExperience += caughtPokemonResponse.CaptureAward.Xp.Sum();
                    _totalPokemon += 1;
                }
                else
                    ColoredConsoleWrite(Color.Red, $"{pokemonName} with {pokemonCp} CP and {pokemonIv}% IV got away..");

                // I believe a switch is more efficient and easier to read.
                switch (ClientSettings.TransferType)
                {
                    case "Leave Strongest":
                        await TransferAllButStrongestUnwantedPokemon(client);
                        break;

                    case "All":
                        await TransferAllGivenPokemons(client, pokemons2, ClientSettings.TransferIvThreshold);
                        break;

                    case "Duplicate":
                        await TransferDuplicatePokemon(client);
                        break;

                    case "IV Duplicate":
                        await TransferDuplicateIVPokemon(client);
                        break;

                    case "CP/IV Duplicate":
                        await TransferDuplicateCPIVPokemon(client);
                        break;

                    case "CP":
                        await TransferAllWeakPokemon(client, ClientSettings.TransferCpThreshold);
                        break;

                    case "IV":
                        await TransferAllGivenPokemons(client, pokemons2, ClientSettings.TransferIvThreshold);
                        break;

                    default:
                        ColoredConsoleWrite(Color.DarkGray, "Transfering pokemon disabled");
                        break;
                }

                _farmingPokemons = false;
                //await Task.Delay(3000);
                await Task.Delay(500);
            }
            pokemons = null;
        }

        private void UpdatePlayerLocation(double latitude, double longitude)
        {
            SynchronizationContext.Post(o =>
            {
                _playerMarker.Position = (PointLatLng)o;

                _searchAreaOverlay.Polygons.Clear();
            }, new PointLatLng(latitude, longitude));

            ColoredConsoleWrite(Color.Gray, $"Moving player location to Lat: {latitude}, Lng: {longitude}");
        }

        private void UpdateMap()
        {
            SynchronizationContext.Post(o =>
            {
                _pokestopsOverlay.Markers.Clear();
                var routePoint = new List<PointLatLng>();
                foreach (var pokeStop in _pokeStops)
                {
                    var type = GMarkerGoogleType.blue_small;
                    if (pokeStop.CooldownCompleteTimestampMs > DateTime.UtcNow.ToUnixTime())
                    {
                        type = GMarkerGoogleType.gray_small;
                    }
                    var pokeStopLoc = new PointLatLng(pokeStop.Latitude, pokeStop.Longitude);
                    var pokestopMarker = new GMarkerGoogle(pokeStopLoc, type);
                    //pokestopMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                    //pokestopMarker.ToolTip = new GMapBaloonToolTip(pokestopMarker);
                    _pokestopsOverlay.Markers.Add(pokestopMarker);

                    routePoint.Add(pokeStopLoc);
                }
                _pokestopsOverlay.Routes.Clear();
                _pokestopsOverlay.Routes.Add(new GMapRoute(routePoint, "Walking Path"));

                _pokemonsOverlay.Markers.Clear();
                if (_wildPokemons != null)
                {
                    foreach (var pokemon in _wildPokemons)
                    {
                        var pokemonMarker = new GMarkerGoogle(new PointLatLng(pokemon.Latitude, pokemon.Longitude),
                            GMarkerGoogleType.red_small);
                        _pokemonsOverlay.Markers.Add(pokemonMarker);
                    }
                }

                _searchAreaOverlay.Polygons.Clear();
                S2GMapDrawer.DrawS2Cells(
                    S2Helper.GetNearbyCellIds(ClientSettings.DefaultLongitude, ClientSettings.DefaultLatitude),
                    _searchAreaOverlay);
            }, null);
        }

        private async Task ExecuteFarmingPokestopsAndPokemons(Client client)
        {
            var mapObjects = await client.Map.GetMapObjects();

            var rawPokeStops =
                mapObjects.Item1.MapCells.SelectMany(i => i.Forts)
                    .Where(
                        i =>
                            i.Type == FortType.Checkpoint &&
                            i.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime())
                    .ToArray();
            if (rawPokeStops == null || rawPokeStops.Count() <= 0)
            {
                ColoredConsoleWrite(Color.Red,
                    $"No PokeStops to visit here, please stop the bot and change your location.");
                return;
            }
            _pokeStops = rawPokeStops;
            UpdateMap();
            ColoredConsoleWrite(Color.Cyan, $"Finding fastest route through all PokeStops..");
            var startingLatLong = new LatLong(ClientSettings.DefaultLatitude, ClientSettings.DefaultLongitude);
            _pokeStops = RouteOptimizer.Optimize(rawPokeStops, startingLatLong, _pokestopsOverlay);
            _wildPokemons = mapObjects.Item1.MapCells.SelectMany(i => i.WildPokemons);
            if (!_forceUnbanning && !_stopping)
                ColoredConsoleWrite(Color.Cyan, $"Visiting {_pokeStops.Count()} PokeStops");

            UpdateMap();
            foreach (var pokeStop in _pokeStops)
            {
                if (_forceUnbanning || _stopping)
                    break;

                _farmingStops = true;
                await _locationManager.Update(pokeStop.Latitude, pokeStop.Longitude);
                UpdatePlayerLocation(pokeStop.Latitude, pokeStop.Longitude);
                UpdateMap();

                var fortInfo = await client.Fort.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                var fortSearch = await client.Fort.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                var PokeStopOutput = new StringWriter();
                PokeStopOutput.Write($"");
                if (fortInfo.Name != string.Empty)
                    PokeStopOutput.Write("PokeStop: " + fortInfo.Name);
                if (fortSearch.ExperienceAwarded != 0)
                    PokeStopOutput.Write($", XP: {fortSearch.ExperienceAwarded}");
                if (fortSearch.GemsAwarded != 0)
                    PokeStopOutput.Write($", Gems: {fortSearch.GemsAwarded}");
                if (fortSearch.PokemonDataEgg != null)
                    PokeStopOutput.Write($", Eggs: {fortSearch.PokemonDataEgg}");
                if (GetFriendlyItemsString(fortSearch.ItemsAwarded) != string.Empty)
                {
                    PokeStopOutput.Write($", Items: {GetFriendlyItemsString(fortSearch.ItemsAwarded)} ");
                }

                ColoredConsoleWrite(Color.Cyan, PokeStopOutput.ToString());

                await RecycleItems(client);

                if (fortSearch.ExperienceAwarded != 0)
                    _totalExperience += fortSearch.ExperienceAwarded;

                pokeStop.CooldownCompleteTimestampMs = DateTime.UtcNow.ToUnixTime() + 300000;

                if (ClientSettings.CatchPokemon)
                    await ExecuteCatchAllNearbyPokemons(client);
            }
            _farmingStops = false;
            /*if (!_forceUnbanning && !_stopping)
            {
                //await ExecuteFarmingPokestopsAndPokemons(client);
            }*/
        }

        private async Task ForceUnban(Client client)
        {
            if (!_forceUnbanning && !_stopping)
            {
                ColoredConsoleWrite(Color.LightGreen, "Waiting for last farming action to be complete...");
                _forceUnbanning = true;

                while (_farmingStops || _farmingPokemons)
                {
                    await Task.Delay(25);
                }

                ColoredConsoleWrite(Color.LightGreen, "Starting force unban...");

                _pokestopsOverlay.Routes.Clear();
                _pokestopsOverlay.Markers.Clear();
                var done = false;
                foreach (var pokeStop in _pokeStops)
                {
                    if (pokeStop.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime())
                    {
                        await _locationManager.Update(pokeStop.Latitude, pokeStop.Longitude);
                        UpdatePlayerLocation(pokeStop.Latitude, pokeStop.Longitude);
                        UpdateMap();

                        var fortInfo = await client.Fort.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                        if (fortInfo.Name != string.Empty)
                        {
                            ColoredConsoleWrite(Color.LightGreen,
                                "Chosen PokeStop " + fortInfo.Name + " for force unban");
                            for (var i = 1; i <= 50; i++)
                            {
                                var fortSearch =
                                    await client.Fort.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                                if (fortSearch.Result == FortSearchResponse.Types.Result.Success)
                                {
                                    if (fortSearch.ExperienceAwarded == 0)
                                    {
                                        ColoredConsoleWrite(Color.LightGreen, "Attempt: " + i);
                                    }
                                    else
                                    {
                                        ColoredConsoleWrite(Color.LightGreen,
                                            "Fuck yes, you are now unbanned! Total attempts: " + i);
                                        done = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    ColoredConsoleWrite(Color.LightGreen,
                                        $"Pokestop error on attempt {i}: {fortSearch.Result}");
                                }
                            }
                            if (done)
                                break;
                        }
                    }
                }
                if (!done)
                    ColoredConsoleWrite(Color.LightGreen, "Force unban failed, please try again.");
                _forceUnbanning = false;
            }
            else
            {
                ColoredConsoleWrite(Color.Red, "A action is in play... Please wait.");
            }
        }

        private string GetFriendlyItemsString(IEnumerable<ItemAward> items)
        {
            var enumerable = items as IList<ItemAward> ?? items.ToList();

            if (!enumerable.Any())
                return string.Empty;

            return enumerable.GroupBy(i => i.ItemId)
                .Select(kvp => new { ItemName = kvp.Key.ToString().Substring(4), Amount = kvp.Sum(x => x.ItemCount) })
                .Select(y => $"{y.Amount}x {y.ItemName}")
                .Aggregate((a, b) => $"{a}, {b}");
        }

        private async Task TransferAllButStrongestUnwantedPokemon(Client client)
        {
            var unwantedPokemonTypes = new List<PokemonId>();
            for (var i = 1; i <= 151; i++)
            {
                unwantedPokemonTypes.Add((PokemonId)i);
            }

            var inventory = await client.Inventory.GetInventory();
            var pokemons = inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.PokemonData)
                .Where(p => p != null && p?.PokemonId > 0)
                .ToArray();

            foreach (var unwantedPokemonType in unwantedPokemonTypes)
            {
                var pokemonOfDesiredType = pokemons.Where(p => p.PokemonId == unwantedPokemonType)
                    .OrderByDescending(p => p.Cp)
                    .ToList();

                var unwantedPokemon =
                    pokemonOfDesiredType.Skip(1) // keep the strongest one for potential battle-evolving
                        .ToList();

                await TransferAllGivenPokemons(client, unwantedPokemon);
            }
        }

        public static float Perfect(PokemonData poke)
        {
            if (poke == null)
                return 0f;
            return (poke.IndividualAttack + poke.IndividualDefense + poke.IndividualStamina) / 45f * 100f;
        }

        private async Task TransferAllGivenPokemons(Client client, IEnumerable<PokemonData> unwantedPokemons,
            float keepPerfectPokemonLimit = 80.0f)
        {
            var excludedPokemon = Settings.Instance.ExcludedPokemonTransfer;
            foreach (var pokemon in unwantedPokemons)
            {
                if (excludedPokemon.Contains(pokemon.PokemonId))
                {
                    continue;
                }

                if (Perfect(pokemon) >= keepPerfectPokemonLimit) continue;
                ColoredConsoleWrite(Color.White,
                    $"Pokemon {pokemon.PokemonId} with {pokemon.Cp} CP has IV percent less than {keepPerfectPokemonLimit}%");

                if (pokemon.Favorite == 0)
                {
                    var transferPokemonResponse = await client.Inventory.TransferPokemon(pokemon.Id);

                    /*
                    ReleasePokemonOutProto.Status {
                        UNSET = 0;
                        SUCCESS = 1;
                        POKEMON_DEPLOYED = 2;
                        FAILED = 3;
                        ERROR_POKEMON_IS_EGG = 4;
                    }*/
                    string pokemonName;
                    if (ClientSettings.Language == "german")
                    {
                        // Dont really need to print this do we? youll know if its German or not
                        //ColoredConsoleWrite(Color.DarkCyan, "german");
                        var name_english = Convert.ToString(pokemon.PokemonId);
                        var request =
                            (HttpWebRequest)
                                WebRequest.Create("http://boosting-service.de/pokemon/index.php?pokeName=" +
                                                  name_english);
                        var response = (HttpWebResponse)request.GetResponse();
                        pokemonName = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    }
                    else
                        pokemonName = Convert.ToString(pokemon.PokemonId);
                    if (transferPokemonResponse.Result == ReleasePokemonResponse.Types.Result.Success)
                    {
                        ColoredConsoleWrite(Color.Magenta, $"Transferred {pokemonName} with {pokemon.Cp} CP");
                    }
                    else
                    {
                        var status = transferPokemonResponse.Result;

                        ColoredConsoleWrite(Color.Red,
                            $"Somehow failed to transfer {pokemonName} with {pokemon.Cp} CP. " +
                            $"ReleasePokemonOutProto.Status was {status}");
                    }

                    await Task.Delay(3000);
                }
            }
        }

        private async Task TransferDuplicatePokemon(Client client)
        {
            var excludedPokemon = Settings.Instance.ExcludedPokemonTransfer;
            //ColoredConsoleWrite(ConsoleColor.White, $"Check for duplicates");
            var inventory = await client.Inventory.GetInventory();
            var allpokemons =
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonData)
                    .Where(p => p != null && p?.PokemonId > 0);

            var dupes = allpokemons.OrderBy(x => x.Cp).Select((x, i) => new { index = i, value = x })
                .GroupBy(x => x.value.PokemonId)
                .Where(x => x.Skip(1).Any());

            for (var i = 0; i < dupes.Count(); i++)
            {
                for (var j = 0; j < dupes.ElementAt(i).Count() - 1; j++)
                {
                    var dubpokemon = dupes.ElementAt(i).ElementAt(j).value;

                    if (excludedPokemon.Contains(dubpokemon.PokemonId))
                    {
                        continue;
                    }

                    if (dubpokemon.Favorite == 0)
                    {
                        var transfer = await client.Inventory.TransferPokemon(dubpokemon.Id);
                        string pokemonName;
                        if (ClientSettings.Language == "german")
                        {
                            var name_english = Convert.ToString(dubpokemon.PokemonId);
                            var request =
                                (HttpWebRequest)
                                    WebRequest.Create("http://boosting-service.de/pokemon/index.php?pokeName=" +
                                                      name_english);
                            var response = (HttpWebResponse)request.GetResponse();
                            pokemonName = new StreamReader(response.GetResponseStream()).ReadToEnd();
                        }
                        else
                            pokemonName = Convert.ToString(dubpokemon.PokemonId);
                        ColoredConsoleWrite(Color.DarkGreen,
                            $"Transferred {pokemonName} with {dubpokemon.Cp} CP (Highest is {dupes.ElementAt(i).Last().value.Cp})");
                    }
                }
            }
        }

        private async Task TransferDuplicateIVPokemon(Client client)
        {
            var excludedPokemon = Settings.Instance.ExcludedPokemonTransfer;
            //ColoredConsoleWrite(ConsoleColor.White, $"Check for duplicates");
            var inventory = await client.Inventory.GetInventory();
            var allpokemons =
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonData)
                    .Where(p => p != null && p?.PokemonId > 0);

            var dupes = allpokemons.OrderBy(x => Perfect(x)).Select((x, i) => new { index = i, value = x })
                .GroupBy(x => x.value.PokemonId)
                .Where(x => x.Skip(1).Any());

            for (var i = 0; i < dupes.Count(); i++)
            {
                for (var j = 0; j < dupes.ElementAt(i).Count() - 1; j++)
                {
                    var dubpokemon = dupes.ElementAt(i).ElementAt(j).value;

                    if (excludedPokemon.Contains(dubpokemon.PokemonId))
                    {
                        continue;
                    }

                    if (dubpokemon.Favorite == 0)
                    {
                        var transfer = await client.Inventory.TransferPokemon(dubpokemon.Id);
                        string pokemonName;
                        if (ClientSettings.Language == "german")
                        {
                            var name_english = Convert.ToString(dubpokemon.PokemonId);
                            var request =
                                (HttpWebRequest)
                                    WebRequest.Create("http://boosting-service.de/pokemon/index.php?pokeName=" +
                                                      name_english);
                            var response = (HttpWebResponse)request.GetResponse();
                            pokemonName = new StreamReader(response.GetResponseStream()).ReadToEnd();
                        }
                        else
                            pokemonName = Convert.ToString(dubpokemon.PokemonId);
                        ColoredConsoleWrite(Color.DarkGreen,
                            $"Transferred {pokemonName} with {Math.Round(Perfect(dubpokemon))}% IV (Highest is {Math.Round(Perfect(dupes.ElementAt(i).Last().value))}% IV)");
                    }
                }
            }
        }

        private async Task TransferDuplicateCPIVPokemon(Client client)
        {
            //ColoredConsoleWrite(Color.White, $"Check for CP/IV duplicates");
            var inventory = await client.Inventory.GetInventory();
            var allpokemons =
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonData)
                    .Where(p => p != null && p?.PokemonId > 0);

            var dupes = allpokemons.OrderBy(x => Perfect(x)).Select((x, i) => new { index = i, value = x })
                .GroupBy(x => x.value.PokemonId)
                .Where(x => x.Skip(1).Any());

            for (var i = 0; i < dupes.Count(); i++)
            {
                var dupe_group = dupes.ElementAt(i);
                var max_cp = 0;
                var max_index = -1;
                for (var j = 0; j < dupe_group.Count(); j++)
                {
                    var this_cp = dupe_group.ElementAt(j).value.Cp;
                    if (this_cp >= max_cp)
                    {
                        max_cp = this_cp;
                        max_index = j;
                    }
                }
                for (var j = 0; j < dupes.ElementAt(i).Count() - 1; j++)
                {
                    var dubpokemon = dupes.ElementAt(i).ElementAt(j).value;
                    if (dubpokemon.Favorite == 0 && j != max_index)
                    {
                        var transfer = await client.Inventory.TransferPokemon(dubpokemon.Id);
                        string pokemonName;
                        if (ClientSettings.Language == "german")
                        {
                            var name_english = Convert.ToString(dubpokemon.PokemonId);
                            var request =
                                (HttpWebRequest)
                                    WebRequest.Create("http://boosting-service.de/pokemon/index.php?pokeName=" +
                                                      name_english);
                            var response = (HttpWebResponse)request.GetResponse();
                            pokemonName = new StreamReader(response.GetResponseStream()).ReadToEnd();
                        }
                        else
                            pokemonName = Convert.ToString(dubpokemon.PokemonId);
                        ColoredConsoleWrite(Color.DarkGreen,
                            $"Transferred {pokemonName} with {dubpokemon.Cp} CP, {Math.Round(Perfect(dubpokemon))}% IV (Highest is {max_cp} CP/{Math.Round(Perfect(dupes.ElementAt(i).Last().value))}% IV)");
                    }
                }
            }
        }

        private async Task TransferAllWeakPokemon(Client client, int cpThreshold)
        {
            //ColoredConsoleWrite(ConsoleColor.White, $"Firing up the meat grinder");

            var inventory = await client.Inventory.GetInventory();
            var pokemons = inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.PokemonData)
                .Where(p => p != null && p?.PokemonId > 0)
                .ToArray();

            var pokemonToDiscard = pokemons.Where(p => p.Cp < cpThreshold).OrderByDescending(p => p.Cp).ToList();
            ColoredConsoleWrite(Color.Gray, $"Grinding {pokemonToDiscard.Count} pokemon below {cpThreshold} CP.");
            await TransferAllGivenPokemons(client, pokemonToDiscard);

            ColoredConsoleWrite(Color.Gray, $"Finished grinding all the meat");
        }

        public async Task PrintLevel(Client client)
        {
            var inventory = await client.Inventory.GetInventory();
            var stats = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PlayerStats);
            foreach (var v in stats)
                if (v != null)
                {
                    var XpDiff = GetXpDiff(client, v.Level);
                    if (ClientSettings.LevelOutput == "time")
                        ColoredConsoleWrite(Color.Yellow,
                            $"Current Level: " + v.Level + " (" + (v.Experience - XpDiff) + "/" +
                            (v.NextLevelXp - XpDiff) + ")");
                    else if (ClientSettings.LevelOutput == "levelup")
                        if (_currentlevel != v.Level)
                        {
                            _currentlevel = v.Level;
                            ColoredConsoleWrite(Color.Magenta,
                                $"Current Level: " + v.Level + ". XP needed for next Level: " +
                                (v.NextLevelXp - v.Experience));
                        }
                }
            if (ClientSettings.LevelOutput == "levelup")
                await Task.Delay(1000);
            else

                await Task.Delay(ClientSettings.LevelTimeInterval * 1000);
            // PrintLevel(client);
        }

        // Pulled from NecronomiconCoding
        public static string _getSessionRuntimeInTimeFormat()
        {
            return (DateTime.Now - InitSessionDateTime).ToString(@"dd\.hh\:mm\:ss");
        }

        public async Task updateUserStatusBar(Client client, GetInventoryResponse inventory, GetPlayerResponse profile)
        {
            var stats =
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PlayerStats)
                    .Where(i => i != null)
                    .ToArray();
            short hoursLeft = 0;
            short minutesLeft = 0;
            var secondsLeft = 0;
            double xpSec = 0;
            var v = stats.First();
            if (v != null)
            {
                var XpDiff = GetXpDiff(client, v.Level);
                //Calculating the exp needed to level up
                float expNextLvl = v.NextLevelXp - v.Experience;
                //Calculating the exp made per second
                xpSec = Math.Round(_totalExperience / GetRuntime()) / 60 / 60;
                //Calculating the seconds left to level up
                if (xpSec != 0)
                    secondsLeft = Convert.ToInt32(expNextLvl / xpSec);
                //formatting data to make an output like DateFormat
                while (secondsLeft > 60)
                {
                    secondsLeft -= 60;
                    if (minutesLeft < 60)
                    {
                        minutesLeft++;
                    }
                    else
                    {
                        minutesLeft = 0;
                        hoursLeft++;
                    }
                }
                SetStatusText(
                    string.Format(
                        profile.PlayerData.Username +
                        " | Level: {0:0} - ({2:0} / {3:0}) | Runtime {1} | Stardust: {4:0}", v.Level,
                        _getSessionRuntimeInTimeFormat(), v.Experience - v.PrevLevelXp - XpDiff,
                        v.NextLevelXp - v.PrevLevelXp - XpDiff, profile.PlayerData.Currencies.ToArray()[1].Amount) +
                    " | XP/Hour: " + Math.Round(_totalExperience / GetRuntime()) + " | Pokemon/Hour: " +
                    Math.Round(_totalPokemon / GetRuntime()) + " | NextLevel in: " + hoursLeft + ":" + minutesLeft +
                    ":" + secondsLeft);
            }
            await Task.Delay(1000);
        }

        public static int GetXpDiff(Client client, int Level)
        {
            switch (Level)
            {
                case 1:
                    return 0;

                case 2:
                    return 1000;

                case 3:
                    return 2000;

                case 4:
                    return 3000;

                case 5:
                    return 4000;

                case 6:
                    return 5000;

                case 7:
                    return 6000;

                case 8:
                    return 7000;

                case 9:
                    return 8000;

                case 10:
                    return 9000;

                case 11:
                    return 10000;

                case 12:
                    return 10000;

                case 13:
                    return 10000;

                case 14:
                    return 10000;

                case 15:
                    return 15000;

                case 16:
                    return 20000;

                case 17:
                    return 20000;

                case 18:
                    return 20000;

                case 19:
                    return 25000;

                case 20:
                    return 25000;

                case 21:
                    return 50000;

                case 22:
                    return 75000;

                case 23:
                    return 100000;

                case 24:
                    return 125000;

                case 25:
                    return 150000;

                case 26:
                    return 190000;

                case 27:
                    return 200000;

                case 28:
                    return 250000;

                case 29:
                    return 300000;

                case 30:
                    return 350000;

                case 31:
                    return 500000;

                case 32:
                    return 500000;

                case 33:
                    return 750000;

                case 34:
                    return 1000000;

                case 35:
                    return 1250000;

                case 36:
                    return 1500000;

                case 37:
                    return 2000000;

                case 38:
                    return 2500000;

                case 39:
                    return 1000000;

                case 40:
                    return 1000000;
            }
            return 0;
        }

        public void confirmBotStopped()
        {
            //ConsoleClear(); // dont really want the console to be wipped on bot stop, unnecessary
            ColoredConsoleWrite(Color.Red, $"Bot successfully stopped.");
            startStopBotToolStripMenuItem.Text = "▶ Start Bot";
            _stopping = false;
            _botStarted = false;
        }

        private void logTextBox_TextChanged(object sender, EventArgs e)
        {
            logTextBox.SelectionStart = logTextBox.Text.Length;
            logTextBox.ScrollToCaret();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }

        private void startStopBotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_botStarted)
            {
                _botStarted = true;
                startStopBotToolStripMenuItem.Text = "■ Stop Bot";
                Task.Run(async () =>
                {
                    //CheckVersion();
                    while (true)
                    {
                        try
                        {
                            //ColoredConsoleWrite(ConsoleColor.White, "Coded by Ferox - edited by NecronomiconCoding");
                            if (!_botStarted)
                            {
                                break;
                            }
                            await Execute();
                        }
                        catch (PtcOfflineException)
                        {
                            ColoredConsoleWrite(Color.Red,
                                "PTC Servers are probably down OR your credentials are wrong. Try google");
                        }
                        catch (Exception ex)
                        {
                            ColoredConsoleWrite(Color.Red, $"Unhandled exception: {ex}");
                        }
                    }
                });
            }
            else
            {
                if (!_forceUnbanning)
                {
                    _stopping = true;
                    ColoredConsoleWrite(Color.Red, $"Stopping the bot.. Waiting for the last action to be complete.");
                }
                else
                {
                    ColoredConsoleWrite(Color.Red, $"An action is in play, please wait until it's done.");
                }
            }
        }

        private void Client_OnConsoleWrite(ConsoleColor color, string message)
        {
            var c = Color.White;
            switch (color)
            {
                case ConsoleColor.Green:
                    c = Color.Green;
                    break;

                case ConsoleColor.DarkCyan:
                    c = Color.DarkCyan;
                    break;
            }
            ColoredConsoleWrite(c, message);
        }

        private async void forceUnbanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_client != null && _pokeStops != null)
            {
                if (_forceUnbanning)
                {
                    ColoredConsoleWrite(Color.Red, "A force unban attempt is in action... Please wait.");
                }
                else
                {
                    await ForceUnban(_client);
                }
            }
            else
            {
                ColoredConsoleWrite(Color.Red,
                    "Please start the bot and wait for map to load before trying to force unban");
            }
        }

        private void todoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }

        #region POKEMON LIST

        private IEnumerable<Candy> families;

        private void InitializePokemonForm()
        {
            olvPokemonList.ButtonClick += PokemonListButton_Click;

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
                    return;
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

        private Image GetPokemonImage(int pokemonId)
        {
            return (Image)Properties.Resources.ResourceManager.GetObject("Pokemon_" + pokemonId);
        }

        private void SetState(bool state)
        {
            btnRefresh.Enabled = state;
            olvPokemonList.Enabled = state;
            flpItems.Enabled = state;
        }

        private async Task ReloadPokemonList()
        {
            SetState(false);

            try
            {
                _client2 = new Client(ClientSettings, new ApiFailureStrategy());
                await _client2.Login.DoLogin();

                var inventory = await _client2.Inventory.GetInventory();
                var profile = await _client2.Player.GetPlayer();
                var itemTemplates = await _client2.Download.GetItemTemplates();

                var appliedItems = new Dictionary<ItemId, DateTime>();
                var inventoryAppliedItems =
                    inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.AppliedItems);

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
                families = inventory.InventoryDelta.InventoryItems
                    .Select(i => i.InventoryItemData.Candy)
                    .Where(p => p != null && p.FamilyId > 0)
                    .OrderByDescending(p => p.FamilyId);

                var pokemonObjects = new List<PokemonObject>();
                foreach (var pokemon in pokemons)
                {
                    var pokemonObject = new PokemonObject(pokemon);
                    var family =
                        families.Where(i => (int)i.FamilyId <= (int)pokemon.PokemonId)
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
            catch (GoogleException)
            {
                ColoredConsoleWrite(Color.Red, "Please check your google login information again");
            }
            catch (LoginFailedException)
            {
                ColoredConsoleWrite(Color.Red, "Login failed, please check your ptc login information again");
            }
            catch (AccessTokenExpiredException ex)
            {
                ColoredConsoleWrite(Color.Red, ex.Message);
            }
            catch (Exception ex)
            {
                ColoredConsoleWrite(Color.Red, ex.ToString());
                _client2 = null;
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
                        if (!_botStarted)
                        {
                            ColoredConsoleWrite(Color.Red, $"Bot must be running first!");
                            SetState(true);
                            return;
                        }
                        var response = await _client.Inventory.UseItemXpBoost();
                        if (response.Result == UseItemXpBoostResponse.Types.Result.Success)
                        {
                            ColoredConsoleWrite(Color.Green, $"Using a Lucky Egg");
                            ColoredConsoleWrite(Color.Yellow, $"Lucky Egg valid until: {DateTime.Now.AddMinutes(30)}");
                        }
                        else if (response.Result == UseItemXpBoostResponse.Types.Result.ErrorXpBoostAlreadyActive)
                        {
                            ColoredConsoleWrite(Color.Orange, $"A Lucky Egg is already active!");
                        }
                        else if (response.Result == UseItemXpBoostResponse.Types.Result.ErrorLocationUnset)
                        {
                            ColoredConsoleWrite(Color.Red, $"Bot must be running first!");
                        }
                        else
                        {
                            ColoredConsoleWrite(Color.Red, $"Failed using a Lucky Egg!");
                        }
                    }
                    else if (item.ItemId == ItemId.ItemIncenseOrdinary)
                    {
                        if (!_botStarted)
                        {
                            ColoredConsoleWrite(Color.Red, $"Bot must be running first!");
                            SetState(true);
                            return;
                        }
                        var response = await _client.Inventory.UseIncense(ItemId.ItemIncenseOrdinary);
                        if (response.Result == UseIncenseResponse.Types.Result.Success)
                        {
                            ColoredConsoleWrite(Color.Green, $"Using an incense");
                            ColoredConsoleWrite(Color.Yellow, $"Incense valid until: {DateTime.Now.AddMinutes(30)}");
                        }
                        else if (response.Result == UseIncenseResponse.Types.Result.IncenseAlreadyActive)
                        {
                            ColoredConsoleWrite(Color.Orange, $"An incense is already active!");
                        }
                        else if (response.Result == UseIncenseResponse.Types.Result.LocationUnset)
                        {
                            ColoredConsoleWrite(Color.Red, $"Bot must be running first!");
                        }
                        else
                        {
                            ColoredConsoleWrite(Color.Red, $"Failed using an incense!");
                        }
                    }
                    else
                    {
                        var response =
                            await _client2.Inventory.RecycleItem(item.ItemId, decimal.ToInt32(form.numCount.Value));
                        if (response.Result == RecycleInventoryItemResponse.Types.Result.Success)
                        {
                            ColoredConsoleWrite(Color.DarkCyan,
                                $"Recycled {decimal.ToInt32(form.numCount.Value)}x {item.ItemId.ToString().Substring(4)}");
                        }
                        else
                        {
                            ColoredConsoleWrite(Color.Red,
                                $"Unable to recycle {decimal.ToInt32(form.numCount.Value)}x {item.ItemId.ToString().Substring(4)}");
                        }
                    }
                    ReloadPokemonList();
                }
            }
        }

        private void PokemonListButton_Click(object sender, CellClickEventArgs e)
        {
            try
            {
                var pokemon = e.Model as PokemonObject;
                var cName = olvPokemonList.AllColumns[e.ColumnIndex].AspectToStringFormat;
                if (cName.Equals("Transfer"))
                {
                    TransferPokemon(new List<PokemonData> { pokemon.PokemonData });
                }
                else if (cName.Equals("Power Up"))
                {
                    PowerUpPokemon(new List<PokemonData> { pokemon.PokemonData });
                }
                else if (cName.Equals("Evolve"))
                {
                    EvolvePokemon(new List<PokemonData> { pokemon.PokemonData });
                }
            }
            catch (Exception ex)
            {
                ColoredConsoleWrite(Color.Red, ex.ToString());
                _client2 = null;
                ReloadPokemonList();
            }
        }

        private async void TransferPokemon(IEnumerable<PokemonData> pokemons)
        {
            SetState(false);
            foreach (var pokemon in pokemons)
            {
                var transferPokemonResponse = await _client2.Inventory.TransferPokemon(pokemon.Id);
                if (transferPokemonResponse.Result == ReleasePokemonResponse.Types.Result.Success)
                {
                    ColoredConsoleWrite(Color.Magenta,
                        $"{pokemon.PokemonId} was transferred. {transferPokemonResponse.CandyAwarded} candy awarded");
                }
                else
                {
                    ColoredConsoleWrite(Color.Magenta, $"{pokemon.PokemonId} could not be transferred");
                }
            }
            ReloadPokemonList();
        }

        private async void PowerUpPokemon(IEnumerable<PokemonData> pokemons)
        {
            SetState(false);
            foreach (var pokemon in pokemons)
            {
                var evolvePokemonResponse = await _client2.Inventory.UpgradePokemon(pokemon.Id);
                if (evolvePokemonResponse.Result == UpgradePokemonResponse.Types.Result.Success)
                {
                    ColoredConsoleWrite(Color.Magenta, $"{pokemon.PokemonId} successfully upgraded.");
                }
                else
                {
                    ColoredConsoleWrite(Color.Magenta, $"{pokemon.PokemonId} could not be upgraded");
                }
            }
            ReloadPokemonList();
        }

        private async void EvolvePokemon(IEnumerable<PokemonData> pokemons)
        {
            SetState(false);
            foreach (var pokemon in pokemons)
            {
                var evolvePokemonResponse = await _client2.Inventory.EvolvePokemon(pokemon.Id);
                if (evolvePokemonResponse.Result == EvolvePokemonResponse.Types.Result.Success)
                {
                    ColoredConsoleWrite(Color.Magenta,
                        $"{pokemon.PokemonId} successfully evolved into {evolvePokemonResponse.EvolvedPokemonData.PokemonId}\n{evolvePokemonResponse.ExperienceAwarded} experience awarded\n{evolvePokemonResponse.CandyAwarded} candy awarded");
                }
                else
                {
                    ColoredConsoleWrite(Color.Magenta, $"{pokemon.PokemonId} could not be evolved");
                }
            }
            ReloadPokemonList();
        }

        private async Task UseIncubators(Client client, String mode)
        {
            var profile = (await GetProfile(client)).FirstOrDefault();

            if (profile == null)
                return;

            var kmWalked = profile.KmWalked;

            var unusedEggs = (await getUnusedEggs(client))
                .Where(x => string.IsNullOrEmpty(x.EggIncubatorId))
                .OrderBy(x => x.EggKmWalkedTarget - x.EggKmWalkedStart)
                .ToList();
            var incubators = (await getUnusedIncubators(client))
                .Where(x => x.UsesRemaining > 0 || x.ItemId == ItemId.ItemIncubatorBasicUnlimited)
                .OrderByDescending(x => x.ItemId == ItemId.ItemIncubatorBasicUnlimited)
                .OrderByDescending(x => x.PokemonId != 0)
                .ToList();

            var num = 1;

            foreach (var inc in incubators)
            {
                var usesLeft = (inc.ItemId == ItemId.ItemIncubatorBasicUnlimited) ?
                "∞" : inc.UsesRemaining.ToString();
                if (inc.PokemonId == 0)
                {
                    if (mode.Equals("only unlimited") 
                        && inc.ItemId != ItemId.ItemIncubatorBasicUnlimited)
                        continue;

                    var egg = (inc.ItemId == ItemId.ItemIncubatorBasicUnlimited && incubators.Count > 1)
                    ? unusedEggs.FirstOrDefault()
                    : unusedEggs.LastOrDefault();

                    if (egg == null)
                        continue;

                    var useIncubator = await client.Inventory.UseItemEggIncubator(inc.Id, egg.Id);
                    unusedEggs.Remove(egg);
                    var eggKm = egg.EggKmWalkedTarget;
                    ColoredConsoleWrite(Color.YellowGreen, $"Incubator #{num} was successfully used on a {eggKm}km egg, Incubator uses left: {usesLeft}");
                }
                else
                {
                    var remainingDistance = String.Format("{0:0.00}", (inc.TargetKmWalked - kmWalked));
                    var eggKm = inc.TargetKmWalked - inc.StartKmWalked;
                    ColoredConsoleWrite(Color.YellowGreen, $"[Status] Incubator #{num}, Uses left: {usesLeft}, Distance left: {remainingDistance}/{eggKm} km");
                }
                num++;
            }
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

        public async Task<IEnumerable<ItemData>> GetItems(Client client)
        {
            var inventory = await client.Inventory.GetInventory();
            return inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.Item)
                .Where(p => p != null);
        }

        private async Task<IEnumerable<EggIncubator>> getUnusedIncubators(Client client)
        {
            var inventory = await client.Inventory.GetInventory();
            return inventory.InventoryDelta.InventoryItems.
            Where(x => x.InventoryItemData?.EggIncubators != null).
            SelectMany(x => x.InventoryItemData.EggIncubators.EggIncubator).
            Where(x => x != null);
        }

        private async Task<IEnumerable<PokemonData>> getUnusedEggs(Client client)
        {
            var inventory = await client.Inventory.GetInventory();
            return inventory.InventoryDelta.InventoryItems.
            Select(i => i.InventoryItemData?.PokemonData).
            Where(p => p != null && p.IsEgg).ToList().
            Where(x => string.IsNullOrEmpty(x.EggIncubatorId)).
            OrderBy(x => x.EggKmWalkedTarget - x.EggKmWalkedStart);
        }

        private async Task<IEnumerable<PlayerStats>> GetProfile(Client client)
        {
            var inventory = await client.Inventory.GetInventory();
            return inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.PlayerStats)
                .Where(p => p != null);
        }

        public async Task<IEnumerable<ItemData>> GetItemsToRecycle(ISettings _settings, Client client)
        {
            var itemCounts = (_settings as Settings).ItemCounts;
            var settings = (Settings)_settings;
            var myItems = await GetItems(client);
            var itemsToRecycle = new List<ItemData>();

            foreach (var itemData in myItems)
            {
                foreach (var itemCount in itemCounts)
                {
                    if (itemData.ItemId == itemCount.ItemId && itemData.Count > itemCount.Count)
                    {
                        var itemToRecycle = new ItemData();
                        itemToRecycle.ItemId = itemData.ItemId;
                        itemToRecycle.Count = itemData.Count - itemCount.Count;
                        itemsToRecycle.Add(itemToRecycle);
                    }
                }
            }

            return itemsToRecycle;
        }

        public async Task RecycleItems(Client client)
        {
            var items = await GetItemsToRecycle(client.Settings, client);

            foreach (var item in items)
            {
                var transfer = await client.Inventory.RecycleItem(item.ItemId, item.Count);
                ColoredConsoleWrite(Color.DarkCyan, $"Recycled {item.Count}x {item.ItemId.ToString().Substring(4)}");
                //await Task.Delay(500);
            }
        }

        public async Task UseRazzBerry(Client client, ulong encounterId, string spawnPointGuid)
        {
            var myItems = await GetItems(client);
            var RazzBerries = myItems.Where(i => i.ItemId == ItemId.ItemRazzBerry);
            var RazzBerry = RazzBerries.FirstOrDefault();
            if (RazzBerry != null)
            {
                var useRazzBerry =
                    await client.Encounter.UseCaptureItem(encounterId, ItemId.ItemRazzBerry, spawnPointGuid);
                ColoredConsoleWrite(Color.Green, $"Using a Razz Berry, we have {RazzBerry.Count} left");
                await Task.Delay(2000);
            }
            else
            {
                ColoredConsoleWrite(Color.Red, $"You don't have any Razz Berry to use.");
            }
        }

        public async Task<CatchPokemonResponse> CatchPokemon(ulong encounterId, string spawnPointGuid, double pokemonLat,
            double pokemonLng, ItemId pokeball, int? pokemonCP)
        {
            return await _client.Encounter.CatchPokemon(encounterId, spawnPointGuid, GetBestBall(pokemonCP).Result);
        }

        private async Task<ItemId> GetBestBall(int? pokemonCP)
        {
            var inventory = await _client.Inventory.GetInventory();

            var ballCollection = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Item)
                .Where(p => p != null)
                .GroupBy(i => i.ItemId)
                .Select(kvp => new { ItemId = kvp.Key, Amount = kvp.Sum(x => x.Count) })
                .Where(y => y.ItemId == ItemId.ItemPokeBall
                            || y.ItemId == ItemId.ItemGreatBall
                            || y.ItemId == ItemId.ItemUltraBall
                            || y.ItemId == ItemId.ItemMasterBall);

            var pokeBallsCount = ballCollection.Where(p => p.ItemId == ItemId.ItemPokeBall).
                DefaultIfEmpty(new { ItemId = ItemId.ItemPokeBall, Amount = 0 }).FirstOrDefault().Amount;
            var greatBallsCount = ballCollection.Where(p => p.ItemId == ItemId.ItemGreatBall).
                DefaultIfEmpty(new { ItemId = ItemId.ItemGreatBall, Amount = 0 }).FirstOrDefault().Amount;
            var ultraBallsCount = ballCollection.Where(p => p.ItemId == ItemId.ItemUltraBall).
                DefaultIfEmpty(new { ItemId = ItemId.ItemUltraBall, Amount = 0 }).FirstOrDefault().Amount;
            var masterBallsCount = ballCollection.Where(p => p.ItemId == ItemId.ItemMasterBall).
                DefaultIfEmpty(new { ItemId = ItemId.ItemMasterBall, Amount = 0 }).FirstOrDefault().Amount;

            // Use better balls for high CP pokemon
            if (masterBallsCount > 0 && pokemonCP >= 1000)
            {
                ColoredConsoleWrite(Color.Green, $"Master Ball is being used");
                return ItemId.ItemMasterBall;
            }

            if (ultraBallsCount > 0 && pokemonCP >= 600)
            {
                ColoredConsoleWrite(Color.Green, $"Ultra Ball is being used");
                return ItemId.ItemUltraBall;
            }

            if (greatBallsCount > 0 && pokemonCP >= 350)
            {
                ColoredConsoleWrite(Color.Green, $"Great Ball is being used");
                return ItemId.ItemGreatBall;
            }

            // If low CP pokemon, but no more pokeballs; only use better balls if pokemon are of semi-worthy quality
            if (pokeBallsCount > 0)
            {
                ColoredConsoleWrite(Color.Green, $"Poke Ball is being used");
                return ItemId.ItemPokeBall;
            }
            if ((greatBallsCount < 40 && pokemonCP >= 200) || greatBallsCount >= 40)
            {
                ColoredConsoleWrite(Color.Green, $"Great Ball is being used");
                return ItemId.ItemGreatBall;
            }
            if (ultraBallsCount > 0 && pokemonCP >= 500)
            {
                ColoredConsoleWrite(Color.Green, $"Ultra Ball is being used");
                return ItemId.ItemUltraBall;
            }
            if (masterBallsCount > 0 && pokemonCP >= 700)
            {
                ColoredConsoleWrite(Color.Green, $"Master Ball is being used");
                return ItemId.ItemMasterBall;
            }

            return ItemId.ItemPokeBall;
        }

        public async void NicknamePokemon(IEnumerable<PokemonData> pokemons, string nickname)
        {
            SetState(false);
            foreach (var pokemon in pokemons)
            {
                var newName = new StringBuilder(nickname);
                newName.Replace("{Name}", Convert.ToString(pokemon.PokemonId));
                newName.Replace("{CP}", Convert.ToString(pokemon.Cp));
                newName.Replace("{IV}", Convert.ToString(Math.Round(Perfect(pokemon))));
                newName.Replace("{IA}", Convert.ToString(pokemon.IndividualAttack));
                newName.Replace("{ID}", Convert.ToString(pokemon.IndividualDefense));
                newName.Replace("{IS}", Convert.ToString(pokemon.IndividualStamina));
                nickname = newName.ToString();
                if (nickname.Length > 12)
                {
                    ColoredConsoleWrite(Color.Red, $"\"{nickname}\" is too long, please choose another name");
                    continue;
                }
                var response = await _client2.Inventory.NicknamePokemon(pokemon.Id, nickname);
                if (response.Result == NicknamePokemonResponse.Types.Result.Success)
                {
                    ColoredConsoleWrite(Color.Green, $"Successfully renamed {pokemon.PokemonId} to \"{nickname}\"");
                }
                else
                {
                    ColoredConsoleWrite(Color.Red, $"Failed renaming {pokemon.PokemonId} to \"{nickname}\"");
                }
                await Task.Delay(ACTIONDELAY);
            }
            await ReloadPokemonList();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await ReloadPokemonList();
        }

        #endregion POKEMON LIST
    }
}