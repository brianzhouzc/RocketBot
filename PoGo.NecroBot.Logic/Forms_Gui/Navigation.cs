#region using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GeoCoordinatePortable;
using Newtonsoft.Json.Linq;
using PokemonGo.RocketAPI;
using PoGo.NecroBot.Logic.Forms_Gui.Event;
using PoGo.NecroBot.Logic.Forms_Gui.State;
using PoGo.NecroBot.Logic.Forms_Gui.Utils;
using POGOProtos.Networking.Responses;

#endregion

namespace PoGo.NecroBot.Logic.Forms_Gui
{
    public delegate void UpdatePositionDelegate(double lat, double lng);

    public delegate void GetHumanizeRouteDelegate(List<GeoCoordinate> route, GeoCoordinate destination);

    public class Navigation
    {
        private const double SpeedDownTo = 10 / 3.6;
        private readonly Client _client;
        private readonly Random _randWalking = new Random();
        private double _currentWalkingSpeed;
        private DateTime _lastMajorVariantWalkingSpeed;
        private DateTime _nextMajorVariantWalkingSpeed;

        public Navigation(Client client)
        {
            _client = client;
        }

        private double MajorWalkingSpeedVariant(ISession session)
        {
            if (_lastMajorVariantWalkingSpeed == DateTime.MinValue && _nextMajorVariantWalkingSpeed == DateTime.MinValue)
            {
                var minutes = _randWalking.NextDouble() * (2 - 6) + 2;
                _lastMajorVariantWalkingSpeed = DateTime.Now;
                _nextMajorVariantWalkingSpeed = _lastMajorVariantWalkingSpeed.AddMinutes(minutes);
                _currentWalkingSpeed = session.LogicSettings.WalkingSpeedInKilometerPerHour;
            }
            else if (_nextMajorVariantWalkingSpeed.Ticks < DateTime.Now.Ticks)
            {
                var oldWalkingSpeed = _currentWalkingSpeed;

                var randomMin = session.LogicSettings.WalkingSpeedInKilometerPerHour -
                                session.LogicSettings.WalkingSpeedVariant;
                var randomMax = session.LogicSettings.WalkingSpeedInKilometerPerHour +
                                session.LogicSettings.WalkingSpeedVariant;
                _currentWalkingSpeed = _randWalking.NextDouble() * (randomMax - randomMin) + randomMin;

                var minutes = _randWalking.NextDouble() * (2 - 6) + 2;
                _lastMajorVariantWalkingSpeed = DateTime.Now;
                _nextMajorVariantWalkingSpeed = _lastMajorVariantWalkingSpeed.AddMinutes(minutes);

                session.EventDispatcher.Send(new HumanWalkingEvent
                {
                    OldWalkingSpeed = oldWalkingSpeed,
                    CurrentWalkingSpeed = _currentWalkingSpeed
                });
            }

            return _currentWalkingSpeed / 3.6;
        }

        private double MinorWalkingSpeedVariant(ISession session)
        {
            if (_randWalking.Next(1, 10) > 5) //Random change or no variant speed
            {
                var oldWalkingSpeed = _currentWalkingSpeed;

                if (_randWalking.Next(1, 10) > 5) //Random change upper or lower variant speed
                {
                    var randomMax = session.LogicSettings.WalkingSpeedInKilometerPerHour +
                                    session.LogicSettings.WalkingSpeedVariant + 0.5;

                    _currentWalkingSpeed += _randWalking.NextDouble() * (0.01 - 0.09) + 0.01;
                    if (_currentWalkingSpeed > randomMax)
                        _currentWalkingSpeed = randomMax;
                }
                else
                {
                    var randomMin = session.LogicSettings.WalkingSpeedInKilometerPerHour -
                                    session.LogicSettings.WalkingSpeedVariant - 0.5;

                    _currentWalkingSpeed -= _randWalking.NextDouble() * (0.01 - 0.09) + 0.01;
                    if (_currentWalkingSpeed < randomMin)
                        _currentWalkingSpeed = randomMin;
                }

                if (Math.Abs(oldWalkingSpeed - _currentWalkingSpeed) > 0)
                {
                    session.EventDispatcher.Send(new HumanWalkingEvent
                    {
                        OldWalkingSpeed = oldWalkingSpeed,
                        CurrentWalkingSpeed = _currentWalkingSpeed
                    });
                }
            }

            return _currentWalkingSpeed / 3.6;
        }

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

