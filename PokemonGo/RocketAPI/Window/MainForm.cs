using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using AllEnum;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.GeneratedCode;

namespace PokemonGo.RocketAPI.Window
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            ClientSettings = Settings.Instance;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
           
        }

        public static ISettings ClientSettings;
        private static int Currentlevel = -1;
        private static int TotalExperience = 0;
        private static int TotalPokemon = 0;
        private static DateTime TimeStarted = DateTime.Now;
        public static DateTime InitSessionDateTime = DateTime.Now;

        public static double GetRuntime()
        {
            return ((DateTime.Now - TimeStarted).TotalSeconds) / 3600;
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
                if (gitVersion <= Assembly.GetExecutingAssembly().GetName().Version)
                {
                    ColoredConsoleWrite(Color.Green, Language.GetPhrases()["have_latest_ver"].Replace("[version]", Convert.ToString(Assembly.GetExecutingAssembly().GetName().Version)));
                    return;
                }

                ColoredConsoleWrite(Color.Red, Language.GetPhrases()["new_ver_avaliabe"].Replace("[version]", Convert.ToString(gitVersion)));
                ColoredConsoleWrite(Color.Red, Language.GetPhrases()["get_new_ver_link"]);
            }
            catch (Exception)
            {
                ColoredConsoleWrite(Color.Red, Language.GetPhrases()["unable_to_check_update"]);
            }
        }

        private static string DownloadServerVersion()
        {
            using (var wC = new WebClient())
                return
                    wC.DownloadString(
                        "https://raw.githubusercontent.com/DetectiveSquirrel/Pokemon-Go-Rocket-API/master/PokemonGo/RocketAPI/Window/Properties/AssemblyInfo.cs");
        }

        public void ColoredConsoleWrite(Color color, string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Color, string>(ColoredConsoleWrite), color, text);
                return;
            }
            string textToAppend = "[" + DateTime.Now.ToString("HH:mm:ss tt") + "] " + text + "\r\n";
            logTextBox.SelectionColor = color;
            logTextBox.AppendText(textToAppend);

            object syncRoot = new object();
            lock (syncRoot) // Added locking to prevent text file trying to be accessed by two things at the same time
            {
                File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Logs.txt", "[" + DateTime.Now.ToString("HH:mm:ss tt") + "] " + text + "\n");
            }
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

        private async Task EvolveAllGivenPokemons(Client client, IEnumerable<PokemonData> pokemonToEvolve)
        {
            foreach (var pokemon in pokemonToEvolve)
            {
                /*
                enum Holoholo.Rpc.Types.EvolvePokemonOutProto.Result {
	                UNSET = 0;
	                SUCCESS = 1;
	                FAILED_POKEMON_MISSING = 2;
	                FAILED_INSUFFICIENT_RESOURCES = 3;
	                FAILED_POKEMON_CANNOT_EVOLVE = 4;
	                FAILED_POKEMON_IS_DEPLOYED = 5;
                }
                }*/

                var countOfEvolvedUnits = 0;
                var xpCount = 0;

                EvolvePokemonOut evolvePokemonOutProto;
                string pokemonname = Language.GetPokemons()[Convert.ToString(pokemon.PokemonId)];

                do
                {
                    evolvePokemonOutProto = await client.EvolvePokemon(pokemon.Id);
                    //todo: someone check whether this still works
                    if (evolvePokemonOutProto.Result == 1)
                    {
                        ColoredConsoleWrite(Color.Cyan,
                            Language.GetPhrases()["evolved_one_pokemon"].Replace("[pokemon]", pokemonname).Replace("[xp]", Convert.ToString(evolvePokemonOutProto.ExpAwarded)));

                        countOfEvolvedUnits++;
                        xpCount += evolvePokemonOutProto.ExpAwarded;
                    }
                    else
                    {
                        var result = evolvePokemonOutProto.Result;
                        /*
                        ColoredConsoleWrite(Color.White, $"Failed to evolve {pokemon.PokemonId}. " +
                                                 $"EvolvePokemonOutProto.Result was {result}");

                        ColoredConsoleWrite(Color.White, $"Due to above error, stopping evolving {pokemon.PokemonId}");
                        */
                    }
                } while (evolvePokemonOutProto.Result == 1);
                if (countOfEvolvedUnits > 0)
                    ColoredConsoleWrite(Color.Cyan, Language.GetPhrases()["evolved_multi_pokemons"].Replace("[number]", Convert.ToString(countOfEvolvedUnits)).Replace("[pokemon]", pokemonname).Replace("[xp]", Convert.ToString(evolvePokemonOutProto.ExpAwarded)));

                await Task.Delay(3000);
            }
        }

        private async void Execute()
        {
            var client = new Client(ClientSettings);
            try
            {
                switch (ClientSettings.AuthType)
                {
                    case AuthType.Ptc:
                        ColoredConsoleWrite(Color.Green, "Attempting to log into Pokemon Trainers Club..");
                        await client.DoPtcLogin(ClientSettings.PtcUsername, ClientSettings.PtcPassword);
                        break;
                    case AuthType.Google:
                        ColoredConsoleWrite(Color.Green, "Attempting to log into Google..");
                        if (ClientSettings.GoogleRefreshToken == "")
                            ColoredConsoleWrite(Color.Green, "Now opening www.Google.com/device and copying the 8 digit code to your clipboard");

                        await client.DoGoogleLogin();
                        break;
                }

                await client.SetServer();
                var profile = await client.GetProfile();
                var settings = await client.GetSettings();
                var mapObjects = await client.GetMapObjects();
                var inventory = await client.GetInventory();
                var pokemons =
                    inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Pokemon)
                        .Where(p => p != null && p?.PokemonId > 0);

                ConsoleLevelTitle(profile.Profile.Username, client);

                // Write the players ingame details
                ColoredConsoleWrite(Color.Yellow, "----------------------------");
                if (ClientSettings.AuthType == AuthType.Ptc)
                {
                    ColoredConsoleWrite(Color.Cyan, Language.GetPhrases()["account"].Replace("[username]", ClientSettings.PtcUsername));
                    ColoredConsoleWrite(Color.Cyan, Language.GetPhrases()["password"].Replace("[password]", ClientSettings.PtcPassword) + "\n");
                }
                ColoredConsoleWrite(Color.DarkGray, Language.GetPhrases()["username"].Replace("[username]", profile.Profile.Username));
                ColoredConsoleWrite(Color.DarkGray, Language.GetPhrases()["team"].Replace("[team]", Convert.ToString(profile.Profile.Team)));
                if (profile.Profile.Currency.ToArray()[0].Amount > 0) // If player has any pokecoins it will show how many they have.
                    ColoredConsoleWrite(Color.DarkGray, "Pokecoins: " + profile.Profile.Currency.ToArray()[0].Amount);
                ColoredConsoleWrite(Color.DarkGray, Language.GetPhrases()["stardust"].Replace("[stardust]", Convert.ToString(profile.Profile.Currency.ToArray()[1].Amount)) + "\n");
                ColoredConsoleWrite(Color.DarkGray, Language.GetPhrases()["latitude"].Replace("[latitude]", Convert.ToString(ClientSettings.DefaultLatitude)));
                ColoredConsoleWrite(Color.DarkGray, Language.GetPhrases()["longtitude"].Replace("[longtitude]", Convert.ToString(ClientSettings.DefaultLongitude)));
                try
                {
                    ColoredConsoleWrite(Color.DarkGray, Language.GetPhrases()["area"].Replace("[area]", CallAPI("place", ClientSettings.DefaultLatitude, ClientSettings.DefaultLongitude)));
                    ColoredConsoleWrite(Color.DarkGray, Language.GetPhrases()["country"].Replace("[country]", CallAPI("country", ClientSettings.DefaultLatitude, ClientSettings.DefaultLongitude)));
                }
                catch (Exception)
                {
                    ColoredConsoleWrite(Color.DarkGray, "Unable to get Country/Place");
                }

                ColoredConsoleWrite(Color.Yellow, "----------------------------");

                // I believe a switch is more efficient and easier to read.
                switch (ClientSettings.TransferType)
                {
                    case "leaveStrongest":
                        await TransferAllButStrongestUnwantedPokemon(client);
                        break;
                    case "all":
                        await TransferAllGivenPokemons(client, pokemons);
                        break;
                    case "duplicate":
                        await TransferDuplicatePokemon(client);
                        break;
                    case "cp":
                        await TransferAllWeakPokemon(client, ClientSettings.TransferCPThreshold);
                        break;
                    default:
                        ColoredConsoleWrite(Color.DarkGray, Language.GetPhrases()["transfering_disabled"]);
                        break;
                }

                if (ClientSettings.EvolveAllGivenPokemons)
                    await EvolveAllGivenPokemons(client, pokemons);
                if (ClientSettings.Recycler)
                    client.RecycleItems(client);

                await Task.Delay(5000);
                PrintLevel(client);
                await ExecuteFarmingPokestopsAndPokemons(client);
                ColoredConsoleWrite(Color.Red, Language.GetPhrases()["no_nearby_loc_found"]);
                await Task.Delay(10000);
                CheckVersion();
                Execute();
            }
            catch (TaskCanceledException) { ColoredConsoleWrite(Color.Red, Language.GetPhrases()["task_canceled_ex"]); Execute(); }
            catch (UriFormatException) { ColoredConsoleWrite(Color.Red, Language.GetPhrases()["sys_uri_format_ex"]); Execute(); }
            catch (ArgumentOutOfRangeException) { ColoredConsoleWrite(Color.Red, Language.GetPhrases()["arg_out_of_range_ex"]); Execute(); }
            catch (ArgumentNullException) { ColoredConsoleWrite(Color.Red, Language.GetPhrases()["arg_null_ref"]); Execute(); }
            catch (NullReferenceException) { ColoredConsoleWrite(Color.Red, Language.GetPhrases()["null_ref"]); Execute(); }
            catch (Exception ex) { ColoredConsoleWrite(Color.Red, ex.ToString()); Execute(); }
        }

        private static string CallAPI(string elem, double lat, double lon)
        {
            using (XmlReader reader = XmlReader.Create(@"http://api.geonames.org/findNearby?lat=" + lat + "&lng=" + lon + "&username=demo"))
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
                                if (reader.Name == "toponymName")
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
            var mapObjects = await client.GetMapObjects();

            var pokemons = mapObjects.MapCells.SelectMany(i => i.CatchablePokemons);

            var inventory2 = await client.GetInventory();
            var pokemons2 = inventory2.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.Pokemon)
                .Where(p => p != null && p?.PokemonId > 0)
                .ToArray();

            foreach (var pokemon in pokemons)
            {
                var update = await client.UpdatePlayerLocation(pokemon.Latitude, pokemon.Longitude);
                var encounterPokemonResponse = await client.EncounterPokemon(pokemon.EncounterId, pokemon.SpawnpointId);
                var pokemonCP = encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp;
                CatchPokemonResponse caughtPokemonResponse;
                do
                {
                    if (ClientSettings.RazzBerryMode == "cp")
                        if (pokemonCP > ClientSettings.RazzBerrySetting)
                            await client.UseRazzBerry(client, pokemon.EncounterId, pokemon.SpawnpointId);
                    if (ClientSettings.RazzBerryMode == "probability")
                        if (encounterPokemonResponse.CaptureProbability.CaptureProbability_.First() < ClientSettings.RazzBerrySetting)
                            await client.UseRazzBerry(client, pokemon.EncounterId, pokemon.SpawnpointId);
                    caughtPokemonResponse = await client.CatchPokemon(pokemon.EncounterId, pokemon.SpawnpointId, pokemon.Latitude, pokemon.Longitude, MiscEnums.Item.ITEM_POKE_BALL, pokemonCP); ; //note: reverted from settings because this should not be part of settings but part of logic
                } while (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed || caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchEscape);

                string pokemonName = Language.GetPokemons()[Convert.ToString(pokemon.PokemonId)];

                if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchSuccess)
                {
                    ColoredConsoleWrite(Color.Green, $"We caught a {pokemonName} with {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} CP");
                    foreach (int xp in caughtPokemonResponse.Scores.Xp)
                        TotalExperience += xp;
                    TotalPokemon += 1;
                }
                else
                    ColoredConsoleWrite(Color.Red, $"{pokemonName} with {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} CP got away..");

                if (ClientSettings.TransferType == "leaveStrongest")
                    await TransferAllButStrongestUnwantedPokemon(client);
                else if (ClientSettings.TransferType == "all")
                    await TransferAllGivenPokemons(client, pokemons2);
                else if (ClientSettings.TransferType == "duplicate")
                    await TransferDuplicatePokemon(client);
                else if (ClientSettings.TransferType == "cp")
                    await TransferAllWeakPokemon(client, ClientSettings.TransferCPThreshold);

                await Task.Delay(3000);
            }
        }

        private async Task ExecuteFarmingPokestopsAndPokemons(Client client)
        {
            var mapObjects = await client.GetMapObjects();

            var pokeStops = mapObjects.MapCells.SelectMany(i => i.Forts).Where(i => i.Type == FortType.Checkpoint && i.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime());

            foreach (var pokeStop in pokeStops)
            {
                var update = await client.UpdatePlayerLocation(pokeStop.Latitude, pokeStop.Longitude);
                var fortInfo = await client.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                var fortSearch = await client.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);

                StringWriter PokeStopOutput = new StringWriter();
                PokeStopOutput.Write($"");
                if (fortInfo.Name != string.Empty)
                    PokeStopOutput.Write(Language.GetPhrases()["pokestop"].Replace("[pokestop]", fortInfo.Name));
                if (fortSearch.ExperienceAwarded != 0)
                    PokeStopOutput.Write($", {Language.GetPhrases()["xp"].Replace("[xp]", Convert.ToString(fortSearch.ExperienceAwarded))}");
                if (fortSearch.GemsAwarded != 0)
                    PokeStopOutput.Write($", {Language.GetPhrases()["gem"].Replace("[gem]", Convert.ToString(fortSearch.GemsAwarded))}");
                if (fortSearch.PokemonDataEgg != null)
                    PokeStopOutput.Write($", {Language.GetPhrases()["egg"].Replace("[egg]", Convert.ToString(fortSearch.PokemonDataEgg))}");
                if (GetFriendlyItemsString(fortSearch.ItemsAwarded) != string.Empty)
                    PokeStopOutput.Write($", {Language.GetPhrases()["item"].Replace("[item]", GetFriendlyItemsString(fortSearch.ItemsAwarded))}");
                ColoredConsoleWrite(Color.Cyan, PokeStopOutput.ToString());

                if (fortSearch.ExperienceAwarded != 0)
                    TotalExperience += (fortSearch.ExperienceAwarded);
                await Task.Delay(15000);
                await ExecuteCatchAllNearbyPokemons(client);
            }
        }

        private string GetFriendlyItemsString(IEnumerable<FortSearchResponse.Types.ItemAward> items)
        {
            var enumerable = items as IList<FortSearchResponse.Types.ItemAward> ?? items.ToList();

            if (!enumerable.Any())
                return string.Empty;

            return enumerable.GroupBy(i => i.ItemId)
                    .Select(kvp => new { ItemName = kvp.Key.ToString().Substring(4), Amount = kvp.Sum(x => x.ItemCount) })
                    .Select(y => $"{y.Amount}x {y.ItemName}")
                    .Aggregate((a, b) => $"{a}, {b}");
        }


        private async Task TransferAllButStrongestUnwantedPokemon(Client client)
        {
            //ColoredConsoleWrite(Color.White, $"Firing up the meat grinder");

            var unwantedPokemonTypes = new[]
            {
                PokemonId.Pidgey,
                PokemonId.Rattata,
                PokemonId.Weedle,
                PokemonId.Zubat,
                PokemonId.Caterpie,
                PokemonId.Pidgeotto,
                PokemonId.NidoranFemale,
                PokemonId.Paras,
                PokemonId.Venonat,
                PokemonId.Psyduck,
                PokemonId.Poliwag,
                PokemonId.Slowpoke,
                PokemonId.Drowzee,
                PokemonId.Gastly,
                PokemonId.Goldeen,
                PokemonId.Staryu,
                PokemonId.Magikarp,
                PokemonId.Clefairy,
                PokemonId.Eevee,
                PokemonId.Tentacool,
                PokemonId.Dratini,
                PokemonId.Ekans,
                PokemonId.Jynx,
                PokemonId.Lickitung,
                PokemonId.Spearow,
                PokemonId.NidoranFemale,
                PokemonId.NidoranMale
            };

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

                //ColoredConsoleWrite(Color.White, $"Grinding {unwantedPokemon.Count} pokemons of type {unwantedPokemonType}");
                await TransferAllGivenPokemons(client, unwantedPokemon);
            }

            //ColoredConsoleWrite(Color.White, $"Finished grinding all the meat");
        }

        public static float Perfect(PokemonData poke)
        {
            return ((float)(poke.IndividualAttack + poke.IndividualDefense + poke.IndividualStamina) / (3.0f * 15.0f)) * 100.0f;
        }

        private async Task TransferAllGivenPokemons(Client client, IEnumerable<PokemonData> unwantedPokemons, float keepPerfectPokemonLimit = 80.0f)
        {
            foreach (var pokemon in unwantedPokemons)
            {
                if (Perfect(pokemon) >= keepPerfectPokemonLimit) continue;
                ColoredConsoleWrite(Color.White, Language.GetPhrases()["pokemon_iv_percent_less_than"].Replace("[pokemon]", Language.GetPokemons()[Convert.ToString(pokemon.PokemonId)]).Replace("[cp]", Convert.ToString(pokemon.Cp)).Replace("[percent]", Convert.ToString(keepPerfectPokemonLimit)));

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
                    string pokemonName = Language.GetPokemons()[Convert.ToString(pokemon.PokemonId)];

                    if (transferPokemonResponse.Status == 1)
                    {
                        ColoredConsoleWrite(Color.Magenta, Language.GetPhrases()["transferred_pokemon"].Replace("[pokemon]", Language.GetPokemons()[Convert.ToString(pokemon.PokemonId)]).Replace("[cp]", Convert.ToString(pokemon.Cp)));
                    }
                    else
                    {
                        var status = transferPokemonResponse.Status;

                        ColoredConsoleWrite(Color.Red, Language.GetPhrases()["transferred_pokemon_failed"].Replace("[pokemon]", Language.GetPokemons()[Convert.ToString(pokemon.PokemonId)]).Replace("[cp]", Convert.ToString(pokemon.Cp)) +
                                                 $"ReleasePokemonOutProto.Status was {status}");
                    }

                    await Task.Delay(3000);
                }
            }
        }

        private async Task TransferDuplicatePokemon(Client client)
        {

            //ColoredConsoleWrite(Color.White, $"Check for duplicates");
            var inventory = await client.GetInventory();
            var allpokemons =
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Pokemon)
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
                        var transfer = await client.TransferPokemon(dubpokemon.Id);

                        string pokemonName = Language.GetPokemons()[Convert.ToString(dubpokemon.PokemonId)];

                        ColoredConsoleWrite(Color.DarkGreen,
                            Language.GetPhrases()["transferred_low_pokemon"].Replace("[pokemon]", Language.GetPokemons()[Convert.ToString(dubpokemon.PokemonId)]).Replace("[cp]", Convert.ToString(dubpokemon.Cp)).Replace("[high_cp]", Convert.ToString(dupes.ElementAt(i).Last().value.Cp)));

                    }
                }
            }
        }

        private async Task TransferAllWeakPokemon(Client client, int cpThreshold)
        {
            //ColoredConsoleWrite(Color.White, $"Firing up the meat grinder");

            PokemonId[] doNotTransfer = new[] //these will not be transferred even when below the CP threshold
            { // DO NOT EMPTY THIS ARRAY
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
                PokemonId.Eevee//,
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
                    pokemonToDiscard = pokemons.Where(p => !doNotTransfer.Contains(p.PokemonId) && p.Cp < cpThreshold).OrderByDescending(p => p.Cp).ToList();
                else
                    pokemonToDiscard = pokemons.Where(p => p.Cp < cpThreshold).OrderByDescending(p => p.Cp).ToList();


                //var unwantedPokemon = pokemonOfDesiredType.Skip(1) // keep the strongest one for potential battle-evolving
                //                                          .ToList();
                ColoredConsoleWrite(Color.Gray, Language.GetPhrases()["grinding_pokemon"].Replace("[number]", Convert.ToString(pokemonToDiscard.Count)).Replace("[cp]", Convert.ToString(cpThreshold)));
                await TransferAllGivenPokemons(client, pokemonToDiscard);

            }

            ColoredConsoleWrite(Color.Gray, Language.GetPhrases()["finished_grinding"]);
        }

        public async Task PrintLevel(Client client)
        {
            var inventory = await client.GetInventory();
            var stats = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PlayerStats).ToArray();
            foreach (var v in stats)
                if (v != null)
                {
                    int XpDiff = GetXpDiff(client, v.Level);
                    if (ClientSettings.LevelOutput == "time")
                        ColoredConsoleWrite(Color.Yellow, Language.GetPhrases()["current_lv"] + v.Level + "(" + (v.Experience - v.PrevLevelXp - XpDiff) + " / " + (v.NextLevelXp - v.PrevLevelXp - XpDiff) + ")");
                    else if (ClientSettings.LevelOutput == "levelup")
                        if (Currentlevel != v.Level)
                        {
                            Currentlevel = v.Level;
                            ColoredConsoleWrite(Color.Magenta, Language.GetPhrases()["current_lv"] + v.Level + " " + Language.GetPhrases()["xp_needed_for_next_lv"] + (v.NextLevelXp - v.Experience));
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

        public async Task ConsoleLevelTitle(string Username, Client client)
        {
            var inventory = await client.GetInventory();
            var stats = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PlayerStats).ToArray();
            var profile = await client.GetProfile();
            foreach (var v in stats)
                if (v != null)
                {
                    int XpDiff = GetXpDiff(client, v.Level);
                    System.Console.Title = string.Format(Username + " | Level: {0:0} - ({2:0} / {3:0}) | Runtime {1} | Stardust: {4:0}", v.Level, _getSessionRuntimeInTimeFormat(), (v.Experience - v.PrevLevelXp - XpDiff), (v.NextLevelXp - v.PrevLevelXp - XpDiff), profile.Profile.Currency.ToArray()[1].Amount) + " | XP/Hour: " + Math.Round(TotalExperience / GetRuntime()) + " | Pokemon/Hour: " + Math.Round(TotalPokemon / GetRuntime());
                }
            await Task.Delay(1000);
            ConsoleLevelTitle(Username, client);
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
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.Show();
        }

        private void startBotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startBotToolStripMenuItem.Enabled = false;
            try
            {
                Language.LoadLanguageFile(ClientSettings.Language);
                ColoredConsoleWrite(Color.Gray, $"Loaded language file: {ClientSettings.Language}");
            }
            catch (Exception ex)
            {
                ColoredConsoleWrite(Color.Red, $"[{DateTime.Now.ToString("HH:mm:ss")}] Something's wrong when loading language file: \n{ex}");
                try
                {
                    ColoredConsoleWrite(Color.White, $"[{DateTime.Now.ToString("HH:mm:ss")}] Using default en_us instead.");
                    Language.LoadLanguageFile("en_us");
                }
                catch
                {
                    ColoredConsoleWrite(Color.Red, $"[{DateTime.Now.ToString("HH:mm:ss")}] Something's wrong when loading default language file again: \n{ex}");
                    ColoredConsoleWrite(Color.White, $"[{DateTime.Now.ToString("HH:mm:ss")}] Please check if your language files are valid. Press any key to exit the program...");
                    System.Console.ReadKey();
                    System.Environment.Exit(1);
                }

            }
            Task.Run(() =>
            {
                try
                {
                    //ColoredConsoleWrite(Color.White, "Coded by Ferox - edited by NecronomiconCoding");
                    CheckVersion();
                    Execute();
                }
                catch (PtcOfflineException)
                {
                    ColoredConsoleWrite(Color.Red, Language.GetPhrases()["ptc_server_down"]);
                }
                catch (Exception ex)
                {
                    ColoredConsoleWrite(Color.Red, Language.GetPhrases()["unhandled_ex"].Replace("[ex]", Convert.ToString(ex)));
                }
            });
        }

        private void showAllToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            var pForm = new PokemonForm();
            pForm.Show();
        }

        private void statsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // todo: add player stats later
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
