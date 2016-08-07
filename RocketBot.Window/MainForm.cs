using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.Helpers;

namespace PokemonGo.RocketAPI.Window
{
    public partial class MainForm : Form
    {
        public static MainForm Instance;
        public static SynchronizationContext synchronizationContext;

        public static ISettings ClientSettings;
        private static int Currentlevel = -1;
        private static int TotalExperience;
        private static int TotalPokemon;
        private static bool bot_started;
        private static bool Stopping;
        private static bool ForceUnbanning;
        private static bool FarmingStops;
        private static bool FarmingPokemons;
        private static readonly DateTime TimeStarted = DateTime.Now;
        public static DateTime InitSessionDateTime = DateTime.Now;
        private readonly GMapOverlay playerOverlay = new GMapOverlay("players");
        private readonly GMapOverlay pokemonsOverlay = new GMapOverlay("pokemons");
        private readonly GMapOverlay pokestopsOverlay = new GMapOverlay("pokestops");

        private readonly GMapOverlay searchAreaOverlay = new GMapOverlay("areas");

        private Client client;
        private Client client2;


        private IEnumerable<PokemonFamily> families;
        private LocationManager locationManager;

        private GMarkerGoogle playerMarker;

        private IEnumerable<FortData> pokeStops;
        private IEnumerable<WildPokemon> wildPokemons;

        public MainForm()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
            ClientSettings = Settings.Instance;
            Client.OnConsoleWrite += Client_OnConsoleWrite;
            Client.OnStopBot += stopBot;
            Instance = this;

            Text += " v" + Assembly.GetExecutingAssembly().GetName().Version;
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

            gMapControl1.Overlays.Add(searchAreaOverlay);
            gMapControl1.Overlays.Add(pokestopsOverlay);
            gMapControl1.Overlays.Add(pokemonsOverlay);
            gMapControl1.Overlays.Add(playerOverlay);

            playerMarker =
                new GMarkerGoogle(new PointLatLng(ClientSettings.DefaultLatitude, ClientSettings.DefaultLongitude),
                    GMarkerGoogleType.orange_small);
            playerOverlay.Markers.Add(playerMarker);

            InitializeMap();
            InitializePokemonForm();

            if (ClientSettings.TransferType == "Duplicate")
            {
                Settings.Instance.SetSetting("CP Duplicate", "TransferType");
                Settings.Instance.Reload();
            }
        }

        public void startBot()
        {
            bot_started = true;
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

        public void stopBot()
        {
            if (!ForceUnbanning)
            {
                Stopping = true;
                ColoredConsoleWrite(Color.Red, $"Stopping the bot.. Waiting for the last action to be complete.");
            }
            else
            {
                ColoredConsoleWrite(Color.Red, $"An action is in play, please wait until it's done.");
            }
        }

        public void Restart()
        {
            InitializeMap();
            InitializePokemonForm();
        }

        private void InitializeMap()
        {
            playerMarker.Position = new PointLatLng(ClientSettings.DefaultLatitude, ClientSettings.DefaultLongitude);
            searchAreaOverlay.Polygons.Clear();
            S2GMapDrawer.DrawS2Cells(
                S2Helper.GetNearbyCellIds(ClientSettings.DefaultLongitude, ClientSettings.DefaultLatitude),
                searchAreaOverlay);
        }

        public static void ResetMap()
        {
            Instance.gMapControl1.Position = new PointLatLng(ClientSettings.DefaultLatitude,
                ClientSettings.DefaultLongitude);
            Instance.playerMarker.Position = new PointLatLng(ClientSettings.DefaultLatitude,
                ClientSettings.DefaultLongitude);
            Instance.searchAreaOverlay.Polygons.Clear();
            S2GMapDrawer.DrawS2Cells(
                S2Helper.GetNearbyCellIds(ClientSettings.DefaultLongitude, ClientSettings.DefaultLatitude),
                Instance.searchAreaOverlay);
        }

        public static double GetRuntime()
        {
            return (DateTime.Now - TimeStarted).TotalSeconds/3600;
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
                ColoredConsoleWrite(Color.Green, "Your version is " + Assembly.GetExecutingAssembly().GetName().Version);
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
            var inventory = await client.GetInventory();
            var pokemons =
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Pokemon)
                    .Where(p => p != null && p?.PokemonId > 0);

            await EvolveAllGivenPokemons(client, pokemons);
        }