        public async Task<PlayerUpdateResponse> Move(GeoCoordinate targetLocation,
            Func<Task<bool>> functionExecutedWhileWalking,
            ISession session,
            CancellationToken cancellationToken, bool forceDisableHumanWalking = false)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!session.LogicSettings.DisableHumanWalking && !forceDisableHumanWalking)
            {
                PlayerUpdateResponse result = null;
                var points = new List<GeoCoordinate>();
                var route = Route(session,
                    new GeoCoordinate(
                        _client.CurrentLatitude,
                        _client.CurrentLongitude,
                        _client.CurrentAltitude),
                    targetLocation);

                foreach (var item in route)
                    points.Add(new GeoCoordinate(item.ToArray()[1], item.ToArray()[0]));

                OnGetHumanizeRouteEvent(points, targetLocation);

                for (var i = 0; i < points.Count; i++)
                {
                    var speedInMetersPerSecond = session.LogicSettings.UseWalkingSpeedVariant
                        ? MajorWalkingSpeedVariant(session)
                        : session.LogicSettings.WalkingSpeedInKilometerPerHour / 3.6;
                    var sourceLocation = new GeoCoordinate(_client.CurrentLatitude, _client.CurrentLongitude);

                    var nextWaypointBearing = LocationUtils.DegreeBearing(sourceLocation, points.ToArray()[i]);
                    var nextWaypointDistance = speedInMetersPerSecond;
                    var waypoint = LocationUtils.CreateWaypoint(sourceLocation, nextWaypointDistance,
                        nextWaypointBearing);

                    var requestSendDateTime = DateTime.Now;
                    result =
                        await
                            _client.Player.UpdatePlayerLocation(waypoint.Latitude, waypoint.Longitude,
                                waypoint.Altitude, (float) waypoint.Speed);

                    UpdatePositionEvent?.Invoke(waypoint.Latitude, waypoint.Longitude);

                    var realDistanceToTarget = LocationUtils.CalculateDistanceInMeters(sourceLocation, targetLocation);
                    if (realDistanceToTarget < 30)
                        break;

                    do
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var millisecondsUntilGetUpdatePlayerLocationResponse =
                            (DateTime.Now - requestSendDateTime).TotalMilliseconds;

                        sourceLocation = new GeoCoordinate(_client.CurrentLatitude, _client.CurrentLongitude);
                        var currentDistanceToTarget = LocationUtils.CalculateDistanceInMeters(sourceLocation,
                            points.ToArray()[i]);

                        var realDistanceToTargetSpeedDown = LocationUtils.CalculateDistanceInMeters(sourceLocation,
                            targetLocation);
                        if (realDistanceToTargetSpeedDown < 40)
                            if (speedInMetersPerSecond > SpeedDownTo)
                                speedInMetersPerSecond = SpeedDownTo;

                        if (session.LogicSettings.UseWalkingSpeedVariant)
                            speedInMetersPerSecond = MinorWalkingSpeedVariant(session);

                        nextWaypointDistance = Math.Min(currentDistanceToTarget,
                            millisecondsUntilGetUpdatePlayerLocationResponse / 1000 * speedInMetersPerSecond);
                        nextWaypointBearing = LocationUtils.DegreeBearing(sourceLocation, points.ToArray()[i]);
                        waypoint = LocationUtils.CreateWaypoint(sourceLocation, nextWaypointDistance,
                            nextWaypointBearing);

                        requestSendDateTime = DateTime.Now;
                        result =
                            await
                                _client.Player.UpdatePlayerLocation(waypoint.Latitude, waypoint.Longitude,
                                    waypoint.Altitude,(float) waypoint.Speed);

                        UpdatePositionEvent?.Invoke(waypoint.Latitude, waypoint.Longitude);

                        if (functionExecutedWhileWalking != null)
                            await functionExecutedWhileWalking(); // look for pokemon
                    } while (LocationUtils.CalculateDistanceInMeters(sourceLocation, points.ToArray()[i]) >= 2);

                    UpdatePositionEvent?.Invoke(points.ToArray()[i].Latitude, points.ToArray()[i].Longitude);
                }

