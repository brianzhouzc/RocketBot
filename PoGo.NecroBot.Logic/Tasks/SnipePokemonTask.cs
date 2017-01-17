#region using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using CloudFlareUtilities;
using GeoCoordinatePortable;
using Newtonsoft.Json;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.Model.Settings;
using PoGo.NecroBot.Logic.PoGoUtils;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Utils;
using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
using POGOProtos.Map.Pokemon;
using POGOProtos.Networking.Responses;
using Quobject.Collections.Immutable;
using Quobject.SocketIoClientDotNet.Client;
using Socket = Quobject.SocketIoClientDotNet.Client.Socket;
using PoGo.NecroBot.Logic.Exceptions;
using PokemonGo.RocketAPI.Exceptions;
using PoGo.NecroBot.Logic.Captcha;

#endregion

namespace PoGo.NecroBot.Logic.Tasks
{
    public class SniperInfo
    {
        public ulong EncounterId { get; set; }
        public DateTime ExpirationTimestamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public PokemonId Id { get; set; }
        public string SpawnPointId { get; set; }
        public PokemonMove Move1 { get; set; }
        public PokemonMove Move2 { get; set; }
        public double IV { get; set; }

        [JsonIgnore]
        public DateTime TimeStampAdded { get; set; } = DateTime.Now;
    }

    public class PokemonLocation
    {
        public PokemonLocation(double lat, double lon)
        {
            latitude = lat;
            longitude = lon;
        }

        public long Id { get; set; }
        public double expires { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int pokemon_id { get; set; }
        public PokemonId pokemon_name { get; set; }

        [JsonIgnore]
        public DateTime TimeStampAdded { get; set; } = DateTime.Now;

        public bool Equals(PokemonLocation obj)
        {
            return Math.Abs(latitude - obj.latitude) < 0.0001 && Math.Abs(longitude - obj.longitude) < 0.0001;
        }

        public override bool Equals(object obj) // contains calls this here
        {
            var p = obj as PokemonLocation;
            if (p == null) // no cast available
            {
                return false;
            }

            return Math.Abs(latitude - p.latitude) < 0.0001 && Math.Abs(longitude - p.longitude) < 0.0001;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return latitude.ToString("0.00000000000") + ", " + longitude.ToString("0.00000000000");
        }
    }

