using GeoCoordinatePortable;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.Exceptions;
using PoGo.NecroBot.Logic.Logging;
using PoGo.NecroBot.Logic.Model.Settings;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Utils;
using POGOProtos.Data;
using POGOProtos.Enums;
using POGOProtos.Map.Pokemon;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Tasks
{
    public static class MSniperServiceTask
    {
        #region Variables
        private const int SNIPE_SAFE_TIME = 180;
        public static List<EncounterInfo> LocationQueue = new List<EncounterInfo>();
        public static List<string> VisitedEncounterIds = new List<string>();
        private static List<MSniperInfo2> autoSnipePokemons = new List<MSniperInfo2>();
        private static List<MSniperInfo2> manualSnipePokemons = new List<MSniperInfo2>();
        private static List<MSniperInfo2> pokedexSnipePokemons = new List<MSniperInfo2>();
        private static bool inProgress = false;
        private static DateTime OutOffBallBlock = DateTime.MinValue;
        public static bool isConnected = false;
        public static double minIvPercent = 0.0;//no iv filter
        private static string _botIdentiy;
        private static HubConnection _connection;
        private static IHubProxy _msniperHub;
        private static string _msniperServiceUrl = "http://msniper.com/signalr";

        #endregion Variables

        #region signalr msniper service

        public static void ConnectToService()
        {
            while (true)
            {
                try
                {
                    if (!isConnected)
                    {
                        Thread.Sleep(10000);
                        _connection = new HubConnection(_msniperServiceUrl, useDefaultUrl: false);
                        _msniperHub = _connection.CreateHubProxy("msniperHub");
                        _msniperHub.On<MSniperInfo2>("msvc", p =>
                        {
                            lock(locker)
                            {
                                autoSnipePokemons.Add(p);
                            }
                        });
                        _connection.Received += Connection_Received;
                        _connection.Reconnecting += Connection_Reconnecting;
                        //_connection.Reconnected += Connection_Reconnected;
                        _connection.Closed += Connection_Closed;
                        _connection.Start().Wait();
                        //Logger.Write("connecting", LogLevel.Service);
                        _msniperHub.Invoke("Identity");
                        isConnected = true;
                    }
                    break;
                }
                catch (CaptchaException cex)
                {
                    throw cex;
                }

                catch (Exception)
                {
                    //Logger.Write("service: " +e.Message, LogLevel.Error);
                    Thread.Sleep(500);
                }
            }
        }

        private static void Connection_Closed()
        {
            //Logger.Write("connection closed, trying to reconnect in 10secs", LogLevel.Service);
            ConnectToService();
        }

        private static void Connection_Received(string obj)
        {
            try
            {
                HubData xx = _connection.JsonDeserializeObject<HubData>(obj);
                switch (xx.Method)
                {
                    case "sendIdentity":
                        _botIdentiy = xx.List[0];
                        Logger.Write($"(Identity) [ {_botIdentiy} ] connection establisted", LogLevel.Service);
                        //Console.WriteLine($"[{numb}]now waiting pokemon request (15sec)");
                        break;

                    case "sendPokemon":
                        RefreshLocationQueue();
                        if (LocationQueue.Count > 0)
                        {
                            //Logger.Write($"pokemons are sending.. {LocationQueue.Count} count", LogLevel.Service);
                            var findingSendables = FindNew(LocationQueue);
                            AddToVisited(findingSendables);
                            _msniperHub.Invoke("RecvPokemons", findingSendables);
                            findingSendables.ForEach(p =>
                            {
                                LocationQueue.Remove(p);
                            });
                        }
                        break;

                    case "Exceptions":
                        var defaultc = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Logger.Write("ERROR: " + xx.List.FirstOrDefault(), LogLevel.Service);
                        Console.ForegroundColor = defaultc;
                        break;
                }
            }
            catch (Exception)
            {
            }
        }

        private static void Connection_Reconnecting()
        {
            isConnected = false;
            _connection.Stop(); //removing server cache
            ConnectToService();
            //Logger.Write("reconnecting", LogLevel.Service);
        }

        //private static void Connection_Reconnected()
        //{
        //    Logger.Write("reconnected", LogLevel.Service);
        //}

        #endregion signalr msniper service

        #region Classes

        public class EncounterInfo : IEvent
        {
            public string EncounterId { get; set; }
            public long Expiration { get; set; }
            public double Iv { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public string Move1 { get; set; }
            public string Move2 { get; set; }
            public int PokemonId { get; set; }
            public string PokemonName { get; set; }
            public string SpawnPointId { get; set; }
            //public long LastModifiedTimestampMs { get; set; }
            //public int TimeTillHiddenMs { get; set; }

            public ulong GetEncounterId()
            {
                return Convert.ToUInt64(EncounterId);
            }

            public double GetLatitude()
            {
                return double.Parse(Latitude, CultureInfo.InvariantCulture);
            }

            public double GetLongitude()
            {
                return double.Parse(Longitude, CultureInfo.InvariantCulture);
            }

            public PokemonId GetPokemonName()
            {
                return (PokemonId)PokemonId;
            }
        }

        static bool isBlocking = true; //turn it on when account switching, do not add or run snipe
        public static void BlockSnipe()
        {
            isBlocking = true;
        }

        public class HubData
        {
            [JsonProperty("H")]
            public string HubName { get; set; }

            [JsonProperty("A")]
            public List<string> List { get; set; }

            [JsonProperty("M")]
            public string Method { get; set; }
        }

        public class MSniperInfo2
        {
            public DateTime AddedTime { get; set; }
            public ulong EncounterId { get; set; }
            public double ExpiredTime { get; set; }
            public double Iv { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public PokemonMove Move1 { get; set; }
            public PokemonMove Move2 { get; set; }
            public short PokemonId { get; set; }
            public string SpawnPointId { get; set; }
        }

        #endregion Classes


        #region MSniper Location Feeder

        public static void AddToList(IEvent evt)
        {
            if (evt is EncounterInfo)
            {
                var xx = TimeStampToDateTime((evt as EncounterInfo).Expiration);
                var ff = DateTime.Now;
                if ((ff - xx).TotalMinutes < 1)
                {
                    (evt as EncounterInfo).Expiration += 500000;
                    LocationQueue.Add(evt as EncounterInfo);
                }
                else
                {
                    //we need exact expiry time,so here is disabled
                }
            }
        }

        public static void AddToVisited(List<EncounterInfo> encounterIds)
        {
            encounterIds.ForEach(p =>
            {
                string query = $"{p.EncounterId}-{p.SpawnPointId}";
                if (!VisitedEncounterIds.Contains(query))
                    VisitedEncounterIds.Add(query);
            });
        }

        public static void UnblockSnipe(bool spinned = true)
        {
            isBlocking = false;//block release whenever first pokestop looted.

            snipeFailedCount = 0;
            waitNextPokestop = spinned;
        }
        static DateTime lastPrintMessageTime = DateTime.Now;

        private static bool CheckSnipeConditions(ISession session)
        {
            //if (waitNextPokestop) return false;
            if (session.LoggedTime > DateTime.Now.AddMinutes(1)) return false; //only snipe after login 1 min.

            if (snipeFailedCount >= 3) return false;
            if (session.Stats.CatchThresholdExceeds(session)) return false;

            if (inProgress || OutOffBallBlock > DateTime.Now)
                return false;

            if (!session.LogicSettings.UseSnipeLimit) return true;

            if (lastPrintMessageTime.AddMinutes(1) > DateTime.Now)
            {
                session.EventDispatcher.Send(new SnipeEvent
                {
                    Message = session.Translation.GetTranslation(TranslationString.SniperCount, session.Stats.SnipeCount)
                });

            }
            if (session.Stats.LastSnipeTime.AddMilliseconds(session.LogicSettings.MinDelayBetweenSnipes) > DateTime.Now) return false;

            if (session.Stats.SnipeCount < session.LogicSettings.SnipeCountLimit)
                return true;

            if ((DateTime.Now - session.Stats.LastSnipeTime).TotalSeconds > session.LogicSettings.SnipeRestSeconds)
            {
                session.Stats.SnipeCount = 0;
            }
            else
            {
                if (lastPrintMessageTime.AddMinutes(1) > DateTime.Now)
                {

                    lastPrintMessageTime = DateTime.Now;
                    session.EventDispatcher.Send(new SnipeEvent
                    {
                        Message = session.Translation.GetTranslation(TranslationString.SnipeExceeds)
                    });
                }
                return false;
            }
            return true;
        }

        public static async Task<bool> CatchFromService(ISession session, CancellationToken cancellationToken, MSniperInfo2 encounterId)
        {
            cancellationToken.ThrowIfCancellationRequested();

            double lat = session.Client.CurrentLatitude;
            double lon = session.Client.CurrentLongitude;

            bool captchaShowed = false;
            EncounterResponse encounter;
            try
            {
                await LocationUtils.UpdatePlayerLocationWithAltitude(session,
                   new GeoCoordinate(encounterId.Latitude, encounterId.Longitude, session.Client.CurrentAltitude), 0); // Speed set to 0 for random speed.

                await Task.Delay(1000, cancellationToken);

                encounter = await session.Client.Encounter.EncounterPokemon(encounterId.EncounterId, encounterId.SpawnPointId);
                
               
            }
            catch (CaptchaException ex)
            {
                captchaShowed = true;
                throw ex;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (!captchaShowed)
                {
                    //TODO - What if udpate location failed 
                    var response = await LocationUtils.UpdatePlayerLocationWithAltitude(session,
                        new GeoCoordinate(lat, lon, session.Client.CurrentAltitude), 0);  // Speed set to 0 for random speed.

                    
                }
                else
                    session.Client.Player.SetCoordinates(lat, lon, session.Client.CurrentAltitude); //only reset d
            }

            if (encounter.Status == EncounterResponse.Types.Status.PokemonInventoryFull)
            {
                Logger.Write("Pokemon bag full, snipe cancel");
                await TransferDuplicatePokemonTask.Execute(session, cancellationToken);
                return false;
            }
            PokemonData encounteredPokemon;

            // Catch if it's a WildPokemon (MSniping not allowed for Incense pokemons)
            if (encounter?.Status == EncounterResponse.Types.Status.EncounterSuccess)
            {
                encounteredPokemon = encounter.WildPokemon?.PokemonData;
            }
            else
            {
                Logger.Write($"Pokemon despawned or wrong link format!", LogLevel.Service, ConsoleColor.Gray);
                return false;
                //return await CatchWithSnipe(session, encounterId, cancellationToken);// No success to work with
            }

            var pokemon = new MapPokemon
            {
                EncounterId = encounterId.EncounterId,
                Latitude = encounterId.Latitude,
                Longitude = encounterId.Longitude,
                PokemonId = encounteredPokemon.PokemonId,
                SpawnPointId = encounterId.SpawnPointId
            };

            return await CatchPokemonTask.Execute(session, cancellationToken, encounter, pokemon, currentFortData: null, sessionAllowTransfer: true);
        }

        public static List<EncounterInfo> FindNew(List<EncounterInfo> received)
        {
            List<EncounterInfo> newOne = new List<EncounterInfo>();
            received.ForEach(p =>
            {
                if (!VisitedEncounterIds.Contains($"{p.EncounterId}-{p.SpawnPointId}"))
                    newOne.Add(p);
            });
            return newOne;
        }

        public static DateTime TimeStampToDateTime(double timeStamp)
        {
            // Java timestamp is millisecods past epoch
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(Math.Round(timeStamp / 1000)).ToLocalTime();
            return dtDateTime;
        }

        private static void RefreshLocationQueue()
        {
            var pkmns = LocationQueue
                .Where(p => TimeStampToDateTime(p.Expiration) > DateTime.Now)
                .ToList();
            LocationQueue.Clear();
            LocationQueue.AddRange(pkmns);
        }

        #endregion MSniper Location Feeder

        private static object locker = new object();

        public static async Task AddSnipeItem(ISession session, MSniperInfo2 item, bool byPassValidation = false)
        {
            if (isBlocking) return;
            if (Math.Abs(item.Latitude) > 90 || Math.Abs(item.Longitude) > 180) return;

            lock (locker)
            {
                item.AddedTime = DateTime.Now;
                //just keep pokemon in last 2 min
                autoSnipePokemons.RemoveAll(x => x.AddedTime.AddSeconds(SNIPE_SAFE_TIME) < DateTime.Now);// || (x.ExpiredTime > 0 && x.ExpiredTime < DateTime.Now.ToUnixTime()));
                pokedexSnipePokemons.RemoveAll(x => x.AddedTime.AddMinutes(2) < DateTime.Now);
            }
            if (OutOffBallBlock > DateTime.Now ||
                autoSnipePokemons.Exists(x => x.EncounterId == item.EncounterId && item.EncounterId > 0) ||
                (item.EncounterId > 0 && session.Cache[item.EncounterId.ToString()] != null) ||
                (session.LogicSettings.SnipeIgnoreUnknownIv && item.Iv == 0 && !byPassValidation)) return;

            item.Iv = Math.Round(item.Iv, 2);
            if (session.LogicSettings.SnipePokemonNotInPokedex)
            {
                var pokedex = await session.Inventory.GetPokeDexItems();

                if (!pokedex.Exists(x => x.InventoryItemData?.PokedexEntry?.PokemonId == (PokemonId)item.PokemonId) &&
                    !pokedexSnipePokemons.Exists(p => p.PokemonId == item.PokemonId) &&
                    (!session.LogicSettings.AutosnipeVerifiedOnly || (session.LogicSettings.AutosnipeVerifiedOnly && item.EncounterId > 0)))
                {
                    session.EventDispatcher.Send(new WarnEvent() { Message = session.Translation.GetTranslation(TranslationString.SnipePokemonNotInPokedex, session.Translation.GetPokemonTranslation((PokemonId)item.PokemonId)) });
                    pokedexSnipePokemons.Add(item);//Add as hight priority snipe entry
                    return;
                }
            }
            SnipeFilter filter = new SnipeFilter()
            {
                SnipeIV = session.LogicSettings.MinIVForAutoSnipe,
                VerifiedOnly = session.LogicSettings.AutosnipeVerifiedOnly
            };

            var pokemonId = (PokemonId)item.PokemonId;

            if (session.LogicSettings.PokemonSnipeFilters.ContainsKey(pokemonId))
            {
                filter = session.LogicSettings.PokemonSnipeFilters[pokemonId];
            }
            lock (locker)
            {
                if (byPassValidation)
                {
                    manualSnipePokemons.Add(item);

                    Logger.Write($"(MANUAL SNIPER) Pokemon added |  {(PokemonId)item.PokemonId} [{item.Latitude},{item.Longitude}] IV {item.Iv}%");
                    return;
                }

                //hack, this case we can't determite move :)

                if (filter.VerifiedOnly && item.EncounterId == 0) return;

                if (filter.SnipeIV <= item.Iv && item.Move1 == PokemonMove.Absorb && item.Move2 == PokemonMove.Absorb)
                {
                    autoSnipePokemons.Add(item);
                    return;
                }
                //ugly but readable
                if ((string.IsNullOrEmpty(filter.Operator) || filter.Operator == Operator.or.ToString()) &&
                    (filter.SnipeIV <= item.Iv
                    || (filter.Moves != null
                        && filter.Moves.Count > 0
                        && filter.Moves.Any(x => x[0] == item.Move1 && x[1] == item.Move2))
                    ))

                {
                    autoSnipePokemons.Add(item);
                }

                if (filter.Operator == Operator.and.ToString() &&
                   (filter.SnipeIV <= item.Iv
                   && (filter.Moves != null
                       && filter.Moves.Count > 0
                       && filter.Moves.Any(x => x[0] == item.Move1 && x[1] == item.Move2))
                   ))
                {
                    autoSnipePokemons.Add(item);
                }
            }
        }
    

        public static async Task<bool> CatchWithSnipe(ISession session, MSniperInfo2 encounterId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await
                  SnipePokemonTask.Snipe(session, new List<PokemonId>() { (PokemonId)encounterId.PokemonId }, encounterId.Latitude, encounterId.Longitude, cancellationToken);
        }

        static int snipeFailedCount = 0;
        static bool waitNextPokestop = true;
        public static async Task Execute(ISession session, CancellationToken cancellationToken)
        {
            if (!CheckSnipeConditions(session)) return;

            inProgress = true;

            var pth = Path.Combine(Directory.GetCurrentDirectory(), "SnipeMS.json");
            try
            {
                if (OutOffBallBlock > DateTime.Now || (
                    File.Exists(pth) && autoSnipePokemons.Count ==0 && manualSnipePokemons.Count ==0 && pokedexSnipePokemons.Count ==0))
                {
                    return;
                }

                if (autoSnipePokemons.Count >0 && !await SnipePokemonTask.CheckPokeballsToSnipe(session.LogicSettings.MinPokeballsToSnipe + 1, session, cancellationToken))
                {
                    session.EventDispatcher.Send(new WarnEvent()
                    {
                        Message = session.Translation.GetTranslation(Common.TranslationString.AutoSnipeDisabled, session.LogicSettings.SnipePauseOnOutOfBallTime)
                    });

                    OutOffBallBlock = DateTime.Now.AddMinutes(session.LogicSettings.SnipePauseOnOutOfBallTime);
                    return;
                }
                List<MSniperInfo2> mSniperLocation2 = new List<MSniperInfo2>();
                if (File.Exists(pth))
                {
                    var sr = new StreamReader(pth, Encoding.UTF8);
                    var jsn = sr.ReadToEnd();
                    sr.Close();

                    mSniperLocation2 = JsonConvert.DeserializeObject<List<MSniperInfo2>>(jsn);
                    File.Delete(pth);
                    if (mSniperLocation2 == null) mSniperLocation2 = new List<MSniperInfo2>();
                }
                lock (locker)
                {
                    if (pokedexSnipePokemons.Count > 0)
                    {
                        mSniperLocation2.Add(pokedexSnipePokemons.OrderByDescending(x => x.PokemonId).FirstOrDefault());
                        pokedexSnipePokemons.Clear();

                    }
                    if (manualSnipePokemons.Count > 0)
                    {

                        mSniperLocation2.AddRange(manualSnipePokemons);
                        manualSnipePokemons.Clear();
                    }
                    else {

                        autoSnipePokemons.RemoveAll(x => x.AddedTime.AddSeconds(SNIPE_SAFE_TIME) < DateTime.Now);// || ( x.ExpiredTime >0  && x.ExpiredTime < DateTime.Now.ToUnixTime()));
                        autoSnipePokemons.OrderByDescending(x => PokemonGradeHelper.GetPokemonGrade((PokemonId)x.PokemonId))
                                         .ThenByDescending(x => x.Iv)
                                         .ThenByDescending(x => x.PokemonId)
                                         .ThenByDescending(x => x.AddedTime);

                        var batch = autoSnipePokemons.Take(10);
                        //mSniperLocation2.AddRange(autoSnipePokemons.Take(10));
                        //autoSnipePokemons.Clear();
                        if (batch != null && batch.Count() > 0)
                        {
                            mSniperLocation2.AddRange(batch);
                            autoSnipePokemons.RemoveAll(x => batch.Contains(x));
                        }
                    }
                }
                foreach (var location in mSniperLocation2)
                {
                    if (session.Stats.CatchThresholdExceeds(session) || isBlocking) break;
                    lock (locker)
                    {
                        if (pokedexSnipePokemons.Count > 0) break;
                    }
                    if (location.EncounterId > 0 && session.Cache[location.EncounterId.ToString()] != null) continue;

                    if (!await SnipePokemonTask.CheckPokeballsToSnipe(session.LogicSettings.MinPokeballsWhileSnipe + 1, session, cancellationToken))
                    {
                        session.EventDispatcher.Send(new WarnEvent()
                        {
                            Message = session.Translation.GetTranslation(Common.TranslationString.AutoSnipeDisabled)
                        });

                        OutOffBallBlock = DateTime.Now.AddMinutes(session.LogicSettings.SnipePauseOnOutOfBallTime);
                        break;
                    }

                    if (location.AddedTime.AddSeconds(SNIPE_SAFE_TIME) < DateTime.Now) continue;

                    //If bot already catch the same pokemon, and very close this location. 
                    string uniqueCacheKey = $"{session.Settings.PtcUsername}{session.Settings.GoogleUsername}{Math.Round(location.Latitude, 6)}{location.PokemonId}{Math.Round(location.Longitude, 6)}";
                    if (session.Cache.Get(uniqueCacheKey) != null) continue;

                    session.Cache.Add(location.EncounterId.ToString(), true, DateTime.Now.AddMinutes(15));

                    cancellationToken.ThrowIfCancellationRequested();

                    session.EventDispatcher.Send(new SnipeScanEvent
                    {
                        Bounds = new Location(location.Latitude, location.Longitude),
                        PokemonId = (PokemonId)location.PokemonId,
                        Source = "MSniperService",
                        Iv = location.Iv
                    });
                    session.Stats.IsSnipping = true;
                    var result = location.EncounterId != 0 ? await CatchFromService(session, cancellationToken, location) : await CatchWithSnipe(session, location, cancellationToken);

                    if (result)
                    {
                        snipeFailedCount = 0;
                    }
                    else
                    {
                        snipeFailedCount++;
                        if (snipeFailedCount >= 3) break; //maybe softban, stop snipe wait until verify it not been 
                    }
                    await Task.Delay(1000, cancellationToken);
                    session.Stats.LastSnipeTime = DateTime.Now;
                    session.Stats.SnipeCount++;
                    waitNextPokestop = true;
                }
            }
            catch (ActiveSwitchByRuleException ex)
            {
                throw ex;
            }
            catch (CaptchaException cex)
            {
                throw cex;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException is CaptchaException) throw ex.InnerException;

                File.Delete(pth);
                var ee = new ErrorEvent { Message = ex.Message };
                if (ex.InnerException != null) ee.Message = ex.InnerException.Message;
                session.EventDispatcher.Send(ee);
            }
            finally
            {
                
                inProgress = false;
                session.Stats.IsSnipping = false;
            }
        }
    }
}