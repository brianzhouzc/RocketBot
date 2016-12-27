using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GeoCoordinatePortable;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Utils;
using PokemonGo.RocketAPI;
using POGOProtos.Networking.Responses;
using PoGo.NecroBot.Logic.Model;

namespace PoGo.NecroBot.Logic.Strategies.Walk
{
    class FlyStrategy : BaseWalkStrategy
    {
        public FlyStrategy(Client client)  : base(client)
        {
        }

        public override string RouteName => "Necrobot Flying";


        public override  async Task<PlayerUpdateResponse> Walk(IGeoLocation targetLocation, Func<Task> functionExecutedWhileWalking, ISession session, CancellationToken cancellationToken, double walkSpeed = 0.0)
        {
            var curLocation = new GeoCoordinate(_client.CurrentLatitude, _client.CurrentLongitude);
            var destinaionCoordinate = new GeoCoordinate(targetLocation.Latitude, targetLocation.Longitude);

            var dist = LocationUtils.CalculateDistanceInMeters(curLocation, destinaionCoordinate);
            if (dist >= 100)
            {
                var nextWaypointDistance = dist * 70 / 100;
                var nextWaypointBearing = LocationUtils.DegreeBearing(curLocation, destinaionCoordinate);

                var waypoint = LocationUtils.CreateWaypoint(curLocation, nextWaypointDistance, nextWaypointBearing);
                var sentTime = DateTime.Now;

                var result = await LocationUtils.UpdatePlayerLocationWithAltitude(session, waypoint, 0); // We are setting speed to 0, so it will be randomly generated speed.
                base.DoUpdatePositionEvent(waypoint.Latitude, waypoint.Longitude);

                do
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var millisecondsUntilGetUpdatePlayerLocationResponse =
                        (DateTime.Now - sentTime).TotalMilliseconds;

                    curLocation = new GeoCoordinate(_client.CurrentLatitude, _client.CurrentLongitude);
                    var currentDistanceToTarget = LocationUtils.CalculateDistanceInMeters(curLocation, destinaionCoordinate);

                    dist = LocationUtils.CalculateDistanceInMeters(curLocation, destinaionCoordinate);

                    if (dist >= 100)
                        nextWaypointDistance = dist * 70 / 100;
                    else
                        nextWaypointDistance = dist;

                    nextWaypointBearing = LocationUtils.DegreeBearing(curLocation, destinaionCoordinate);
                    waypoint = LocationUtils.CreateWaypoint(curLocation, nextWaypointDistance, nextWaypointBearing);
                    sentTime = DateTime.Now;
                    result = await LocationUtils.UpdatePlayerLocationWithAltitude(session, waypoint, 0);  // We are setting speed to 0, so it will be randomly generated speed.
                    base.DoUpdatePositionEvent(waypoint.Latitude, waypoint.Longitude);


                    if (functionExecutedWhileWalking != null)
                        await functionExecutedWhileWalking(); // look for pokemon
                } while (LocationUtils.CalculateDistanceInMeters(curLocation, destinaionCoordinate) >= 10);
                return result;
            }
            else
            {
                var result = await LocationUtils.UpdatePlayerLocationWithAltitude(session, targetLocation.ToGeoCoordinate(), 0);  // We are setting speed to 0, so it will be randomly generated speed.
                base.DoUpdatePositionEvent(targetLocation.Latitude, targetLocation.Longitude);
                if (functionExecutedWhileWalking != null)
                    await functionExecutedWhileWalking(); // look for pokemon
                return result;
            }
        }

        public override double CalculateDistance(double sourceLat, double sourceLng, double destinationLat, double destinationLng, ISession session = null)
        {
            return LocationUtils.CalculateDistanceInMeters(sourceLat, sourceLng, destinationLat, destinationLng);
        }
    }
}