        private async Task EvolveAllGivenPokemons(Client client, IEnumerable<PokemonData> pokemonToEvolve)
        {
            foreach (var pokemon in pokemonToEvolve)
            {
                var countOfEvolvedUnits = 0;
                var xpCount = 0;

                EvolvePokemonOut evolvePokemonOutProto;
                do
                {
                    evolvePokemonOutProto = await client.EvolvePokemon(pokemon.Id);
                    //todo: someone check whether this still works

                    if (evolvePokemonOutProto.Result == 1)
                    {
                        countOfEvolvedUnits++;
                        xpCount += evolvePokemonOutProto.ExpAwarded;
                        TotalExperience += evolvePokemonOutProto.ExpAwarded;
                    }
                } while (evolvePokemonOutProto.Result == 1);

                if (countOfEvolvedUnits > 0)
                    ColoredConsoleWrite(Color.Cyan,
                        $"Evolved {countOfEvolvedUnits} pieces of {pokemon.PokemonId} for {xpCount}xp");

                await Task.Delay(3000);
            }
        }

        private async void Execute()
        {
            client = new Client(ClientSettings);
            locationManager = new LocationManager(client, ClientSettings.TravelSpeed);
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

                await client.Login();
                await client.SetServer();
                var profile = await client.GetProfile();
                var settings = await client.GetSettings();
                var mapObjects = await client.GetMapObjects();
                var inventory = await client.GetInventory();
                var pokemons =
                    inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Pokemon)
                        .Where(p => p != null && p?.PokemonId > 0);

