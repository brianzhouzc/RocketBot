#region using directives

using System;
using System.Threading;
using System.Threading.Tasks;
using GeoCoordinatePortable;
using PoGo.NecroBot.Logic.Interfaces.Configuration;
using PokemonGo.RocketAPI;
using POGOProtos.Networking.Responses;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Strategies.Walk;
using PoGo.NecroBot.Logic.Event;
using System.Collections.Generic;
using System.Linq;
using PoGo.NecroBot.Logic.Model;
using System.Net;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;
using POGOProtos.Map.Fort;
using PoGo.NecroBot.Logic.Utils;
using PokemonGo.RocketAPI.Extensions;

#endregion

namespace PoGo.NecroBot.Logic
{
    //add delegate
    public delegate void GetHumanizeRouteDelegate(List<GeoCoordinate> route, GeoCoordinate destination, List<FortData>pokeStops);
    public delegate void UpdatePositionDelegate(double lat, double lng);

    public class Navigation
    {
        public IWalkStrategy WalkStrategy { get; set; }
        private readonly Client _client;
        private Random WalkingRandom = new Random();
        private List<IWalkStrategy> WalkStrategyQueue { get; set; }
        public Dictionary<Type, DateTime> WalkStrategyBlackList = new Dictionary<Type, DateTime>();

        public Navigation(Client client, ILogicSettings logicSettings)
        {
            _client = client;
            
            InitializeWalkStrategies(logicSettings);
            WalkStrategy = GetStrategy(logicSettings);

        }

        public double VariantRandom(ISession session, double currentSpeed)
        {
            if (WalkingRandom.Next(1, 10) > 5)
            {
                if (WalkingRandom.Next(1, 10) > 5)
                {
                    var randomicSpeed = currentSpeed;
                    var max = session.LogicSettings.WalkingSpeedInKilometerPerHour + session.LogicSettings.WalkingSpeedVariant;
                    randomicSpeed += WalkingRandom.NextDouble() * (0.02 - 0.001) + 0.001;

                    if (randomicSpeed > max)
                        randomicSpeed = max;

                    if (Math.Round(randomicSpeed, 2) != Math.Round(currentSpeed, 2))
                    {
                        session.EventDispatcher.Send(new HumanWalkingEvent
                        {
                            OldWalkingSpeed = currentSpeed,
                            CurrentWalkingSpeed = randomicSpeed
                        });
                    }

                    return randomicSpeed;
                }
                else
                {
                    var randomicSpeed = currentSpeed;
                    var min = session.LogicSettings.WalkingSpeedInKilometerPerHour - session.LogicSettings.WalkingSpeedVariant;
                    randomicSpeed -= WalkingRandom.NextDouble() * (0.02 - 0.001) + 0.001;                    

                    if (randomicSpeed < min)
                        randomicSpeed = min;

                    if (Math.Round(randomicSpeed, 2) != Math.Round(currentSpeed, 2))
                    {
                        session.EventDispatcher.Send(new HumanWalkingEvent
                        {
                            OldWalkingSpeed = currentSpeed,
                            CurrentWalkingSpeed = randomicSpeed
                        });
                    }

                    return randomicSpeed;
                }
            }

            return currentSpeed;
        }
        private object ensureOneWalkEvent = new object();
        public async Task<PlayerUpdateResponse> Move(IGeoLocation targetLocation,
            Func<Task> functionExecutedWhileWalking,
            ISession session,
            CancellationToken cancellationToken, double customWalkingSpeed =0.0)
        {
            cancellationToken.ThrowIfCancellationRequested();

            //add routes to map
            var points = new List<GeoCoordinate>();
            var route = Route(session,
                new GeoCoordinate(
                    _client.CurrentLatitude,
                    _client.CurrentLongitude,
                    _client.CurrentAltitude),
                targetLocation.ToGeoCoordinate());

            foreach (var item in route)
                points.Add(new GeoCoordinate(item.ToArray()[1], item.ToArray()[0]));

            //get pokeStops to map
            var pokeStops = await GetPokeStops(session);
            OnGetHumanizeRouteEvent(points, targetLocation.ToGeoCoordinate(), pokeStops);
            //end code add routes

            // If the stretegies become bigger, create a factory for easy management

            return await WalkStrategy.Walk(targetLocation, functionExecutedWhileWalking, session, cancellationToken, customWalkingSpeed);
        }

        private void InitializeWalkStrategies(ILogicSettings logicSettings)
        {
            WalkStrategyQueue = new List<IWalkStrategy>();

            // Maybe change configuration for a Navigation Type.
            if (logicSettings.DisableHumanWalking)
            {
                WalkStrategyQueue.Add(new FlyStrategy(_client));
            }

            if (logicSettings.UseGpxPathing)
            {
                WalkStrategyQueue.Add(new HumanPathWalkingStrategy(_client));
            }
            
            if (logicSettings.UseGoogleWalk)
            {
                WalkStrategyQueue.Add(new GoogleStrategy(_client));
            }

            if (logicSettings.UseMapzenWalk)
            {
                WalkStrategyQueue.Add(new MapzenNavigationStrategy(_client));
            }

            if (logicSettings.UseYoursWalk)
            {
                WalkStrategyQueue.Add(new YoursNavigationStrategy(_client));
            }

            WalkStrategyQueue.Add(new HumanStrategy(_client));
        }

