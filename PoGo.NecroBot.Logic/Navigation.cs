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

#endregion

namespace PoGo.NecroBot.Logic
{
    public delegate void GetHumanizeRouteDelegate(List<GeoCoordinate> route, GeoCoordinate destination);

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

        public static event GetHumanizeRouteDelegate GetHumanizeRouteEvent;

        protected virtual void OnGetHumanizeRouteEvent(List<GeoCoordinate> route, GeoCoordinate destination)
        {
            GetHumanizeRouteEvent?.Invoke(route, destination);
        }
    }
}