    public class PokemonLocationPokezz
    {
        public double time { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string iv { get; set; }

        public double _iv
        {
            get
            {
                try
                {
                    return Convert.ToDouble(iv, CultureInfo.InvariantCulture);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public PokemonId name { get; set; }
        public bool verified { get; set; }
    }

    public class PokemonLocationPokesnipers
    {
        public int id { get; set; }
        public double iv { get; set; }
        public PokemonId name { get; set; }
        public string until { get; set; }
        public string coords { get; set; }
    }

    public class PokemonLocationPokewatchers
    {
        public PokemonId pokemon { get; set; }
        public double timeadded { get; set; }
        public double timeend { get; set; }
        public string cords { get; set; }
    }

    public class ScanResult
    {
        public string Status { get; set; }
        public List<PokemonLocation> pokemons { get; set; }
    }

    public class ScanResultPokesnipers
    {
        public string Status { get; set; }

        [JsonProperty("results")]
        public List<PokemonLocationPokesnipers> pokemons { get; set; }
    }

    public class ScanResultPokewatchers
    {
        public string Status { get; set; }
        public List<PokemonLocationPokewatchers> pokemons { get; set; }
    }

    public static class SnipePokemonTask
    {
        public static List<PokemonLocation> LocsVisited = new List<PokemonLocation>();
        private static readonly List<SniperInfo> SnipeLocations = new List<SniperInfo>();
        private static DateTime _lastSnipe = DateTime.MinValue;

        public static Task AsyncStart(Session session, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => Start(session, cancellationToken), cancellationToken);
        }

        public static async Task<bool> CheckPokeballsToSnipe(int minPokeballs, ISession session,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

           // Refresh inventory so that the player stats are fresh
           //await session.Inventory.RefreshCachedInventory();

            var pokeBallsCount = await session.Inventory.GetItemAmountByType(ItemId.ItemPokeBall);
            pokeBallsCount += await session.Inventory.GetItemAmountByType(ItemId.ItemGreatBall);
            pokeBallsCount += await session.Inventory.GetItemAmountByType(ItemId.ItemUltraBall);
            pokeBallsCount += await session.Inventory.GetItemAmountByType(ItemId.ItemMasterBall);

            if (pokeBallsCount >= minPokeballs)
                return true;

            session.EventDispatcher.Send(new SnipeEvent
            {
                Message =
                    session.Translation.GetTranslation(TranslationString.NotEnoughPokeballsToSnipe, pokeBallsCount,
                        minPokeballs)
            });

            return false;
        }

        private static bool CheckSnipeConditions(ISession session)
        {
            if (!session.LogicSettings.UseSnipeLimit) return true;

            session.EventDispatcher.Send(new SnipeEvent
            {
                Message = session.Translation.GetTranslation(TranslationString.SniperCount, session.Stats.SnipeCount)
            });

            if (session.Stats.SnipeCount < session.LogicSettings.SnipeCountLimit)
                return true;

            if ((DateTime.Now - session.Stats.LastSnipeTime).TotalSeconds > session.LogicSettings.SnipeRestSeconds)
            {
                session.Stats.SnipeCount = 0;
            }
            else
            {
                session.EventDispatcher.Send(new SnipeEvent
                {
                    Message = session.Translation.GetTranslation(TranslationString.SnipeExceeds)
                });
                return false;
            }
            return true;
        }

        public static async Task Execute(ISession session, CancellationToken cancellationToken)
        {
            if (_lastSnipe.AddMilliseconds(session.LogicSettings.MinDelayBetweenSnipes) > DateTime.Now)
                return;

            LocsVisited.RemoveAll(q => DateTime.Now > q.TimeStampAdded.AddMinutes(15));
            SnipeLocations.RemoveAll(x => DateTime.Now > x.TimeStampAdded.AddMinutes(15));

            if (await CheckPokeballsToSnipe(session.LogicSettings.MinPokeballsToSnipe, session, cancellationToken))
            {
                if (session.LogicSettings.PokemonToSnipe != null)
                {
                    List<PokemonId> pokemonIds;
                    if (session.LogicSettings.SnipePokemonNotInPokedex)
                    {
                        var pokeDex = await session.Inventory.GetPokeDexItems();
                        var pokemonOnlyList = session.LogicSettings.PokemonToSnipe.Pokemon;
                        var capturedPokemon =
                            pokeDex.Where(i => i.InventoryItemData.PokedexEntry.TimesCaptured >= 1)
                                .Select(i => i.InventoryItemData.PokedexEntry.PokemonId);
                        var pokemonToCapture =
                            Enum.GetValues(typeof(PokemonId)).Cast<PokemonId>().Except(capturedPokemon);
                        pokemonIds = pokemonOnlyList.Union(pokemonToCapture).ToList();
                    }
                    else
                    {
                        pokemonIds = session.LogicSettings.PokemonToSnipe.Pokemon;
                    }

                    if (session.LogicSettings.UseSnipeLocationServer)
                    {
                        var locationsToSnipe = SnipeLocations?.Where(q =>
                            (!session.LogicSettings.UseTransferIvForSnipe ||
                             (q.IV == 0 && !session.LogicSettings.SnipeIgnoreUnknownIv) ||
                             (q.IV >= session.Inventory.GetPokemonTransferFilter(q.Id).KeepMinIvPercentage)) &&
                            !LocsVisited.Contains(new PokemonLocation(q.Latitude, q.Longitude))
                            && !(q.ExpirationTimestamp != default(DateTime) &&
                                 q.ExpirationTimestamp > new DateTime(2016) &&
                                 // make absolutely sure that the server sent a correct datetime
                                 q.ExpirationTimestamp < DateTime.Now) &&
                            (q.Id == PokemonId.Missingno || pokemonIds.Contains(q.Id))).ToList();

                        var _locationsToSnipe = locationsToSnipe.OrderBy(q => q.ExpirationTimestamp).ToList();
                        if (_locationsToSnipe.Any())
                        {
                            foreach (var location in _locationsToSnipe)
                            {
                                if (LocsVisited.Contains(new PokemonLocation(location.Latitude, location.Longitude)))
                                    continue;

                                session.EventDispatcher.Send(new SnipeScanEvent
                                {
                                    Bounds = new Location(location.Latitude, location.Longitude),
                                    PokemonId = location.Id,
                                    Source = session.LogicSettings.SnipeLocationServer,
                                    Iv = location.IV
                                });

                                if (
                                    !await
                                        CheckPokeballsToSnipe(session.LogicSettings.MinPokeballsWhileSnipe + 1, session,
                                            cancellationToken))
                                    return;
                                if (!CheckSnipeConditions(session)) return;
                                await
                                    Snipe(session, pokemonIds, location.Latitude, location.Longitude, cancellationToken);
                            }
                        }
                    }

                    if (session.LogicSettings.GetSniperInfoFromPokezz)
                    {
                        var locationsToSnipe = GetSniperInfoFrom_pokezz(session, pokemonIds);
                        if (locationsToSnipe != null && locationsToSnipe.Any())
                        {
                            foreach (var location in locationsToSnipe)
                            {
                                if (LocsVisited.Contains(new PokemonLocation(location.Latitude, location.Longitude)))
                                    continue;

                                if (location.ExpirationTimestamp.AddSeconds(5) < DateTime.Now)
                                    continue;

                                session.EventDispatcher.Send(new SnipeScanEvent
                                {
                                    Bounds = new Location(location.Latitude, location.Longitude),
                                    PokemonId = location.Id,
                                    Source = "Pokezz.com",
                                    Iv = location.IV
                                });

                                if (
                                    !await
                                        CheckPokeballsToSnipe(session.LogicSettings.MinPokeballsWhileSnipe + 1, session,
                                            cancellationToken))
                                    return;
                                if (!CheckSnipeConditions(session)) return;

                                await
                                    Snipe(session, pokemonIds, location.Latitude, location.Longitude, cancellationToken);
                            }
                        }
                    }

                    if (session.LogicSettings.GetSniperInfoFromPokeSnipers)
                    {
                        var locationsToSnipe = GetSniperInfoFrom_pokesnipers(session, pokemonIds);
                        if (locationsToSnipe != null && locationsToSnipe.Any())
                        {
                            foreach (var location in locationsToSnipe)
                            {
                                if (LocsVisited.Contains(new PokemonLocation(location.Latitude, location.Longitude)))
                                    continue;

                                if (location.ExpirationTimestamp.AddSeconds(5) < DateTime.Now)
                                    continue;

                                session.EventDispatcher.Send(new SnipeScanEvent
                                {
                                    Bounds = new Location(location.Latitude, location.Longitude),
                                    PokemonId = location.Id,
                                    Source = "PokeSnipers.com",
                                    Iv = location.IV
                                });

                                if (
                                    !await
                                        CheckPokeballsToSnipe(session.LogicSettings.MinPokeballsWhileSnipe + 1, session,
                                            cancellationToken))
                                    return;
                                if (!CheckSnipeConditions(session)) return;

                                await
                                    Snipe(session, pokemonIds, location.Latitude, location.Longitude, cancellationToken);
                            }
                        }
                    }

                    if (session.LogicSettings.GetSniperInfoFromPokeWatchers)
                    {
                        var locationsToSnipe = GetSniperInfoFrom_pokewatchers(session, pokemonIds);
                        if (locationsToSnipe != null && locationsToSnipe.Any())
                        {
                            foreach (var location in locationsToSnipe)
                            {
                                if (LocsVisited.Contains(new PokemonLocation(location.Latitude, location.Longitude)))
                                    continue;

                                if (location.ExpirationTimestamp.AddSeconds(5) < DateTime.Now)
                                    continue;

                                session.EventDispatcher.Send(new SnipeScanEvent
                                {
                                    Bounds = new Location(location.Latitude, location.Longitude),
                                    PokemonId = location.Id,
                                    Source = "PokeWatchers.com",
                                    Iv = location.IV
                                });

                                if (
                                    !await
                                        CheckPokeballsToSnipe(session.LogicSettings.MinPokeballsWhileSnipe + 1, session,
                                            cancellationToken))
                                    return;
                                if (!CheckSnipeConditions(session)) return;

                                await
                                    Snipe(session, pokemonIds, location.Latitude, location.Longitude, cancellationToken);
                            }
                        }
                    }

                    if (session.LogicSettings.GetSniperInfoFromSkiplagged)
                    {
                        foreach (var location in session.LogicSettings.PokemonToSnipe.Locations)
                        {
                            session.EventDispatcher.Send(new SnipeScanEvent
                            {
                                Bounds = location,
                                PokemonId = PokemonId.Missingno,
                                Source = "Skiplagged.com"
                            });

                            var scanResult = SnipeScanForPokemon(session, location);

                            var locationsToSnipe = new List<PokemonLocation>();
                            if (scanResult.pokemons != null)
                            {
                                var filteredPokemon = scanResult.pokemons.Where(q => pokemonIds.Contains(q.pokemon_name));
                                var notVisitedPokemon = filteredPokemon.Where(q => !LocsVisited.Contains(q));

                                var pokemonLocations = notVisitedPokemon as IList<PokemonLocation> ??
                                                       notVisitedPokemon.ToList();
                                if (pokemonLocations.Any())
                                    locationsToSnipe.AddRange(pokemonLocations);
                            }

                            var _locationsToSnipe = locationsToSnipe.OrderBy(q => q.expires).ToList();

                            if (_locationsToSnipe.Any())
                            {
                                foreach (var pokemonLocation in _locationsToSnipe)
                                {
                                    if (
                                        !(pokemonLocation.expires - 5 >
                                          (DateTime.Now.ToUniversalTime() -
                                           new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds))
                                        continue;

                                    if (
                                        !await
                                            CheckPokeballsToSnipe(session.LogicSettings.MinPokeballsWhileSnipe + 1,
                                                session, cancellationToken))
                                        return;

                                    if (!CheckSnipeConditions(session))
                                        return;

                                    await
                                        Snipe(session, pokemonIds, pokemonLocation.latitude, pokemonLocation.longitude,
                                            cancellationToken);
                                }
                            }
                            else if (session.LogicSettings.UseSnipeLocationServer &&
                                     !string.IsNullOrEmpty(scanResult.Status) && scanResult.Status.Contains("fail"))
                                session.EventDispatcher.Send(new SnipeEvent
                                {
                                    Message =
                                        session.Translation.GetTranslation(TranslationString.SnipeServerOffline)
                                });
                            else
                                session.EventDispatcher.Send(new SnipeEvent
                                {
                                    Message = session.Translation.GetTranslation(TranslationString.NoPokemonToSnipe)
                                });
                        }
                    }
                }
            }
        }

        public static async Task<bool> Snipe(ISession session, IEnumerable<PokemonId> pokemonIds, double latitude,
            double longitude, CancellationToken cancellationToken)
        {
            //if (LocsVisited.Contains(new PokemonLocation(latitude, longitude)))
            //    return;

            var currentLatitude = session.Client.CurrentLatitude;
            var currentLongitude = session.Client.CurrentLongitude;
            var catchedPokemon = false;

            session.EventDispatcher.Send(new SnipeModeEvent {Active = true});

            List<MapPokemon> catchablePokemon;
            int retry = 5;

            bool isCaptchaShow = false;

            try
            {
                do
                {
                    retry--;
                    await
                        LocationUtils.UpdatePlayerLocationWithAltitude(session,
                            new GeoCoordinate(latitude, longitude, 10d), 0); // Set speed to 0 for random speed.
                    await Task.Delay(1000);
                    latitude += 0.00000001;
                    longitude += 0.00000001;

                    session.EventDispatcher.Send(new UpdatePositionEvent
                    {
                        Longitude = longitude,
                        Latitude = latitude
                    });
                    await Task.Delay(1000);
                    var mapObjects = session.Client.Map.GetMapObjects().Result;
                    //session.AddForts(mapObjects.Item1.MapCells.SelectMany(p => p.Forts).ToList());
                    catchablePokemon =
                        mapObjects.Item1.MapCells.SelectMany(q => q.CatchablePokemons)
                            .Where(q => pokemonIds.Contains(q.PokemonId))
                            .OrderByDescending(pokemon => PokemonInfo.CalculateMaxCpMultiplier(pokemon.PokemonId))
                            .ToList();
                } while (catchablePokemon.Count == 0 && retry > 0);
                

            }
            catch(CaptchaException ex)
            {
                //isCaptchaShow = true;
                throw ex;
            }
            finally
            {
                  //if(!isCaptchaShow)
                await
                    LocationUtils.UpdatePlayerLocationWithAltitude(session,
                        new GeoCoordinate(currentLatitude, currentLongitude, session.Client.CurrentAltitude), 0); // Set speed to 0 for random speed.
            }

            if (catchablePokemon.Count == 0)
            {

                // Pokemon not found but we still add to the locations visited, so we don't keep sniping
                // locations with no pokemon.
                if (!LocsVisited.Contains(new PokemonLocation(latitude, longitude)))
                    LocsVisited.Add(new PokemonLocation(latitude, longitude));

                session.EventDispatcher.Send(new SnipeEvent
                {
                    Message = session.Translation.GetTranslation(TranslationString.NoPokemonToSnipe)
                });
                return false;
            }

            isCaptchaShow = false;
            foreach (var pokemon in catchablePokemon)
            {
                EncounterResponse encounter;
                try
                {
                    await
                        LocationUtils.UpdatePlayerLocationWithAltitude(session,
                            new GeoCoordinate(latitude, longitude, session.Client.CurrentAltitude), 0); // Set speed to 0 for random speed.

                    encounter =
                        session.Client.Encounter.EncounterPokemon(pokemon.EncounterId, pokemon.SpawnPointId).Result;
                }
                catch (CaptchaException ex)
                {
                    isCaptchaShow = true;
                    throw ex;
                }
                finally
                {   if(!isCaptchaShow)
                    await
                        LocationUtils.UpdatePlayerLocationWithAltitude(session,
                            new GeoCoordinate(currentLatitude, currentLongitude, session.Client.CurrentAltitude), 0); // Set speed to 0 for random speed.
                }

                switch (encounter.Status)
                {
                    case EncounterResponse.Types.Status.EncounterSuccess:

                        if (!LocsVisited.Contains(new PokemonLocation(latitude, longitude)))
                            LocsVisited.Add(new PokemonLocation(latitude, longitude));

                        //Also add exact pokemon location to LocsVisited, some times the server one differ a little.
                        if (!LocsVisited.Contains(new PokemonLocation(pokemon.Latitude, pokemon.Longitude)))
                            LocsVisited.Add(new PokemonLocation(pokemon.Latitude, pokemon.Longitude));

                        session.EventDispatcher.Send(new UpdatePositionEvent
                        {
                            Latitude = currentLatitude,
                            Longitude = currentLongitude
                        });

                        catchedPokemon = await CatchPokemonTask.Execute(session, cancellationToken, encounter, pokemon, 
                            currentFortData: null, sessionAllowTransfer: true);
                        break;

                    case EncounterResponse.Types.Status.PokemonInventoryFull:

                        if (session.LogicSettings.TransferDuplicatePokemon)
                        {
                            await TransferDuplicatePokemonTask.Execute(session, cancellationToken);
                        }
                        else
                        {
                            session.EventDispatcher.Send(new WarnEvent
                            {
                                Message = session.Translation.GetTranslation(TranslationString.InvFullTransferManually)
                            });
                        }
                        return false;

                    default:
                        session.EventDispatcher.Send(new WarnEvent
                        {
                            Message =
                                session.Translation.GetTranslation(
                                    TranslationString.EncounterProblem, encounter.Status)
                        });
                        break;
                }

                if (!Equals(catchablePokemon.ElementAtOrDefault(catchablePokemon.Count - 1), pokemon))
                    await Task.Delay(session.LogicSettings.DelayBetweenPokemonCatch, cancellationToken);
            }

            _lastSnipe = DateTime.Now;

            if (catchedPokemon)
            {
                session.Stats.SnipeCount++;
                session.Stats.LastSnipeTime = _lastSnipe;
            }
            session.EventDispatcher.Send(new SnipeModeEvent {Active = false});
            return true;
            //await Task.Delay(session.LogicSettings.DelayBetweenPlayerActions, cancellationToken);
        }

        private static ScanResult SnipeScanForPokemon(ISession session, Location location)
        {
            var formatter = new NumberFormatInfo {NumberDecimalSeparator = "."};

            var offset = session.LogicSettings.SnipingScanOffset;
            // 0.003 = half a mile; maximum 0.06 is 10 miles
            if (offset < 0.001) offset = 0.003;
            if (offset > 0.06) offset = 0.06;

            var boundLowerLeftLat = location.Latitude - offset;
            var boundLowerLeftLng = location.Longitude - offset;
            var boundUpperRightLat = location.Latitude + offset;
            var boundUpperRightLng = location.Longitude + offset;

            var uri =
                $"http://skiplagged.com/api/pokemon.php?bounds={boundLowerLeftLat.ToString(formatter)},{boundLowerLeftLng.ToString(formatter)},{boundUpperRightLat.ToString(formatter)},{boundUpperRightLng.ToString(formatter)}";

            ScanResult scanResult;
            try
            {
                var request = WebRequest.CreateHttp(uri);
                request.Accept = "application/json";
                request.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.11; rv:47.0) Gecko/20100101 Firefox/47.0";
                request.Method = "GET";
                request.Timeout = 15000;
                request.ReadWriteTimeout = 32000;

                var resp = request.GetResponse();
                var reader = new StreamReader(resp.GetResponseStream());
                var fullresp =
                    reader.ReadToEnd()
                        .Replace(" M", "Male")
                        .Replace(" F", "Female")
                        .Replace("Farfetch'd", "Farfetchd")
                        .Replace("Mr.Maleime", "MrMime");

                scanResult = JsonConvert.DeserializeObject<ScanResult>(fullresp);
            }
            catch(ActiveSwitchByRuleException asx)
            {
                throw asx;
            }
            catch (CaptchaException cex)
            {
                throw cex;
            }

            catch (Exception ex)

            {
                // most likely System.IO.IOException
                session.EventDispatcher.Send(new ErrorEvent {Message = ex.Message});
                scanResult = new ScanResult
                {
                    Status = "fail",
                    pokemons = new List<PokemonLocation>()
                };
            }
            return scanResult;
        }

        private static List<SniperInfo> GetSniperInfoFrom_pokezz(ISession session, ICollection<PokemonId> pokemonIds)
        {
            var options = new IO.Options
            {
                Transports = ImmutableList.Create("websocket")
            };

            var socket = IO.Socket("http://pokezz.com", options);

            var hasError = true;

            var waitforbroadcast = new ManualResetEventSlim(false);

            var pokemons = new List<PokemonLocationPokezz>();

            socket.On("a", msg =>
            {
                hasError = false;
                socket.Close();
                var pokemonDefinitions = ((string) msg).Split('~');
                foreach (var pokemonDefinition in pokemonDefinitions)
                {
                    try
                    {
                        var pokemonDefinitionElements = pokemonDefinition.Split('|');
                        var pokezzElement = new PokemonLocationPokezz
                        {
                            name =
                                (PokemonId) Convert.ToInt32(pokemonDefinitionElements[0], CultureInfo.InvariantCulture),
                            lat = Convert.ToDouble(pokemonDefinitionElements[1], CultureInfo.InvariantCulture),
                            lng = Convert.ToDouble(pokemonDefinitionElements[2], CultureInfo.InvariantCulture),
                            time = Convert.ToDouble(pokemonDefinitionElements[3], CultureInfo.InvariantCulture),
                            verified = pokemonDefinitionElements[4] != "0",
                            iv = pokemonDefinitionElements[5]
                        };

                        pokemons.Add(pokezzElement);
                    }
                    catch (CaptchaException cex)
                    {
                        throw cex;
                    }
                 
                    catch (Exception)
                    {
                        // Just in case Pokezz changes their implementation, let's catch the error and set the error flag.
                        hasError = true;
                    }
                }

                waitforbroadcast.Set();
            });

            socket.On(Socket.EVENT_ERROR, () =>
            {
                socket.Close();
                waitforbroadcast.Set();
            });

            socket.On(Socket.EVENT_CONNECT_ERROR, () =>
            {
                socket.Close();
                waitforbroadcast.Set();
            });

            waitforbroadcast.Wait(5000); // Wait a maximum of 5 seconds for Pokezz to respond.
            socket.Close();

            if (hasError)
            {
                session.EventDispatcher.Send(new ErrorEvent {Message = "(Pokezz.com) Connection Error"});
                return null;
            }

            foreach (var pokemon in pokemons)
            {
                var snipInfo = new SniperInfo
                {
                    Id = pokemon.name,
                    Latitude = pokemon.lat,
                    Longitude = pokemon.lng,
                    TimeStampAdded = DateTime.Now,
                    ExpirationTimestamp =
                        new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(pokemon.time).ToLocalTime(),
                    IV = pokemon._iv
                };
                if (pokemon.verified || !session.LogicSettings.GetOnlyVerifiedSniperInfoFromPokezz)
                    SnipeLocations.Add(snipInfo);
            }

            var locationsToSnipe = new List<SniperInfo>();
            foreach (var q in SnipeLocations)
            {
                string status;
                if (!session.LogicSettings.UseTransferIvForSnipe ||
                    (q.IV < 1 && !session.LogicSettings.SnipeIgnoreUnknownIv) ||
                    (q.IV >= session.Inventory.GetPokemonTransferFilter(q.Id).KeepMinIvPercentage))
                {
                    if (!LocsVisited.Contains(new PokemonLocation(q.Latitude, q.Longitude)))
                    {
                        if (q.ExpirationTimestamp != default(DateTime) && q.ExpirationTimestamp > new DateTime(2016) &&
                            q.ExpirationTimestamp > DateTime.Now.AddSeconds(20))
                        {
                            if (q.Id == PokemonId.Missingno || pokemonIds.Contains(q.Id))
                            {
                                locationsToSnipe.Add(q);
                                status = "Snipe! Let's Go!";
                            }
                            else
                            {
                                status = "Not User Selected Pokemon";
                            }
                        }
                        else
                        {
                            status = "Expired";
                        }
                    }
                    else
                    {
                        status = "Already Visited";
                    }
                }
                else
                {
                    status = "IV too low or user choosed ignore unknown IV pokemon";
                }
                var message = "Pokezz: Found a " + q.Id + " in " + q.Latitude.ToString("0.0000") + "," +
                              q.Longitude.ToString("0.0000") + " Time Remain:" +
                              (q.ExpirationTimestamp - DateTime.Now).TotalSeconds.ToString("0") + "s " +
                              " Status: " + status;

                Debug.Print(message);
                //session.EventDispatcher.Send(new SnipeEvent {Message = message});
            }
            return locationsToSnipe.OrderBy(q => q.ExpirationTimestamp).ToList();
        }

        private static List<SniperInfo> GetSniperInfoFrom_pokesnipers(ISession session,
            ICollection<PokemonId> pokemonIds)
        {
            var uri = $"http://pokesnipers.com/api/v1/pokemon.json";

            ScanResultPokesnipers scanResultPokesnipers;
            try
            {
                var handler = new ClearanceHandler();

                // Create a HttpClient that uses the handler.
                var client = new HttpClient(handler);

                // Use the HttpClient as usual. Any JS challenge will be solved automatically for you.
                var fullresp =
                    client.GetStringAsync(uri)
                        .Result.Replace(" M", "Male")
                        .Replace(" F", "Female")
                        .Replace("Farfetch'd", "Farfetchd")
                        .Replace("Mr.Maleime", "MrMime");

                scanResultPokesnipers = JsonConvert.DeserializeObject<ScanResultPokesnipers>(fullresp);
            }
            catch (Exception ex)
            {
                // most likely System.IO.IOException
                session.EventDispatcher.Send(new ErrorEvent {Message = "(PokeSnipers.com) " + ex.Message});
                return null;
            }

            if (scanResultPokesnipers.pokemons == null)
                return null;

            foreach (var pokemon in scanResultPokesnipers.pokemons)
            {
                try
                {
                    var snipInfo = new SniperInfo {Id = pokemon.name};
                    var coordsArray = pokemon.coords.Split(',');
                    snipInfo.Latitude = Convert.ToDouble(coordsArray[0], CultureInfo.InvariantCulture);
                    snipInfo.Longitude = Convert.ToDouble(coordsArray[1], CultureInfo.InvariantCulture);
                    snipInfo.TimeStampAdded = DateTime.Now;
                    snipInfo.ExpirationTimestamp = Convert.ToDateTime(pokemon.until);
                    snipInfo.IV = pokemon.iv;
                    SnipeLocations.Add(snipInfo);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            var locationsToSnipe = new List<SniperInfo>();
            foreach (var q in SnipeLocations)
            {
                string status;
                if (!session.LogicSettings.UseTransferIvForSnipe ||
                    (q.IV < 1 && !session.LogicSettings.SnipeIgnoreUnknownIv) ||
                    (q.IV >= session.Inventory.GetPokemonTransferFilter(q.Id).KeepMinIvPercentage))
                {
                    if (!LocsVisited.Contains(new PokemonLocation(q.Latitude, q.Longitude)))
                    {
                        if (q.ExpirationTimestamp != default(DateTime) && q.ExpirationTimestamp > new DateTime(2016) &&
                            q.ExpirationTimestamp > DateTime.Now.AddSeconds(20))
                        {
                            if (q.Id == PokemonId.Missingno || pokemonIds.Contains(q.Id))
                            {
                                locationsToSnipe.Add(q);
                                status = "Snipe! Let's Go!";
                            }
                            else
                            {
                                status = "Not User Selected Pokemon";
                            }
                        }
                        else
                        {
                            status = "Expired";
                        }
                    }
                    else
                    {
                        status = "Already Visited";
                    }
                }
                else
                {
                    status = "IV too low or user choosed ignore unknown IV pokemon";
                }
                var message = "Pokesniper: Found a " + q.Id + " in " + q.Latitude.ToString("0.0000") + "," +
                              q.Longitude.ToString("0.0000") + " Time Remain:" +
                              (q.ExpirationTimestamp - DateTime.Now).TotalSeconds.ToString("0") + "s " +
                              " Status: " + status;

                Debug.Print(message);
                //session.EventDispatcher.Send(new SnipeEvent {Message = message});
            }
            return locationsToSnipe.OrderBy(q => q.ExpirationTimestamp).ToList();
        }

        private static List<SniperInfo> GetSniperInfoFrom_pokewatchers(ISession session,
            ICollection<PokemonId> pokemonIds)
        {
            var uri = $"http://pokewatchers.com/grab/";

            ScanResultPokewatchers scanResultPokewatchers;
            try
            {
                var handler = new ClearanceHandler();

                // Create a HttpClient that uses the handler.
                var client = new HttpClient(handler);

                // Our new firewall requires a user agent, or you'll be blocked.
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent",
                    "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/511.2 (KHTML, like Gecko) Chrome/15.0.041.151 Safari/555.2");

                string response;
                var retries = 0;
                bool retry;

                // Retry up to 5 times, sleeping for 1s between retries.
                do
                {
                    // Use the HttpClient as usual. Any JS challenge will be solved automatically for you.
                    response = client.GetStringAsync(uri).Result;
                    if (response == "You can only request the API every 5 seconds.")
                    {
                        retry = true;
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        retry = false;
                    }
                } while (retry && (retries++ < 5));

                if (response == null || response == "You can only request the API every 5 seconds.")
                {
                    response = "[]";
                }

                var fullresp = "{ \"pokemons\":" + response + "}".Replace("Mr_mime", "MrMime");
                scanResultPokewatchers = JsonConvert.DeserializeObject<ScanResultPokewatchers>(fullresp);
            }
            catch(ActiveSwitchByRuleException ex1)
            {
                throw ex1;
            }
            catch (Exception ex)
            {
                // most likely System.IO.IOException
                session.EventDispatcher.Send(new ErrorEvent {Message = "(PokeWatchers.com) " + ex.Message});
                return null;
            }

            if (scanResultPokewatchers.pokemons == null)
                return null;

            foreach (var pokemon in scanResultPokewatchers.pokemons)
            {
                try
                {
                    var snipInfo = new SniperInfo {Id = pokemon.pokemon};
                    var coordsArray = pokemon.cords.Split(',');
                    snipInfo.Latitude = Convert.ToDouble(coordsArray[0], CultureInfo.InvariantCulture);
                    snipInfo.Longitude = Convert.ToDouble(coordsArray[1], CultureInfo.InvariantCulture);
                    snipInfo.TimeStampAdded =
                        new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(pokemon.timeadded);
                    snipInfo.ExpirationTimestamp =
                        new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(pokemon.timeend);
                    SnipeLocations.Add(snipInfo);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            var locationsToSnipe = new List<SniperInfo>();
            foreach (var q in SnipeLocations)
            {
                string status;
                if (!session.LogicSettings.UseTransferIvForSnipe ||
                    (q.IV < 1 && !session.LogicSettings.SnipeIgnoreUnknownIv) ||
                    (q.IV >= session.Inventory.GetPokemonTransferFilter(q.Id).KeepMinIvPercentage))
                {
                    if (!LocsVisited.Contains(new PokemonLocation(q.Latitude, q.Longitude)))
                    {
                        if (q.ExpirationTimestamp != default(DateTime) && q.ExpirationTimestamp > new DateTime(2016) &&
                            q.ExpirationTimestamp > DateTime.Now.AddSeconds(20))
                        {
                            if (q.Id == PokemonId.Missingno || pokemonIds.Contains(q.Id))
                            {
                                locationsToSnipe.Add(q);
                                status = "Snipe! Let's Go!";
                            }
                            else
                            {
                                status = "Not User Selected Pokemon";
                            }
                        }
                        else
                        {
                            status = "Expired";
                        }
                    }
                    else
                    {
                        status = "Already Visited";
                    }
                }
                else
                {
                    status = "IV too low  or user choosed ignore unknown IV pokemon";
                }
                var message = "Pokewatcher:  Found a " + q.Id + " in " + q.Latitude.ToString("0.0000") + "," +
                              q.Longitude.ToString("0.0000") + " Time Remain:" +
                              (q.ExpirationTimestamp - DateTime.Now).TotalSeconds.ToString("0") + "s " +
                              " Status: " + status;

                Debug.Print(message);
                //session.EventDispatcher.Send(new SnipeEvent {Message = message});
            }
            return locationsToSnipe.OrderBy(q => q.ExpirationTimestamp).ToList();
        }

        public static async Task Start(Session session, CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var lClient = new TcpClient();
                    lClient.Connect(session.LogicSettings.SnipeLocationServer,
                        session.LogicSettings.SnipeLocationServerPort);

                    var sr = new StreamReader(lClient.GetStream());

                    while (lClient.Connected)
                    {
                        try
                        {
                            var line = sr.ReadLine();
                            if (line == null)
                                throw new Exception("Unable to ReadLine from sniper socket");

                            var info = JsonConvert.DeserializeObject<SniperInfo>(line);

                            if (SnipeLocations.Any(x =>
                                Math.Abs(x.Latitude - info.Latitude) < 0.0001 &&
                                Math.Abs(x.Longitude - info.Longitude) < 0.0001))
                                // we might have different precisions from other sources
                                continue;

                            SnipeLocations.RemoveAll(x => _lastSnipe > x.TimeStampAdded);
                            SnipeLocations.RemoveAll(x => DateTime.Now > x.TimeStampAdded.AddMinutes(15));
                            SnipeLocations.Add(info);
                            session.EventDispatcher.Send(new SnipePokemonFoundEvent {PokemonFound = info});
                        }
                        catch (IOException)
                        {
                            session.EventDispatcher.Send(new ErrorEvent
                            {
                                Message = "The connection to the sniping location server was lost."
                            });
                        }
                    }
                }
                catch (SocketException)
                {
                }
                catch (Exception ex)
                {
                    // most likely System.IO.IOException
                    session.EventDispatcher.Send(new ErrorEvent {Message = ex.ToString()});
                }

                await Task.Delay(100, cancellationToken);
            }
        }
    }
}