        public bool IsWalkingStrategyBlacklisted(Type strategy)
        {
            if (!WalkStrategyBlackList.ContainsKey(strategy))
                return false;

            DateTime now = DateTime.Now;
            DateTime blacklistExpiresAt = WalkStrategyBlackList[strategy];
            if (blacklistExpiresAt < now)
            {
                // Blacklist expired
                WalkStrategyBlackList.Remove(strategy);
                return false;
            }
            else
            {
                return true;
            }
        }

        public void BlacklistStrategy(Type strategy)
        {
            // Black list for 1 hour.
            WalkStrategyBlackList[strategy] = DateTime.Now.AddHours(1);
        }

        public IWalkStrategy GetStrategy(ILogicSettings logicSettings)
        {
            return WalkStrategyQueue.First(q => !IsWalkingStrategyBlacklisted(q.GetType()));
        }

        //functions for routes map
        private List<List<double>> Route(ISession session, GeoCoordinate start, GeoCoordinate dest)
        {
            var result = new List<List<double>>();

            try
            {
                var web = WebRequest.Create(
                    $"https://maps.googleapis.com/maps/api/directions/json?origin={start.Latitude},{start.Longitude}&destination={dest.Latitude},{dest.Longitude}&mode=walking&units=metric&key={session.LogicSettings.GoogleApiKey}");
                web.Credentials = CredentialCache.DefaultCredentials;
                
                string strResponse;
                using (var response = web.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        Debug.Assert(stream != null, "stream != null");
                        using (var reader = new StreamReader(stream))
                            strResponse = reader.ReadToEnd();
                    }
                }

                var parseObject = JObject.Parse(strResponse);
                result = Points(parseObject["routes"][0]["overview_polyline"]["points"].ToString(), 1e5);
            }
            catch (WebException e)
            {
                session.EventDispatcher.Send(new WarnEvent
                {
                    Message = $"Web Exception: {e.Message}"
                });
            }
            catch (NullReferenceException e)
            {
                session.EventDispatcher.Send(new WarnEvent
                {
                    Message = $"Routing Error: {e.Message}"
                });
            }

            return result;
        }

        public static List<List<double>> Points(string overview, double precision)
        {
            if (string.IsNullOrEmpty(overview))
                throw new ArgumentNullException("Points");

            var polyline = false;
            int index = 0, lat = 0, lng = 0;
            var polylineChars = overview.ToCharArray();
            var result = new List<List<double>>();

            while (index < polylineChars.Length)
            {
                int sum = 0, shifter = 0, nextBits;
                var coordinates = new List<double>();

                do
                {
                    nextBits = polylineChars[index++] - 63;
                    sum |= (nextBits & 0x1f) << shifter;
                    shifter += 5;
                } while (nextBits >= 0x20 && index < polylineChars.Length);

                if (index >= polylineChars.Length && (!polyline || nextBits >= 0x20))
                    break;

                if (!polyline)
                    lat += (sum & 1) == 1 ? ~(sum >> 1) : sum >> 1;
                else
                {
                    lng += (sum & 1) == 1 ? ~(sum >> 1) : sum >> 1;
                    coordinates.Add(lng / precision);
                    coordinates.Add(lat / precision);
                    result.Add(coordinates);
                }

                polyline = !polyline;
            }

            return result;
        }

        private static async Task<List<FortData>> GetPokeStops(ISession session)
        {
            var mapObjects = await session.Client.Map.GetMapObjects();

            // Wasn't sure how to make this pretty. Edit as needed.
            var pokeStops = mapObjects.Item1.MapCells.SelectMany(i => i.Forts)
                .Where(
                    i =>
                        i.Type == FortType.Checkpoint &&
                        i.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime() &&
                        ( // Make sure PokeStop is within max travel distance, unless it's set to 0.
                            LocationUtils.CalculateDistanceInMeters(
                                session.Settings.DefaultLatitude, session.Settings.DefaultLongitude,
                                i.Latitude, i.Longitude) < session.LogicSettings.MaxTravelDistanceInMeters ||
                            session.LogicSettings.MaxTravelDistanceInMeters == 0)
                );

            return pokeStops.ToList();
        }


        public static event GetHumanizeRouteDelegate GetHumanizeRouteEvent;

        protected virtual void OnGetHumanizeRouteEvent(List<GeoCoordinate> route, GeoCoordinate destination, List<FortData> pokeStops)
        {
            GetHumanizeRouteEvent?.Invoke(route, destination, pokeStops);
        }
        //end functions routes map
    }
}