                updateUserStatusBar(client);

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
                ColoredConsoleWrite(Color.DarkGray, "Name: " + profile.Profile.Username);
                ColoredConsoleWrite(Color.DarkGray, "Team: " + profile.Profile.Team);
                if (profile.Profile.Currency.ToArray()[0].Amount > 0)
                    // If player has any pokecoins it will show how many they have.
                    ColoredConsoleWrite(Color.DarkGray, "Pokecoins: " + profile.Profile.Currency.ToArray()[0].Amount);
                ColoredConsoleWrite(Color.DarkGray, "Stardust: " + profile.Profile.Currency.ToArray()[1].Amount + "\n");
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
                        await TransferAllButStrongestUnwantedPokemon(client);
                        break;
                    case "All":
                        await TransferAllGivenPokemons(client, pokemons);
                        break;
                    case "CP Duplicate":
                        await TransferDuplicatePokemon(client);
                        break;
                    case "IV Duplicate":
                        await TransferDuplicateIVPokemon(client);
                        break;
                    case "CP/IV Duplicate":
                        await TransferDuplicateCPIVPokemon(client);
                        break;
                    case "CP":
                        await TransferAllWeakPokemon(client, ClientSettings.TransferCPThreshold);
                        break;
                    case "IV":
                        await TransferAllGivenPokemons(client, pokemons, ClientSettings.TransferIVThreshold);
                        break;
                    default:
                        ColoredConsoleWrite(Color.DarkGray, "Transfering pokemon disabled");
                        break;
                }


                if (ClientSettings.EvolveAllGivenPokemons)
                    await EvolveAllGivenPokemons(client, pokemons);
                if (ClientSettings.Recycler)
                    client.RecycleItems(client);

                await Task.Delay(5000);
                PrintLevel(client);
                await ExecuteFarmingPokestopsAndPokemons(client);

                while (ForceUnbanning)
                    await Task.Delay(25);

                // await ForceUnban(client);
                if (!Stopping)
                {
                    ColoredConsoleWrite(Color.Red, $"No nearby useful locations found. Please wait 10 seconds.");
                    await Task.Delay(10000);
                    Execute();
                }
                else
                {
                    confirmBotStopped();
                }
            }
            catch (TaskCanceledException)
            {
                ColoredConsoleWrite(Color.Red, "Task Canceled Exception - Restarting");
            }
            catch (UriFormatException)
            {
                ColoredConsoleWrite(Color.Red, "System URI Format Exception - Restarting");
            }
            catch (ArgumentOutOfRangeException)
            {
                ColoredConsoleWrite(Color.Red, "ArgumentOutOfRangeException - Restarting");
            }
            catch (ArgumentNullException)
            {
                ColoredConsoleWrite(Color.Red, "Argument Null Refference - Restarting");
            }
            catch (NullReferenceException)
            {
                ColoredConsoleWrite(Color.Red, "Null Refference - Restarting");
            }
            catch (Exception ex)
            {
                ColoredConsoleWrite(Color.Red, ex.ToString());
            }
            finally
            {
                if (!Stopping)
                {
                    Execute();
                }
                else
                {
                    confirmBotStopped();
                }
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
                        }
                    }
                }
            }
            return "Error";
        }

        private async Task ExecuteCatchAllNearbyPokemons(Client client)
        {
            var mapObjects = await client.GetMapObjects();

            var pokemons = mapObjects.MapCells.SelectMany(i => i.CatchablePokemons);
            var inventory2 = await client.GetInventory();
            var pokemons2 = inventory2.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.Pokemon)
                .Where(p => p != null && p?.PokemonId > 0)
                .ToArray();

            foreach (var pokemon in pokemons)
            {
                if (ForceUnbanning || Stopping)
                    break;

                FarmingPokemons = true;

                await locationManager.update(pokemon.Latitude, pokemon.Longitude);

                string pokemonName;
                if (ClientSettings.Language == "german")
                {
                    var name_english = Convert.ToString(pokemon.PokemonId);
                    var request =
                        (HttpWebRequest)
                            WebRequest.Create("http://boosting-service.de/pokemon/index.php?pokeName=" + name_english);
                    var response = (HttpWebResponse) request.GetResponse();
                    pokemonName = new StreamReader(response.GetResponseStream()).ReadToEnd();
                }
                else
                    pokemonName = Convert.ToString(pokemon.PokemonId);

                await client.UpdatePlayerLocation(pokemon.Latitude, pokemon.Longitude);
                UpdatePlayerLocation(pokemon.Latitude, pokemon.Longitude);
                UpdateMap();
                var encounterPokemonResponse = await client.EncounterPokemon(pokemon.EncounterId, pokemon.SpawnpointId);
                var pokemonCP = encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp;
                var pokemonIV = Math.Round(Perfect(encounterPokemonResponse?.WildPokemon?.PokemonData));
                CatchPokemonResponse caughtPokemonResponse;
                ColoredConsoleWrite(Color.Green, $"Encounter a {pokemonName} with {pokemonCP} CP and {pokemonIV}% IV");
                do
                {
                    if (ClientSettings.RazzBerryMode == "cp")
                        if (pokemonCP > ClientSettings.RazzBerrySetting)
                            await client.UseRazzBerry(client, pokemon.EncounterId, pokemon.SpawnpointId);
                    if (ClientSettings.RazzBerryMode == "probability")
                        if (encounterPokemonResponse.CaptureProbability.CaptureProbability_.First() <
                            ClientSettings.RazzBerrySetting)
                            await client.UseRazzBerry(client, pokemon.EncounterId, pokemon.SpawnpointId);
                    caughtPokemonResponse =
                        await
                            client.CatchPokemon(pokemon.EncounterId, pokemon.SpawnpointId, pokemon.Latitude,
                                pokemon.Longitude, MiscEnums.Item.ITEM_POKE_BALL, pokemonCP);
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
                    foreach (var xp in caughtPokemonResponse.Scores.Xp)
                        TotalExperience += xp;
                    TotalPokemon += 1;
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
                    case "CP Duplicate":
                        await TransferDuplicatePokemon(client);
                        break;
                    case "IV Duplicate":
                        await TransferDuplicateIVPokemon(client);
                        break;
                    case "CP/IV Duplicate":
                        await TransferDuplicateCPIVPokemon(client);
                        break;
                    case "CP":
                        await TransferAllWeakPokemon(client, ClientSettings.TransferCPThreshold);
                        break;
                    case "IV":
                        await TransferAllGivenPokemons(client, pokemons2, ClientSettings.TransferIVThreshold);
                        break;
                    default:
                        ColoredConsoleWrite(Color.DarkGray, "Transfering pokemon disabled");
                        break;
                }

                FarmingPokemons = false;
                await Task.Delay(3000);
            }
            pokemons = null;
        }

        private void UpdatePlayerLocation(double latitude, double longitude)
        {
            synchronizationContext.Post(o =>
            {
                playerMarker.Position = (PointLatLng) o;

                searchAreaOverlay.Polygons.Clear();
            }, new PointLatLng(latitude, longitude));

            ColoredConsoleWrite(Color.Gray, $"Moving player location to Lat: {latitude}, Lng: {longitude}");
        }

        private void UpdateMap()
        {
            synchronizationContext.Post(o =>
            {
                pokestopsOverlay.Markers.Clear();
                var routePoint = new List<PointLatLng>();
                foreach (var pokeStop in pokeStops)
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
                    pokestopsOverlay.Markers.Add(pokestopMarker);

                    routePoint.Add(pokeStopLoc);
                }
                pokestopsOverlay.Routes.Clear();
                pokestopsOverlay.Routes.Add(new GMapRoute(routePoint, "Walking Path"));


                pokemonsOverlay.Markers.Clear();
                if (wildPokemons != null)
                {
                    foreach (var pokemon in wildPokemons)
                    {
                        var pokemonMarker = new GMarkerGoogle(new PointLatLng(pokemon.Latitude, pokemon.Longitude),
                            GMarkerGoogleType.red_small);
                        pokemonsOverlay.Markers.Add(pokemonMarker);
                    }
                }

                searchAreaOverlay.Polygons.Clear();
                S2GMapDrawer.DrawS2Cells(
                    S2Helper.GetNearbyCellIds(ClientSettings.DefaultLongitude, ClientSettings.DefaultLatitude),
                    searchAreaOverlay);
            }, null);
        }

        private async Task ExecuteFarmingPokestopsAndPokemons(Client client)
        {
            var mapObjects = await client.GetMapObjects();

            var rawPokeStops =
                mapObjects.MapCells.SelectMany(i => i.Forts)
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
            pokeStops = rawPokeStops;
            UpdateMap();
            ColoredConsoleWrite(Color.Cyan, $"Finding fastest route through all PokeStops..");
            var startingLatLong = new LatLong(ClientSettings.DefaultLatitude, ClientSettings.DefaultLongitude);
            pokeStops = RouteOptimizer.Optimize(rawPokeStops, startingLatLong, pokestopsOverlay);
            wildPokemons = mapObjects.MapCells.SelectMany(i => i.WildPokemons);
            if (!ForceUnbanning && !Stopping)
                ColoredConsoleWrite(Color.Cyan, $"Visiting {pokeStops.Count()} PokeStops");

            UpdateMap();
            foreach (var pokeStop in pokeStops)
            {
                if (ForceUnbanning || Stopping)
                    break;

                FarmingStops = true;
                await locationManager.update(pokeStop.Latitude, pokeStop.Longitude);
                UpdatePlayerLocation(pokeStop.Latitude, pokeStop.Longitude);
                UpdateMap();

                var fortInfo = await client.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                var fortSearch = await client.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
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
                    TotalExperience += fortSearch.ExperienceAwarded;

                pokeStop.CooldownCompleteTimestampMs = DateTime.UtcNow.ToUnixTime() + 300000;

                if (ClientSettings.CatchPokemon)
                    await ExecuteCatchAllNearbyPokemons(client);
            }
            FarmingStops = false;
            if (!ForceUnbanning && !Stopping)
            {
                client.RecycleItems(client);
                await ExecuteFarmingPokestopsAndPokemons(client);
            }
        }

        private async Task ForceUnban(Client client)
        {
            if (!ForceUnbanning && !Stopping)
            {
                ColoredConsoleWrite(Color.LightGreen, "Waiting for last farming action to be complete...");
                ForceUnbanning = true;

                while (FarmingStops || FarmingPokemons)
                {
                    await Task.Delay(25);
                }

                ColoredConsoleWrite(Color.LightGreen, "Starting force unban...");

                pokestopsOverlay.Routes.Clear();
                pokestopsOverlay.Markers.Clear();
                var done = false;
                foreach (var pokeStop in pokeStops)
                {
                    if (pokeStop.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime())
                    {
                        await locationManager.update(pokeStop.Latitude, pokeStop.Longitude);
                        UpdatePlayerLocation(pokeStop.Latitude, pokeStop.Longitude);
                        UpdateMap();

                        var fortInfo = await client.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                        if (fortInfo.Name != string.Empty)
                        {
                            ColoredConsoleWrite(Color.LightGreen,
                                "Chosen PokeStop " + fortInfo.Name + " for force unban");
                            for (var i = 1; i <= 50; i++)
                            {
                                var fortSearch =
                                    await client.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
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
                ForceUnbanning = false;
            }
            else
            {
                ColoredConsoleWrite(Color.Red, "A action is in play... Please wait.");
            }
        }

        private string GetFriendlyItemsString(IEnumerable<FortSearchResponse.Types.ItemAward> items)
        {
            var enumerable = items as IList<FortSearchResponse.Types.ItemAward> ?? items.ToList();

            if (!enumerable.Any())
                return string.Empty;

            return enumerable.GroupBy(i => i.ItemId)
                .Select(kvp => new {ItemName = kvp.Key.ToString().Substring(4), Amount = kvp.Sum(x => x.ItemCount)})
                .Select(y => $"{y.Amount}x {y.ItemName}")
                .Aggregate((a, b) => $"{a}, {b}");
        }


        private async Task TransferAllButStrongestUnwantedPokemon(Client client)
        {
            var unwantedPokemonTypes = new List<PokemonId>();
            for (var i = 1; i <= 151; i++)
            {
                unwantedPokemonTypes.Add((PokemonId) i);
            }

            var inventory = await client.GetInventory();
            var pokemons = inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.Pokemon)
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
            return (poke.IndividualAttack + poke.IndividualDefense + poke.IndividualStamina)/(3.0f*15.0f)*100.0f;
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
                    var transferPokemonResponse = await client.TransferPokemon(pokemon.Id);

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
                        var response = (HttpWebResponse) request.GetResponse();
                        pokemonName = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    }
                    else
                        pokemonName = Convert.ToString(pokemon.PokemonId);
                    if (transferPokemonResponse.Status == 1)
                    {
                        ColoredConsoleWrite(Color.Magenta, $"Transferred {pokemonName} with {pokemon.Cp} CP");
                    }
                    else
                    {
                        var status = transferPokemonResponse.Status;

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
            var inventory = await client.GetInventory();
            var allpokemons =
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Pokemon)
                    .Where(p => p != null && p?.PokemonId > 0);

            var dupes = allpokemons.OrderBy(x => x.Cp).Select((x, i) => new {index = i, value = x})
                .GroupBy(x => x.value.PokemonId)
                .Where(x => x.Skip(1).Any());

            for (var i = 0; i < dupes.Count(); i++)
            {
                for (var j = 0; j < dupes.ElementAt(i).Count() - 1; j++)
                {
                    var dubpokemon = dupes.ElementAt(i).ElementAt(j).value;
                    if (dubpokemon.Favorite == 0)
                    {
                        var transfer = await client.TransferPokemon(dubpokemon.Id);
                        string pokemonName;
                        if (ClientSettings.Language == "german")
                        {
                            var name_english = Convert.ToString(dubpokemon.PokemonId);
                            var request =
                                (HttpWebRequest)
                                    WebRequest.Create("http://boosting-service.de/pokemon/index.php?pokeName=" +
                                                      name_english);
                            var response = (HttpWebResponse) request.GetResponse();
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
            var inventory = await client.GetInventory();
            var allpokemons =
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Pokemon)
                    .Where(p => p != null && p?.PokemonId > 0);

            var dupes = allpokemons.OrderBy(x => Perfect(x)).Select((x, i) => new {index = i, value = x})
                .GroupBy(x => x.value.PokemonId)
                .Where(x => x.Skip(1).Any());

            for (var i = 0; i < dupes.Count(); i++)
            {
                for (var j = 0; j < dupes.ElementAt(i).Count() - 1; j++)
                {
                    var dubpokemon = dupes.ElementAt(i).ElementAt(j).value;
                    if (dubpokemon.Favorite == 0)
                    {
                        var transfer = await client.TransferPokemon(dubpokemon.Id);
                        string pokemonName;
                        if (ClientSettings.Language == "german")
                        {
                            var name_english = Convert.ToString(dubpokemon.PokemonId);
                            var request =
                                (HttpWebRequest)
                                    WebRequest.Create("http://boosting-service.de/pokemon/index.php?pokeName=" +
                                                      name_english);
                            var response = (HttpWebResponse) request.GetResponse();
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
            var inventory = await client.GetInventory();
            var allpokemons =
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Pokemon)
                    .Where(p => p != null && p?.PokemonId > 0);

            var dupes = allpokemons.OrderBy(x => Perfect(x)).Select((x, i) => new {index = i, value = x})
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
                        var transfer = await client.TransferPokemon(dubpokemon.Id);
                        string pokemonName;
                        if (ClientSettings.Language == "german")
                        {
                            var name_english = Convert.ToString(dubpokemon.PokemonId);
                            var request =
                                (HttpWebRequest)
                                    WebRequest.Create("http://boosting-service.de/pokemon/index.php?pokeName=" +
                                                      name_english);
                            var response = (HttpWebResponse) request.GetResponse();
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

            var inventory = await client.GetInventory();
            var pokemons = inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.Pokemon)
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
            var inventory = await client.GetInventory();
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
                        if (Currentlevel != v.Level)
                        {
                            Currentlevel = v.Level;
                            ColoredConsoleWrite(Color.Magenta,
                                $"Current Level: " + v.Level + ". XP needed for next Level: " +
                                (v.NextLevelXp - v.Experience));
                        }
                }
            if (ClientSettings.LevelOutput == "levelup")
                await Task.Delay(1000);
            else
                await Task.Delay(ClientSettings.LevelTimeInterval*1000);
            PrintLevel(client);
        }

        // Pulled from NecronomiconCoding
        public static string _getSessionRuntimeInTimeFormat()
        {
            return (DateTime.Now - InitSessionDateTime).ToString(@"dd\.hh\:mm\:ss");
        }

        public async Task updateUserStatusBar(Client client)
        {
            var inventory = await client.GetInventory();
            var stats = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PlayerStats).ToArray();
            var profile = await client.GetProfile();
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
                    xpSec = Math.Round(TotalExperience/GetRuntime())/60/60;
                    //Calculating the seconds left to level up
                    if (xpSec != 0)
                        secondsLeft = Convert.ToInt32(expNextLvl/xpSec);
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
                            profile.Profile.Username +
                            " | Level: {0:0} - ({2:0} / {3:0}) | Runtime {1} | Stardust: {4:0}", v.Level,
                            _getSessionRuntimeInTimeFormat(), v.Experience - v.PrevLevelXp - XpDiff,
                            v.NextLevelXp - v.PrevLevelXp - XpDiff, profile.Profile.Currency.ToArray()[1].Amount) +
                        " | XP/Hour: " + Math.Round(TotalExperience/GetRuntime()) + " | Pokemon/Hour: " +
                        Math.Round(TotalPokemon/GetRuntime()) + " | NextLevel in: " + hoursLeft + ":" + minutesLeft +
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
            if (!bot_started)
            {
                startBot();
            }
            else
            {
                stopBot();
            }
        }

        public void confirmBotStopped()
        {
            ConsoleClear();
            pokestopsOverlay.Routes.Clear();
            pokestopsOverlay.Markers.Clear();
            ColoredConsoleWrite(Color.Red, $"Bot successfully stopped.");
            startStopBotToolStripMenuItem.Text = "▶ Start Bot";
            Stopping = false;
            bot_started = false;
            pokeStops = null;
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
            if (client != null)
            {
                try
                {
                    var myItems = await client.GetItems(client);
                    var LuckyEggs = myItems.Where(i => (ItemId) i.Id == ItemId.ItemLuckyEgg);
                    var LuckyEgg = LuckyEggs.FirstOrDefault();
                    if (LuckyEgg != null)
                    {
                        var useItemXpBoostRequest = await client.UseItemXpBoost(ItemId.ItemLuckyEgg);
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
            if (client != null && pokeStops != null)
            {
                if (ForceUnbanning)
                {
                    ColoredConsoleWrite(Color.Red, "A force unban attempt is in action... Please wait.");
                }
                else
                {
                    await ForceUnban(client);
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

        private void InitializePokemonForm()
        {
            olvPokemonList.ButtonClick += PokemonListButton_Click;

            pkmnName.ImageGetter = delegate(object rowObject)
            {
                var pokemon = (rowObject as PokemonObject).PokemonData;

                var key = pokemon.PokemonId.ToString();
                if (!olvPokemonList.SmallImageList.Images.ContainsKey(key))
                {
                    var img = GetPokemonImage((int) pokemon.PokemonId);
                    olvPokemonList.SmallImageList.Images.Add(key, img);
                }
                return key;
            };

            olvPokemonList.FormatRow += delegate(object sender, FormatRowEventArgs e)
            {
                var pok = (PokemonObject) e.Model;
                if (
                    olvPokemonList.Objects.Cast<PokemonObject>()
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

            cmsPokemonList.Opening += delegate(object sender, CancelEventArgs e)
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

                var item = new ToolStripMenuItem();
                var separator = new ToolStripSeparator();
                item.Text = "Transfer " + count + " pokemon";
                item.Click += delegate { TransferPokemon(pokemons); };
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
                    /*
                    cmsPokemonList.Items.Add(separator);

                    item = new ToolStripMenuItem();
                    item.Text = "Rename";
                    item.Enabled = false;
                    item.Click += delegate (object obj, EventArgs args) {
                        
                    };
                    cmsPokemonList.Items.Add(item);*/
                }
            };
        }

        private Image GetPokemonImage(int pokemonId)
        {
            return (Image) Properties.Resources.ResourceManager.GetObject("Pokemon_" + pokemonId);
        }

        private async void ReloadPokemonList()
        {
            btnRefreshPokemonList.Enabled = false;
            olvPokemonList.Enabled = false;

            client2 = new Client(ClientSettings);
            try
            {
                await client2.Login();
                await client2.SetServer();
                PokemonObject.Initilize(client2);

                var profile = await client2.GetProfile();
                var inventory = await client2.GetInventory();
                var settings = await client2.GetSettings();

                var pokemons =
                    inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Pokemon)
                        .Where(p => p != null && p?.PokemonId > 0)
                        .OrderByDescending(key => key.Cp)
                        .OrderBy(key => key.PokemonId);
                families =
                    inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonFamily)
                        .Where(p => p != null && (int) p?.FamilyId > 0)
                        .OrderByDescending(p => (int) p.FamilyId);

                var pokemonObjects = new List<PokemonObject>();
                foreach (var pokemon in pokemons)
                {
                    var pokemonObject = new PokemonObject(pokemon);
                    var family = families.Where(i => (int) i.FamilyId <= (int) pokemon.PokemonId).First();
                    pokemonObject.Candy = family.Candy;
                    pokemonObjects.Add(pokemonObject);
                }

                var prevTopItem = olvPokemonList.TopItemIndex;
                olvPokemonList.SetObjects(pokemonObjects);
                olvPokemonList.TopItemIndex = prevTopItem;

                var pokemoncount =
                    inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Pokemon)
                        .Where(p => p != null && p?.PokemonId > 0)
                        .Count();
                var eggcount =
                    inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Pokemon)
                        .Where(p => p != null && p?.IsEgg == true)
                        .Count();
                lblPokemonList.Text = pokemoncount + eggcount + " / " + profile.Profile.PokeStorage + " (" +
                                      pokemoncount + " pokemon, " + eggcount + " eggs)";
            }
            catch (Exception ex)
            {
                ColoredConsoleWrite(Color.Red, ex.ToString());
                client2 = null;
            }

            btnRefreshPokemonList.Enabled = true;
            olvPokemonList.Enabled = true;
        }

        private void PokemonListButton_Click(object sender, CellClickEventArgs e)
        {
            try
            {
                var pokemon = (e.Model as PokemonObject).PokemonData;
                var cName = olvPokemonList.Columns[e.ColumnIndex].Text;
                if (cName.Equals("Transfer"))
                {
                    TransferPokemon(new List<PokemonData> {pokemon});
                }
                else if (cName.Equals("PowerUp"))
                {
                    PowerUpPokemon(new List<PokemonData> {pokemon});
                }
                else if (cName.Equals("Evolve"))
                {
                    EvolvePokemon(new List<PokemonData> {pokemon});
                }
            }
            catch (Exception ex)
            {
                ColoredConsoleWrite(Color.Red, ex.ToString());
                client2 = null;
                ReloadPokemonList();
            }
        }

        private async void TransferPokemon(IEnumerable<PokemonData> pokemons)
        {
            foreach (var pokemon in pokemons)
            {
                var transferPokemonResponse = await client2.TransferPokemon(pokemon.Id);
                if (transferPokemonResponse.Status == 1)
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
            foreach (var pokemon in pokemons)
            {
                var evolvePokemonResponse = await client2.PowerUp(pokemon.Id);
                if (evolvePokemonResponse.Result == 1)
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
            foreach (var pokemon in pokemons)
            {
                var evolvePokemonResponse = await client2.EvolvePokemon(pokemon.Id);
                if (evolvePokemonResponse.Result == 1)
                {
                    ColoredConsoleWrite(Color.Magenta,
                        $"{pokemon.PokemonId} successfully evolved into {evolvePokemonResponse.EvolvedPokemon.PokemonType}\n{evolvePokemonResponse.ExpAwarded} experience awarded\n{evolvePokemonResponse.CandyAwarded} candy awarded");
                }
                else
                {
                    ColoredConsoleWrite(Color.Magenta, $"{pokemon.PokemonId} could not be evolved");
                }
            }
            ReloadPokemonList();
        }

        private void btnRefreshPokemonList_Click(object sender, EventArgs e)
        {
            ReloadPokemonList();
        }
    }
}