                return result;
            }
            else
            {
                var sourceLocation = new GeoCoordinate(_client.CurrentLatitude, _client.CurrentLongitude);

                var nextWaypointBearing = LocationUtils.DegreeBearing(sourceLocation, targetLocation);

                var nextWaypointDistance = session.LogicSettings.UseWalkingSpeedVariant
                    ? MajorWalkingSpeedVariant(session)
                    : session.LogicSettings.WalkingSpeedInKilometerPerHour / 3.6;
                ;
                ;
                var waypoint = LocationUtils.CreateWaypoint(sourceLocation, nextWaypointDistance, nextWaypointBearing);

                //Initial walking
                var requestSendDateTime = DateTime.Now;
                var result =
                    await
                        _client.Player.UpdatePlayerLocation(waypoint.Latitude, waypoint.Longitude,
                            waypoint.Altitude, (float) waypoint.Speed);

                UpdatePositionEvent?.Invoke(waypoint.Latitude, waypoint.Longitude);

                do
                {
                    var speedInMetersPerSecond = session.LogicSettings.UseWalkingSpeedVariant
                        ? MajorWalkingSpeedVariant(session)
                        : session.LogicSettings.WalkingSpeedInKilometerPerHour / 3.6;
                    cancellationToken.ThrowIfCancellationRequested();

                    var millisecondsUntilGetUpdatePlayerLocationResponse =
                        (DateTime.Now - requestSendDateTime).TotalMilliseconds;

                    sourceLocation = new GeoCoordinate(_client.CurrentLatitude, _client.CurrentLongitude);
                    var currentDistanceToTarget = LocationUtils.CalculateDistanceInMeters(sourceLocation,
                        targetLocation);

                    if (currentDistanceToTarget < 40)
                    {
                        if (speedInMetersPerSecond > SpeedDownTo)
                        {
                            //Logger.Write("We are within 40 meters of the target. Speeding down to 10 km/h to not pass the target.", LogLevel.Info);
                            speedInMetersPerSecond = SpeedDownTo;
                        }
                    }

                    nextWaypointDistance = Math.Min(currentDistanceToTarget,
                        millisecondsUntilGetUpdatePlayerLocationResponse / 1000 * speedInMetersPerSecond);
                    nextWaypointBearing = LocationUtils.DegreeBearing(sourceLocation, targetLocation);
                    waypoint = LocationUtils.CreateWaypoint(sourceLocation, nextWaypointDistance,
                        nextWaypointBearing);

                    requestSendDateTime = DateTime.Now;
                    result =
                        await
                            _client.Player.UpdatePlayerLocation(waypoint.Latitude, waypoint.Longitude,
                                waypoint.Altitude, (float) waypoint.Speed);

                    UpdatePositionEvent?.Invoke(waypoint.Latitude, waypoint.Longitude);


                    if (functionExecutedWhileWalking != null)
                        await functionExecutedWhileWalking(); // look for pokemon
                } while (LocationUtils.CalculateDistanceInMeters(sourceLocation, targetLocation) >= 30);

                return result;
            }
        }

        public async Task<PlayerUpdateResponse> HumanPathWalking(GpxReader.Trkpt trk,
            Func<Task<bool>> functionExecutedWhileWalking,
            ISession session,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            //PlayerUpdateResponse result = null;

            var targetLocation = new GeoCoordinate(Convert.ToDouble(trk.Lat, CultureInfo.InvariantCulture),
                Convert.ToDouble(trk.Lon, CultureInfo.InvariantCulture));
            var speedInMetersPerSecond = session.LogicSettings.UseWalkingSpeedVariant
                ? MajorWalkingSpeedVariant(session)
                : session.LogicSettings.WalkingSpeedInKilometerPerHour / 3.6;
            var sourceLocation = new GeoCoordinate(_client.CurrentLatitude, _client.CurrentLongitude);
            LocationUtils.CalculateDistanceInMeters(sourceLocation, targetLocation);
            var nextWaypointBearing = LocationUtils.DegreeBearing(sourceLocation, targetLocation);
            var nextWaypointDistance = speedInMetersPerSecond;
            var waypoint = LocationUtils.CreateWaypoint(sourceLocation, nextWaypointDistance, nextWaypointBearing,
                Convert.ToDouble(trk.Ele, CultureInfo.InvariantCulture));
            var requestSendDateTime = DateTime.Now;
            var result =
                await
                    _client.Player.UpdatePlayerLocation(waypoint.Latitude, waypoint.Longitude, waypoint.Altitude,(float) waypoint.Speed);

            UpdatePositionEvent?.Invoke(waypoint.Latitude, waypoint.Longitude);

            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                var millisecondsUntilGetUpdatePlayerLocationResponse =
                    (DateTime.Now - requestSendDateTime).TotalMilliseconds;

                sourceLocation = new GeoCoordinate(_client.CurrentLatitude, _client.CurrentLongitude);
                var currentDistanceToTarget = LocationUtils.CalculateDistanceInMeters(sourceLocation, targetLocation);

                //if (currentDistanceToTarget < 40)
                //{
                //    if (speedInMetersPerSecond > SpeedDownTo)
                //    {
                //        //Logger.Write("We are within 40 meters of the target. Speeding down to 10 km/h to not pass the target.", LogLevel.Info);
                //        speedInMetersPerSecond = SpeedDownTo;
                //    }
                //}

                if (session.LogicSettings.UseWalkingSpeedVariant)
                    speedInMetersPerSecond = MinorWalkingSpeedVariant(session);

                nextWaypointDistance = Math.Min(currentDistanceToTarget,
                    millisecondsUntilGetUpdatePlayerLocationResponse / 1000 * speedInMetersPerSecond);
                nextWaypointBearing = LocationUtils.DegreeBearing(sourceLocation, targetLocation);
                waypoint = LocationUtils.CreateWaypoint(sourceLocation, nextWaypointDistance, nextWaypointBearing);

                requestSendDateTime = DateTime.Now;
                result =
                    await
                        _client.Player.UpdatePlayerLocation(waypoint.Latitude, waypoint.Longitude,
                            waypoint.Altitude,(float) waypoint.Speed);

                UpdatePositionEvent?.Invoke(waypoint.Latitude, waypoint.Longitude);

                if (functionExecutedWhileWalking != null)
                    await functionExecutedWhileWalking(); // look for pokemon & hit stops
            } while (LocationUtils.CalculateDistanceInMeters(sourceLocation, targetLocation) >= 30);

            return result;
        }

        public event UpdatePositionDelegate UpdatePositionEvent;
        public static event GetHumanizeRouteDelegate GetHumanizeRouteEvent;

        protected virtual void OnGetHumanizeRouteEvent(List<GeoCoordinate> route, GeoCoordinate destination)
        {
            GetHumanizeRouteEvent?.Invoke(route, destination);
        }
    }
}