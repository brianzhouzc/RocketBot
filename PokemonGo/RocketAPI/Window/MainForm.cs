using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using BrightIdeasSoftware;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.Helpers;
using POGOProtos.Data;
using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
using POGOProtos.Map.Fort;
using POGOProtos.Map.Pokemon;
using POGOProtos.Networking.Responses;
using static System.Reflection.Assembly;

namespace PokemonGo.RocketAPI.Window
{
    public partial class MainForm : Form
    {
        public static MainForm Instance;
        public static SynchronizationContext SynchronizationContext;

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

        private Client _client;
        private Client _client2;
        private LocationManager _locationManager;

        private GMarkerGoogle _playerMarker;
        private readonly GMapOverlay _playerOverlay = new GMapOverlay("players");
        private readonly GMapOverlay _pokemonsOverlay = new GMapOverlay("pokemons");

        private IEnumerable<FortData> _pokeStops;
        private readonly GMapOverlay _pokestopsOverlay = new GMapOverlay("pokestops");

        private readonly GMapOverlay _searchAreaOverlay = new GMapOverlay("areas");
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

        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
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
                    "You can find it at www.GitHub.com/1461748123/Pokemon-Go-Rocket-API/releases");
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
                        "https://raw.githubusercontent.com/1461748123/Pokemon-Go-Rocket-API/master/PokemonGo/RocketAPI/Window/Properties/AssemblyInfo.cs");
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
            foreach (var pokemon in pokemonToEvolve)
            {
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

        private async void Execute()
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
                var settings = await _client.Download.GetSettings();
                var mapObjects = await _client.Map.GetMapObjects();
                var inventory = await _client.Inventory.GetInventory();
                var pokemons =
                    inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonData)
                        .Where(p => p != null && p?.PokemonId > 0);

                updateUserStatusBar(_client);

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
                    ColoredConsoleWrite(Color.Cyan, "Email: " + ClientSettings.Email);
                    ColoredConsoleWrite(Color.Cyan, "Password: " + ClientSettings.Password + "\n");
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
                        await TransferAllGivenPokemons(_client, pokemons);
                        break;
                    case "Duplicate":
                        await TransferDuplicatePokemon(_client);
                        break;
                    case "IV Duplicate":
                        await TransferDuplicateIVPokemon(_client);
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
                    RecycleItems(_client);
                //client.RecycleItems(client);

                await Task.Delay(5000);
                PrintLevel(_client);
                await ExecuteFarmingPokestopsAndPokemons(_client);

                while (_forceUnbanning)
                    await Task.Delay(25);

                // await ForceUnban(client);
                if (!_stopping)
                {
                    ColoredConsoleWrite(Color.Red, $"No nearby useful locations found. Please wait 10 seconds.");
                    await Task.Delay(10000);
                    Execute();
                }
            }
            catch (TaskCanceledException)
            {
                ColoredConsoleWrite(Color.Red, "Task Canceled Exception - Restarting");
                if (!_stopping) Execute();
            }
            catch (UriFormatException)
            {
                ColoredConsoleWrite(Color.Red, "System URI Format Exception - Restarting");
                if (!_stopping) Execute();
            }
            catch (ArgumentOutOfRangeException)
            {
                ColoredConsoleWrite(Color.Red, "ArgumentOutOfRangeException - Restarting");
                if (!_stopping) Execute();
            }
            catch (ArgumentNullException)
            {
                ColoredConsoleWrite(Color.Red, "Argument Null Refference - Restarting");
                if (!_stopping) Execute();
            }
            catch (NullReferenceException)
            {
                ColoredConsoleWrite(Color.Red, "Null Refference - Restarting");
                if (!_stopping) Execute();
            }
            catch (Exception ex)
            {
                ColoredConsoleWrite(Color.Red, ex.ToString());
                if (!_stopping) Execute();
            }
            //THIS COMMENT IS ONLY HERE COZ STRUCTURE IS REALLY WEIRD
            //This section is only hit when it doesnt recursify
            ConsoleClear();
            _pokestopsOverlay.Routes.Clear();
            _pokestopsOverlay.Markers.Clear();
            ColoredConsoleWrite(Color.Red, $"Bot successfully stopped.");
            startStopBotToolStripMenuItem.Text = "▶ Start Bot";
            _stopping = false;
            _botStarted = false;
            _pokeStops = null;
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

            foreach (var pokemon in pokemons)
            {
                if (_forceUnbanning || _stopping)
                    break;

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
                var pokemonCP = encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp;
                var pokemonIV = Math.Round(Perfect(encounterPokemonResponse?.WildPokemon?.PokemonData));
                CatchPokemonResponse caughtPokemonResponse;
                ColoredConsoleWrite(Color.Green, $"Encounter a {pokemonName} with {pokemonCP} CP and {pokemonIV}% IV");
                do
                {
                    if (ClientSettings.RazzBerryMode == "cp")
                        if (pokemonCP > ClientSettings.RazzBerrySetting)
                            await UseRazzBerry(client, pokemon.EncounterId, pokemon.SpawnPointId);
                    if (ClientSettings.RazzBerryMode == "probability")
                        if (encounterPokemonResponse.CaptureProbability.CaptureProbability_.First() <
                            ClientSettings.RazzBerrySetting)
                            await UseRazzBerry(client, pokemon.EncounterId, pokemon.SpawnPointId);
                    caughtPokemonResponse =
                        await
                            CatchPokemon(pokemon.EncounterId, pokemon.SpawnPointId, pokemon.Latitude, pokemon.Longitude,
                                ItemId.ItemPokeBall, pokemonCP);
                    ; //note: reverted from settings because this should not be part of settings but part of logic
                } while (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed ||
                         caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchEscape);

                if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchSuccess)
                {
                    var c = Color.LimeGreen;
                    if (pokemonIV >= 80)
                    {
                        c = Color.Yellow;
                    }
                    ColoredConsoleWrite(c, $"We caught a {pokemonName} with {pokemonCP} CP and {pokemonIV}% IV");
                    //foreach (int xp in caughtPokemonResponse.CaptureAward.Xp)
                    //    TotalExperience += xp;
                    _totalExperience += caughtPokemonResponse.CaptureAward.Xp.Sum();
                    _totalPokemon += 1;
                }
                else
                    ColoredConsoleWrite(Color.Red, $"{pokemonName} with {pokemonCP} CP and {pokemonIV}% IV got away..");


                // I believe a switch is more efficient and easier to read.
                switch (ClientSettings.TransferType)
                {
                    case "Leave Strongest":
                        await TransferAllButStrongestUnwantedPokemon(client);
                        break;
                    case "All":
                        await TransferAllGivenPokemons(client, pokemons2);
                        break;
                    case "Duplicate":
                        await TransferDuplicatePokemon(client);
                        break;
                    case "IV Duplicate":
                        await TransferDuplicateIVPokemon(client);
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
                await Task.Delay(3000);
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
                    PokeStopOutput.Write($", Items: {GetFriendlyItemsString(fortSearch.ItemsAwarded)} ");
                ColoredConsoleWrite(Color.Cyan, PokeStopOutput.ToString());

                if (fortSearch.ExperienceAwarded != 0)
                    _totalExperience += fortSearch.ExperienceAwarded;

                pokeStop.CooldownCompleteTimestampMs = DateTime.UtcNow.ToUnixTime() + 300000;

                if (ClientSettings.CatchPokemon)
                    await ExecuteCatchAllNearbyPokemons(client);
            }
            _farmingStops = false;
            if (!_forceUnbanning && !_stopping)
            {
                RecycleItems(client);
                await ExecuteFarmingPokestopsAndPokemons(client);
            }
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
            return (poke.IndividualAttack + poke.IndividualDefense + poke.IndividualStamina) / (3.0f * 15.0f) * 100.0f;
        }

        private async Task TransferAllGivenPokemons(Client client, IEnumerable<PokemonData> unwantedPokemons,
            float keepPerfectPokemonLimit = 80.0f)
        {
            foreach (var pokemon in unwantedPokemons)
            {
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

        private async Task TransferAllWeakPokemon(Client client, int cpThreshold)
        {
            //ColoredConsoleWrite(ConsoleColor.White, $"Firing up the meat grinder");

            PokemonId[] doNotTransfer =
            {
                // DO NOT EMPTY THIS ARRAY
                //PokemonId.Pidgey,
                //PokemonId.Rattata,
                //PokemonId.Weedle,
                //PokemonId.Zubat,
                //PokemonId.Caterpie,
                //PokemonId.Pidgeotto,
                //PokemonId.NidoranFemale,
                //PokemonId.Paras,
                //PokemonId.Venonat,
                //PokemonId.Psyduck,
                //PokemonId.Poliwag,
                //PokemonId.Slowpoke,
                //PokemonId.Drowzee,
                //PokemonId.Gastly,
                //PokemonId.Goldeen,
                //PokemonId.Staryu,
                PokemonId.Magikarp,
                PokemonId.Eevee //,
                //PokemonId.Dratini
            };

            var inventory = await client.Inventory.GetInventory();
            var pokemons = inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.PokemonData)
                .Where(p => p != null && p?.PokemonId > 0)
                .ToArray();

            //foreach (var unwantedPokemonType in unwantedPokemonTypes)
            {
                List<PokemonData> pokemonToDiscard;
                if (doNotTransfer.Count() != 0)
                    pokemonToDiscard =
                        pokemons.Where(p => !doNotTransfer.Contains(p.PokemonId) && p.Cp < cpThreshold)
                            .OrderByDescending(p => p.Cp)
                            .ToList();
                else
                    pokemonToDiscard = pokemons.Where(p => p.Cp < cpThreshold).OrderByDescending(p => p.Cp).ToList();


                //var unwantedPokemon = pokemonOfDesiredType.Skip(1) // keep the strongest one for potential battle-evolving
                //                                          .ToList();
                ColoredConsoleWrite(Color.Gray, $"Grinding {pokemonToDiscard.Count} pokemon below {cpThreshold} CP.");
                await TransferAllGivenPokemons(client, pokemonToDiscard);
            }

            ColoredConsoleWrite(Color.Gray, $"Finished grinding all the meat");
        }


        public async Task PrintLevel(Client client)
        {
            var inventory = await client.Inventory.GetInventory();
            var stats = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PlayerStats).ToArray();
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
            PrintLevel(client);
        }

        // Pulled from NecronomiconCoding
        public static string _getSessionRuntimeInTimeFormat()
        {
            return (DateTime.Now - InitSessionDateTime).ToString(@"dd\.hh\:mm\:ss");
        }

        public async Task updateUserStatusBar(Client client)
        {
            var inventory = await client.Inventory.GetInventory();
            var stats = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PlayerStats).ToArray();
            var profile = await client.Player.GetPlayer();
            short hoursLeft = 0;
            short minutesLeft = 0;
            var secondsLeft = 0;
            double xpSec = 0;
            foreach (var v in stats)
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
            updateUserStatusBar(client);
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
                Task.Run(() =>
                {
                    try
                    {
                        //ColoredConsoleWrite(ConsoleColor.White, "Coded by Ferox - edited by NecronomiconCoding");
                        CheckVersion();
                        Execute();
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

        private void showAllToolStripMenuItem3_Click(object sender, EventArgs e)
        {
        }

        private void statsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // todo: add player stats later
        }

        private async void useLuckyEggToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_client != null)
            {
                try
                {
                    var myItems = await GetItems(_client);
                    var LuckyEggs = myItems.Where(i => i.ItemId == ItemId.ItemLuckyEgg);
                    var LuckyEgg = LuckyEggs.FirstOrDefault();
                    if (LuckyEgg != null)
                    {
                        var useItemXpBoostRequest = await _client.Inventory.UseItemXpBoost();
                        ColoredConsoleWrite(Color.Green, $"Using a Lucky Egg, we have {LuckyEgg.Count} left.");
                        ColoredConsoleWrite(Color.Yellow, $"Lucky Egg Valid until: {DateTime.Now.AddMinutes(30)}");

                        var stripItem = sender as ToolStripMenuItem;
                        stripItem.Enabled = false;
                        await Task.Delay(30000);
                        stripItem.Enabled = true;
                    }
                    else
                    {
                        ColoredConsoleWrite(Color.Red, $"You don't have any Lucky Egg to use.");
                    }
                }
                catch (Exception ex)
                {
                    ColoredConsoleWrite(Color.Red, $"Unhandled exception in using lucky egg: {ex}");
                }
            }
            else
            {
                ColoredConsoleWrite(Color.Red, "Please start the bot before trying to use a lucky egg.");
            }
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

        private void showAllToolStripMenuItem2_Click(object sender, EventArgs e)
        {
        }

        private void todoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }

        private void pokemonToolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }

        private void pokeToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void objectListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        #region POKEMON LIST

        private IEnumerable<PokemonFamilyId> families;

        private void InitializePokemonForm()
        {
            objectListView1.ButtonClick += PokemonListButton_Click;

            pkmnName.ImageGetter = delegate (object rowObject)
            {
                var pokemon = (PokemonData)rowObject;

                var key = pokemon.PokemonId.ToString();
                if (!objectListView1.SmallImageList.Images.ContainsKey(key))
                {
                    var img = GetPokemonImage((int)pokemon.PokemonId);
                    objectListView1.SmallImageList.Images.Add(key, img);
                }
                return key;
            };

            objectListView1.CellToolTipShowing += delegate (object sender, ToolTipShowingEventArgs args)
            {
                var pokemon = (PokemonData)args.Model;

                var family = families
                    .Where(i => (int)i <= (int)pokemon.PokemonId)
                    .First();

                args.Text = $"You have {GetCandies(pokemon.PokemonId).Result} {(PokemonId)family} Candy";
            };
        }

        private Image GetPokemonImage(int pokemonId)
        {
            return (Image)Properties.Resources.ResourceManager.GetObject("Pokemon_" + pokemonId);
        }

        public async Task<int> GetCandies(PokemonId poke)
        {
            var inventory = await _client.Inventory.GetInventory();
            var templates = await _client.Download.GetItemTemplates();
            var pokemontemplate =
                templates.ItemTemplates.Select(i => i?.PokemonSettings).Where(i => i?.PokemonId == poke);
            return
                inventory.InventoryDelta.InventoryItems.Select(i => i?.InventoryItemData?.Candy)
                    .Where(i => i?.FamilyId == pokemontemplate.SingleOrDefault().FamilyId)
                    .SingleOrDefault()
                    .Candy_;
        }

        private async void ReloadPokemonList()
        {
            if (!_botStarted || _stopping) return;
            button1.Enabled = false;
            objectListView1.Enabled = false;

            _client2 = new Client(ClientSettings, new ApiFailureStrategy());
            try
            {
                await _client2.Login.DoLogin();
                var inventory = await _client2.Inventory.GetInventory();
                var pokemons =
                    inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonData)
                        .Where(p => p != null && p?.PokemonId > 0)
                        .OrderByDescending(key => key.Cp);
                families = inventory.InventoryDelta.InventoryItems
                    .Select(i => i.InventoryItemData.Candy.FamilyId)
                    .Where(p => p > 0)
                    .OrderByDescending(p => p);

                var prevTopItem = objectListView1.TopItemIndex;
                objectListView1.SetObjects(pokemons);
                objectListView1.TopItemIndex = prevTopItem;
            }
            catch (Exception ex)
            {
                ColoredConsoleWrite(Color.Red, ex.ToString());
                _client2 = null;
            }

            button1.Enabled = true;
            objectListView1.Enabled = true;
        }

        private void PokemonListButton_Click(object sender, CellClickEventArgs e)
        {
            try
            {
                var pokemon = (PokemonData)e.Model;
                if (e.ColumnIndex == 6)
                {
                    TransferPokemon(pokemon);
                }
                else if (e.ColumnIndex == 7)
                {
                    PowerUpPokemon(pokemon);
                }
                else if (e.ColumnIndex == 8)
                {
                    EvolvePokemon(pokemon);
                }
            }
            catch (Exception ex)
            {
                ColoredConsoleWrite(Color.Red, ex.ToString());
                _client2 = null;
                ReloadPokemonList();
            }
        }

        private async void TransferPokemon(PokemonData pokemon)
        {
            if (
                MessageBox.Show($"Are you sure you want to transfer {pokemon.PokemonId} with {pokemon.Cp} CP?",
                    "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var transferPokemonResponse = await _client2.Inventory.TransferPokemon(pokemon.Id);

                if (transferPokemonResponse.Result == ReleasePokemonResponse.Types.Result.Success)
                {
                    ColoredConsoleWrite(Color.Magenta,
                        $"{pokemon.PokemonId} was transferred. {transferPokemonResponse.CandyAwarded} candy awarded");
                    ReloadPokemonList();
                }
                else
                {
                    ColoredConsoleWrite(Color.Magenta, $"{pokemon.PokemonId} could not be transferred");
                }
            }
        }

        private async void PowerUpPokemon(PokemonData pokemon)
        {
            var evolvePokemonResponse = await _client2.Inventory.UpgradePokemon(pokemon.Id);

            if (evolvePokemonResponse.Result == UpgradePokemonResponse.Types.Result.Success)
            {
                ColoredConsoleWrite(Color.Magenta, $"{pokemon.PokemonId} successfully upgraded.");
                ReloadPokemonList();
            }
            else
            {
                ColoredConsoleWrite(Color.Magenta, $"{pokemon.PokemonId} could not be upgraded");
            }
        }

        private async void EvolvePokemon(PokemonData pokemon)
        {
            var evolvePokemonResponse = await _client2.Inventory.EvolvePokemon(pokemon.Id);

            if (evolvePokemonResponse.Result == EvolvePokemonResponse.Types.Result.Success)
            {
                ColoredConsoleWrite(Color.Magenta,
                    $"{pokemon.PokemonId} successfully evolved into {evolvePokemonResponse.EvolvedPokemonData.PokemonId}\n{evolvePokemonResponse.ExperienceAwarded} experience awarded\n{evolvePokemonResponse.CandyAwarded} candy awarded");
                ReloadPokemonList();
            }
            else
            {
                ColoredConsoleWrite(Color.Magenta, $"{pokemon.PokemonId} could not be evolved");
            }
        }

        public async Task<IEnumerable<ItemData>> GetItems(Client client)
        {
            var inventory = await client.Inventory.GetInventory();
            return inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.Item)
                .Where(p => p != null);
        }

        public async Task<IEnumerable<ItemData>> GetItemsToRecycle(ISettings _settings, Client client)
        {
            var settings = (Settings)_settings;
            var myItems = await GetItems(client);

            return myItems
                .Where(x => settings.ItemRecycleFilter.Any(f => f.Key == x.ItemId && x.Count > f.Value))
                .Select(
                    x =>
                        new ItemData
                        {
                            ItemId = x.ItemId,
                            Count = x.Count - settings.ItemRecycleFilter.Single(f => f.Key == x.ItemId).Value,
                            Unseen = x.Unseen
                        });
        }

        public async Task RecycleItems(Client client)
        {
            var items = await GetItemsToRecycle(client.Settings, client);

            foreach (var item in items)
            {
                var transfer = await client.Inventory.RecycleItem(item.ItemId, item.Count);
                ColoredConsoleWrite(Color.DarkCyan, $"Recycled {item.Count}x {item.ItemId.ToString().Substring(4)}");
                await Task.Delay(500);
            }
            await Task.Delay(ClientSettings.RecycleItemsInterval * 1000);
            RecycleItems(client);
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

        private void button1_Click(object sender, EventArgs e)
        {
            ReloadPokemonList();
        }

        #endregion
    }
}