using GeoCoordinatePortable;
using PoGo.NecroBot.Logic.Utils;
using POGOProtos.Networking.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.State;
using System.Threading;
using PokemonGo.RocketAPI;
using PoGo.NecroBot.Logic.Interfaces.Configuration;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.Model;
using PoGo.NecroBot.Logic.Event.Gym;

namespace PoGo.NecroBot.Logic.Strategies.Walk
{
    abstract class BaseWalkStrategy : IWalkStrategy
    {
        protected readonly Client _client;

        protected double _currentWalkingSpeed = 0;
        protected const double SpeedDownTo = 10 / 3.6;
        protected double _minStepLengthInMeters = 1.3d;
        protected bool isCancelled = false;
        protected readonly Random _randWalking = new Random();
        protected IWalkStrategy _fallbackStrategy;

        public event UpdatePositionDelegate UpdatePositionEvent;
        public abstract Task<PlayerUpdateResponse> Walk(IGeoLocation targetLocation, Func<Task> functionExecutedWhileWalking, ISession session, CancellationToken cancellationToken, double walkSpeed = 0.0);
        public virtual string RouteName {get;}
        public BaseWalkStrategy(Client client)
        {
            _client = client;
        }
        public void OnStartWalking(ISession session, IGeoLocation desination, double calculatedDistance = 0.0)
        {
            var distance = calculatedDistance;
            if (distance == 0)
            {
                distance = this.CalculateDistance(session.Client.CurrentLatitude, session.Client.CurrentLongitude, desination.Latitude, desination.Longitude);
            }

            if(desination is FortLocation)
            {
                var fortLocation = desination as FortLocation;
                session.EventDispatcher.Send(new FortTargetEvent { Name = desination.Name, Distance = distance, Route = this.RouteName, Type = fortLocation.FortData.Type });
            }
        }

        internal void DoUpdatePositionEvent(double latitude, double longitude)
        {
            UpdatePositionEvent?.Invoke(latitude, longitude);
        }

        /// <summary>
        /// Cell phones Gps systems can't generate accurate GEO, the average best they can is 5 meter.
        /// http://gis.stackexchange.com/questions/43617/what-is-the-maximum-theoretical-accuracy-of-gps
        /// </summary>
        public GeoCoordinate GenerateUnaccurateGeocoordinate(GeoCoordinate geo, double nextWaypointBearing)
        {
            var minBearing = Convert.ToInt32(nextWaypointBearing - 40);
            minBearing = minBearing > 0 ? minBearing : minBearing * -1;
            var maxBearing = Convert.ToInt32(nextWaypointBearing + 40);
            maxBearing = maxBearing < 360 ? maxBearing : 360 - maxBearing;

            var randomBearingDegrees = _randWalking.NextDouble() + _randWalking.Next(Math.Min(minBearing, maxBearing), Math.Max(minBearing, maxBearing));

            var randomDistance = _randWalking.NextDouble() * 3;

            return LocationUtils.CreateWaypoint(geo, randomDistance, randomBearingDegrees);
        }
        
        public Task<PlayerUpdateResponse> RedirectToNextFallbackStrategy(ILogicSettings logicSettings, IGeoLocation targetLocation, Func<Task> functionExecutedWhileWalking, ISession session, CancellationToken cancellationToken, double walkSpeed=0.0)
        {
            // If we need to fall-back, then blacklist current strategy for 1 hour.
            session.Navigation.BlacklistStrategy(this.GetType());

            IWalkStrategy nextStrategy = session.Navigation.GetStrategy(logicSettings);
           
            return nextStrategy.Walk(targetLocation, functionExecutedWhileWalking, session, cancellationToken);
        }

