#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AllEnum;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.GeneratedCode;
using System.Net.Http;
using System.Text;
using Google.Protobuf;
using PokemonGo.RocketAPI.Helpers;

#endregion

namespace PokemonGo.RocketAPI.Console
{
    internal class Program
    {
        private static readonly ISettings ClientSettings = new Settings();
        

        public static void CheckVersion()
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
                    ColoredConsoleWrite(ConsoleColor.Yellow, "Awesome! You have already got the newest version! " + Assembly.GetExecutingAssembly().GetName().Version);
                    return;
                }
                ;

                ColoredConsoleWrite(ConsoleColor.White, "There is a new Version available: " + gitVersion + " downloading.. ");
                Thread.Sleep(1000);
                Process.Start("https://github.com/NecronomiconCoding/Pokemon-Go-Rocket-API");
            }
            catch (Exception)
            {
                ColoredConsoleWrite(ConsoleColor.White, "Unable to check for updates now...");
            }
        }

        private static string DownloadServerVersion()
        {
            using (var wC = new WebClient())
                return
                    wC.DownloadString(
                        "https://raw.githubusercontent.com/NecronomiconCoding/Pokemon-Go-Rocket-API/master/PokemonGo/RocketAPI/Console/Properties/AssemblyInfo.cs");
        }

        public static void ColoredConsoleWrite(ConsoleColor color, string text)
        {
            ConsoleColor originalColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color;
            System.Console.WriteLine(text);
            System.Console.ForegroundColor = originalColor;
        }

        private static async Task EvolveAllGivenPokemons(Client client, IEnumerable<PokemonData> pokemonToEvolve)
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
                do
                {
                    evolvePokemonOutProto = await client.EvolvePokemon(pokemon.Id);
                        //todo: someone check whether this still works

                    if (evolvePokemonOutProto.Result == 1)
                    {
                        ColoredConsoleWrite(ConsoleColor.White, 
                            $"[{DateTime.Now.ToString("HH:mm:ss")}] Evolved {pokemon.PokemonId} successfully for {evolvePokemonOutProto.ExpAwarded}xp");

                        countOfEvolvedUnits++;
                        xpCount += evolvePokemonOutProto.ExpAwarded;
                    }
                    else
                    {
                        var result = evolvePokemonOutProto.Result;
                        /*
                        ColoredConsoleWrite(ConsoleColor.White, $"Failed to evolve {pokemon.PokemonId}. " +
                                                 $"EvolvePokemonOutProto.Result was {result}");

                        ColoredConsoleWrite(ConsoleColor.White, $"Due to above error, stopping evolving {pokemon.PokemonId}");
                        */
                    }
                } while (evolvePokemonOutProto.Result == 1);
                if (countOfEvolvedUnits > 0)
                    ColoredConsoleWrite(ConsoleColor.White, 
                        $"[{DateTime.Now.ToString("HH:mm:ss")}] Evolved {countOfEvolvedUnits} pieces of {pokemon.PokemonId} for {xpCount}xp");

                await Task.Delay(3000);
            }
        }

        private static async void Execute()
        {
            var client = new Client(ClientSettings);

            if (ClientSettings.AuthType == AuthType.Ptc)
                await client.DoPtcLogin(ClientSettings.PtcUsername, ClientSettings.PtcPassword);
            else if (ClientSettings.AuthType == AuthType.Google)
                await client.DoGoogleLogin();

            await client.SetServer();
            var profile = await client.GetProfile();
            var settings = await client.GetSettings();
            var mapObjects = await client.GetMapObjects();
            var inventory = await client.GetInventory();
            var pokemons =
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Pokemon)
                    .Where(p => p != null && p?.PokemonId > 0);

            ColoredConsoleWrite(ConsoleColor.Yellow, "----------------------------");
            ColoredConsoleWrite(ConsoleColor.Cyan, "Account: " + ClientSettings.PtcUsername);
            ColoredConsoleWrite(ConsoleColor.Cyan, "Password: " + ClientSettings.PtcPassword + "\n");
            ColoredConsoleWrite(ConsoleColor.DarkGray, "Latitude: " + ClientSettings.DefaultLatitude);
            ColoredConsoleWrite(ConsoleColor.DarkGray, "Longitude: " + ClientSettings.DefaultLongitude);
            ColoredConsoleWrite(ConsoleColor.Yellow, "----------------------------");
            ColoredConsoleWrite(ConsoleColor.DarkGray, "Random Profile Data");
            ColoredConsoleWrite(ConsoleColor.DarkGray, profile.ToString());



            try
            {
                ColoredConsoleWrite(ConsoleColor.Cyan, "Farming Started");
                ColoredConsoleWrite(ConsoleColor.Yellow, "----------------------------");
                if (ClientSettings.TransferType == "leaveStrongest")
                    await TransferAllButStrongestUnwantedPokemon(client);
                else if (ClientSettings.TransferType == "all")
                    await TransferAllGivenPokemons(client, pokemons);
                else if (ClientSettings.TransferType == "duplicate")
                    await TransferDuplicatePokemon(client);
                else if (ClientSettings.TransferType == "cp")
                    await TransferAllWeakPokemon(client, ClientSettings.TransferCPThreshold);
                else
                    ColoredConsoleWrite(ConsoleColor.DarkGray, $"[{DateTime.Now.ToString("HH:mm:ss")}] Transfering pokemon disabled");
                if (ClientSettings.EvolveAllGivenPokemons)
                    await EvolveAllGivenPokemons(client, pokemons);

                await Task.Delay(5000);
                await ExecuteFarmingPokestopsAndPokemons(client);
                ColoredConsoleWrite(ConsoleColor.Red, $"[{DateTime.Now.ToString("HH:mm:ss")}] Unexpected stop? Restarting in 20 seconds.");
                await Task.Delay(20000);
                Execute();
            }
            catch (TaskCanceledException tce) { ColoredConsoleWrite(ConsoleColor.White, "Task Canceled Exception - Restarting"); Execute(); }
            catch (UriFormatException ufe) { ColoredConsoleWrite(ConsoleColor.White, "System URI Format Exception - Restarting"); Execute(); }
            catch (ArgumentOutOfRangeException aore) { ColoredConsoleWrite(ConsoleColor.White, "ArgumentOutOfRangeException - Restarting"); Execute(); }
            catch (NullReferenceException nre) { ColoredConsoleWrite(ConsoleColor.White, "Null Refference - Restarting"); Execute(); }
            //await ExecuteCatchAllNearbyPokemons(client);
        }

        private static async Task ExecuteCatchAllNearbyPokemons(Client client)
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
                CatchPokemonResponse caughtPokemonResponse;
                do
                {
                    caughtPokemonResponse =
                        await
                            client.CatchPokemon(pokemon.EncounterId, pokemon.SpawnpointId, pokemon.Latitude,
                                pokemon.Longitude, MiscEnums.Item.ITEM_POKE_BALL);
                    ; //note: reverted from settings because this should not be part of settings but part of logic
                } while (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed);
                ColoredConsoleWrite(ConsoleColor.Green, caughtPokemonResponse.Status ==
                                         CatchPokemonResponse.Types.CatchStatus.CatchSuccess
                    ? $"[{DateTime.Now.ToString("HH:mm:ss")}] We caught a {pokemon.PokemonId} with {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} CP"
                    : $"[{DateTime.Now.ToString("HH:mm:ss")}] {pokemon.PokemonId} with {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} CP got away..");


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

        private static async Task ExecuteFarmingPokestopsAndPokemons(Client client)
        {
            var mapObjects = await client.GetMapObjects();

            var pokeStops =
                mapObjects.MapCells.SelectMany(i => i.Forts)
                    .Where(
                        i =>
                            i.Type == FortType.Checkpoint &&
                            i.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime());

            foreach (var pokeStop in pokeStops)
            {
                var update = await client.UpdatePlayerLocation(pokeStop.Latitude, pokeStop.Longitude);
                var fortInfo = await client.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                var fortSearch = await client.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                
                ColoredConsoleWrite(ConsoleColor.Cyan, 
                    $"[{DateTime.Now.ToString("HH:mm:ss")}] PokeStop XP: {fortSearch.ExperienceAwarded}, Gems: {fortSearch.GemsAwarded}, Eggs: {fortSearch.PokemonDataEgg} Items: {GetFriendlyItemsString(fortSearch.ItemsAwarded)}");
                

                await Task.Delay(15000);
                await ExecuteCatchAllNearbyPokemons(client);
            }
        }

        private static string GetFriendlyItemsString(IEnumerable<FortSearchResponse.Types.ItemAward> items)
        {
            var enumerable = items as IList<FortSearchResponse.Types.ItemAward> ?? items.ToList();

            if (!enumerable.Any())
                return string.Empty;

            return
                enumerable.GroupBy(i => i.ItemId)
                    .Select(kvp => new {ItemName = kvp.Key.ToString(), Amount = kvp.Sum(x => x.ItemCount)})
                    .Select(y => $"{y.Amount} x {y.ItemName}")
                    .Aggregate((a, b) => $"{a}, {b}");
        }

        private static void Main(string[] args)
        {
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
                    ColoredConsoleWrite(ConsoleColor.Red, "PTC Servers are probably down OR your credentials are wrong. Try google");
                }
                catch (Exception ex)
                {
                    ColoredConsoleWrite(ConsoleColor.Red, $"[{DateTime.Now.ToString("HH:mm:ss")}] Unhandled exception: {ex}");
                }
            });
            System.Console.ReadLine();
        }

        private static async Task TransferAllButStrongestUnwantedPokemon(Client client)
        {
            //ColoredConsoleWrite(ConsoleColor.White, $"[{DateTime.Now.ToString("HH:mm:ss")}] Firing up the meat grinder");

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

                //ColoredConsoleWrite(ConsoleColor.White, $"[{DateTime.Now.ToString("HH:mm:ss")}] Grinding {unwantedPokemon.Count} pokemons of type {unwantedPokemonType}");
                await TransferAllGivenPokemons(client, unwantedPokemon);
            }

            //ColoredConsoleWrite(ConsoleColor.White, $"[{DateTime.Now.ToString("HH:mm:ss")}] Finished grinding all the meat");
        }

        private static async Task TransferAllGivenPokemons(Client client, IEnumerable<PokemonData> unwantedPokemons)
        {
            foreach (var pokemon in unwantedPokemons)
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

                if (transferPokemonResponse.Status == 1)
                {
                    ColoredConsoleWrite(ConsoleColor.Magenta, $"[{DateTime.Now.ToString("HH:mm:ss")}] Transferred {pokemon.PokemonId} with {pokemon.Cp} CP");
                }
                else
                {
                    var status = transferPokemonResponse.Status;

                    ColoredConsoleWrite(ConsoleColor.Red, $"[{DateTime.Now.ToString("HH:mm:ss")}] Somehow failed to transfer {pokemon.PokemonId} with {pokemon.Cp} CP. " +
                                             $"ReleasePokemonOutProto.Status was {status}");
                }

                await Task.Delay(3000);
            }
        }

        private static async Task TransferDuplicatePokemon(Client client)
        {
           
                //ColoredConsoleWrite(ConsoleColor.White, $"Check for duplicates");
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
                        var transfer = await client.TransferPokemon(dubpokemon.Id);
                        ColoredConsoleWrite(ConsoleColor.DarkGreen, 
                            $"[{DateTime.Now.ToString("HH:mm:ss")}] Transferred {dubpokemon.PokemonId} with {dubpokemon.Cp} CP (Highest is {dupes.ElementAt(i).Last().value.Cp})");
                    }
                }
        }

        private static async Task TransferAllWeakPokemon(Client client, int cpThreshold)
        {
            //ColoredConsoleWrite(ConsoleColor.White, $"[{DateTime.Now.ToString("HH:mm:ss")}] Firing up the meat grinder");

            var doNotTransfer = new[] //these will not be transferred even when below the CP threshold
            {
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
                var pokemonToDiscard = pokemons.Where(p => !doNotTransfer.Contains(p.PokemonId) && p.Cp < cpThreshold)
                                                   .OrderByDescending(p => p.Cp)
                                                   .ToList();

                //var unwantedPokemon = pokemonOfDesiredType.Skip(1) // keep the strongest one for potential battle-evolving
                //                                          .ToList();
                ColoredConsoleWrite(ConsoleColor.Gray, $"[{DateTime.Now.ToString("HH:mm:ss")}] Grinding {pokemonToDiscard.Count} pokemon below {cpThreshold} CP.");
                await TransferAllGivenPokemons(client, pokemonToDiscard);

            }

            ColoredConsoleWrite(ConsoleColor.Gray, $"[{DateTime.Now.ToString("HH:mm:ss")}] Finished grinding all the meat");
        }
    }
}