        public async Task<PlayerUpdateResponse> DoWalk(List<GeoCoordinate> points, ISession session, Func<Task> functionExecutedWhileWalking, GeoCoordinate sourceLocation, GeoCoordinate targetLocation, CancellationToken cancellationToken, double walkSpeed = 0.0)
        {
            PlayerUpdateResponse result = null;
            var currentLocation = new GeoCoordinate(_client.CurrentLatitude, _client.CurrentLongitude, _client.CurrentAltitude);

            //filter google defined waypoints and remove those that are too near to the previous ones
            var waypointsDists = new Dictionary<Tuple<GeoCoordinate, GeoCoordinate>, double>();
            var minWaypointsDistance = RandomizeStepLength(_minStepLengthInMeters);

            for (var i = 0; i < points.Count; i++)
            {
                if (i > 0)
                {
                    var dist = LocationUtils.CalculateDistanceInMeters(points[i - 1], points[i]);
                    waypointsDists[new Tuple<GeoCoordinate, GeoCoordinate>(points[i - 1], points[i])] = dist;
                }
            }

            var tooNearPoints = waypointsDists.Where(kvp => kvp.Value < minWaypointsDistance).Select(kvp => kvp.Key.Item1).ToList();
            foreach (var tooNearPoint in tooNearPoints)
            {
                points.Remove(tooNearPoint);
            }
            if (points.Any()) //check if first waypoint is the current location (this is what google returns), in such case remove it!
            {
                var firstStep = points.First();
                if (firstStep == currentLocation)
                    points.Remove(points.First());
            }

            var walkedPointsList = new List<GeoCoordinate>();
            foreach (var nextStep in points)
            {
                currentLocation = new GeoCoordinate(_client.CurrentLatitude, _client.CurrentLongitude);
                if (_currentWalkingSpeed <= 0)
                    _currentWalkingSpeed = session.LogicSettings.WalkingSpeedInKilometerPerHour;
                if (session.LogicSettings.UseWalkingSpeedVariant && walkSpeed == 0)
                    _currentWalkingSpeed = session.Navigation.VariantRandom(session, _currentWalkingSpeed);

                var speedInMetersPerSecond = (walkSpeed > 0 ? walkSpeed : _currentWalkingSpeed) / 3.6;
                var nextStepBearing = LocationUtils.DegreeBearing(currentLocation, nextStep);
                //particular steps are limited by minimal length, first step is calculated from the original speed per second (distance in 1s)
                var nextStepDistance = Math.Max(RandomizeStepLength(_minStepLengthInMeters), speedInMetersPerSecond);
                
                var waypoint = LocationUtils.CreateWaypoint(currentLocation, nextStepDistance, nextStepBearing);
                walkedPointsList.Add(waypoint);

                var previousLocation = currentLocation; //store the current location for comparison and correction purposes
                var requestSendDateTime = DateTime.Now;
                result = await LocationUtils.UpdatePlayerLocationWithAltitude(session, waypoint, (float)speedInMetersPerSecond);

                var realDistanceToTarget = LocationUtils.CalculateDistanceInMeters(currentLocation, targetLocation);
                if (realDistanceToTarget < 30)
                    break;

                do
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var msToPositionChange = (DateTime.Now - requestSendDateTime).TotalMilliseconds;
                    currentLocation = new GeoCoordinate(_client.CurrentLatitude, _client.CurrentLongitude);
                    var currentDistanceToWaypoint = LocationUtils.CalculateDistanceInMeters(currentLocation, nextStep);
                    realDistanceToTarget = LocationUtils.CalculateDistanceInMeters(currentLocation, targetLocation);
                    
                    var realSpeedinMperS = nextStepDistance / (msToPositionChange / 1000);
                    var realDistanceWalked = LocationUtils.CalculateDistanceInMeters(previousLocation, currentLocation);
                    //if the real calculated speed is lower than the one expected, we will raise the speed for the following step
                    double speedRaise = 0;
                    if (realSpeedinMperS < speedInMetersPerSecond)
                        speedRaise = speedInMetersPerSecond - realSpeedinMperS;
                    double distanceRaise = 0;
                    if (realDistanceWalked < nextStepDistance)
                        distanceRaise = nextStepDistance - realDistanceWalked;
                    
                    var realDistanceToTargetSpeedDown = LocationUtils.CalculateDistanceInMeters(currentLocation, targetLocation);
                    if (realDistanceToTargetSpeedDown < 40)
                        if (speedInMetersPerSecond > SpeedDownTo)
                            speedInMetersPerSecond = SpeedDownTo;

                    if (session.LogicSettings.UseWalkingSpeedVariant && walkSpeed ==0)
                    {
                        _currentWalkingSpeed = session.Navigation.VariantRandom(session, _currentWalkingSpeed);
                        speedInMetersPerSecond = _currentWalkingSpeed / 3.6;
                    }
                    speedInMetersPerSecond += speedRaise;
                   if(walkSpeed > 0)
                    {
                        speedInMetersPerSecond = walkSpeed / 3.6;
                    }
                    nextStepBearing = LocationUtils.DegreeBearing(currentLocation, nextStep);

                    //setting next step distance is limited by the target and the next waypoint distance (we don't want to miss them)
                    //also the minimal step length is used as we don't want to spend minutes jumping by cm lengths
                    nextStepDistance = Math.Min(Math.Min(realDistanceToTarget, currentDistanceToWaypoint),
                                            //also add the distance raise (bot overhead corrections) to the normal step length
                                            Math.Max(RandomizeStepLength(_minStepLengthInMeters) + distanceRaise, (msToPositionChange / 1000) * speedInMetersPerSecond) + distanceRaise);
                    
                    waypoint = LocationUtils.CreateWaypoint(currentLocation, nextStepDistance, nextStepBearing);
                    walkedPointsList.Add(waypoint);

                    previousLocation = currentLocation; //store the current location for comparison and correction purposes
                    requestSendDateTime = DateTime.Now;
                    result = await LocationUtils.UpdatePlayerLocationWithAltitude(session, waypoint, (float)speedInMetersPerSecond);

                    UpdatePositionEvent?.Invoke(waypoint.Latitude, waypoint.Longitude);

                    if (functionExecutedWhileWalking != null)
                        await functionExecutedWhileWalking(); // look for pokemon
                } while (LocationUtils.CalculateDistanceInMeters(currentLocation, nextStep) >= 2);

                UpdatePositionEvent?.Invoke(nextStep.Latitude, nextStep.Longitude);
            }

            return result;
        }


        /// <summary>
        /// Basic step length is given but we want to randomize it a bit to avoid usage of steps of the same length
        /// </summary>
        /// <param name="initialStepLength">Length of the step in meters</param>
        /// <returns></returns>
        protected double RandomizeStepLength(double initialStepLength)
        {
            var randFactor = 0.3d;
            var initialStepLengthMm = initialStepLength * 1000;
            var randomMin = (int)(initialStepLengthMm * (1 - randFactor));
            var randomMax = (int)(initialStepLengthMm * (1 + randFactor));
            var randStep = _randWalking.Next(randomMin, randomMax);
            return randStep / 1000d;
        }

        public virtual double CalculateDistance(double sourceLat, double sourceLng, double destinationLat, double destinationLng, ISession session = null)
        {
            return LocationUtils.CalculateDistanceInMeters(sourceLat, sourceLng, destinationLat, destinationLng);
        }
    }    
}
