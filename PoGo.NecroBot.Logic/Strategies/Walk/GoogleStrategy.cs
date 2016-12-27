using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GeoCoordinatePortable;
using PoGo.NecroBot.Logic.Model.Google;
using PoGo.NecroBot.Logic.Service;
using PoGo.NecroBot.Logic.State;
using PokemonGo.RocketAPI;
using POGOProtos.Networking.Responses;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.Utils;
using PoGo.NecroBot.Logic.Model;

namespace PoGo.NecroBot.Logic.Strategies.Walk
{
    class GoogleStrategy : BaseWalkStrategy, IWalkStrategy
    {
        private GoogleDirectionsService _googleDirectionsService;

        public GoogleStrategy(Client client) : base(client)
        {
            _googleDirectionsService = null;
        }

        public override string RouteName => "Google Walk";

        public override async Task<PlayerUpdateResponse> Walk(IGeoLocation targetLocation, Func<Task> functionExecutedWhileWalking, ISession session, CancellationToken cancellationToken, double walkSpeed = 0.0)
        {
            GetGoogleInstance(session);

            _minStepLengthInMeters = session.LogicSettings.DefaultStepLength;
            var currentLocation = new GeoCoordinate(_client.CurrentLatitude, _client.CurrentLongitude, _client.CurrentAltitude);
            var destinaionCoordinate = new GeoCoordinate(targetLocation.Latitude, targetLocation.Longitude);

            var googleWalk = _googleDirectionsService.GetDirections(currentLocation, new List<GeoCoordinate>(), destinaionCoordinate);

            if (googleWalk == null)
            {
                return await RedirectToNextFallbackStrategy(session.LogicSettings, targetLocation, functionExecutedWhileWalking, session, cancellationToken, walkSpeed);
            }
            
            base.OnStartWalking(session, targetLocation, googleWalk.Distance);

            List <GeoCoordinate> points = googleWalk.Waypoints;
            return await DoWalk(points, session, functionExecutedWhileWalking, currentLocation, destinaionCoordinate, cancellationToken, walkSpeed);
        }

        private void GetGoogleInstance(ISession session)
        {
            if (_googleDirectionsService == null)
                _googleDirectionsService = new GoogleDirectionsService(session);
        }

        public override double CalculateDistance(double sourceLat, double sourceLng, double destinationLat, double destinationLng, ISession session = null)
        {
            // Too expensive to calculate true distance.
            return 1.5 * base.CalculateDistance(sourceLat, sourceLng, destinationLat, destinationLng);

            /*
            if (session != null)
                GetGoogleInstance(session);

            //Check Google direction service is initialized
            if (_googleDirectionsService != null)
            {
                var googleResult = _googleDirectionsService.GetDirections(new GeoCoordinate(sourceLat, sourceLng), new List<GeoCoordinate>(), new GeoCoordinate(destinationLat, destinationLng));
                if (googleResult == null || googleResult.Directions.status.Equals("OVER_QUERY_LIMIT"))
                {
                    return 1.5 * base.CalculateDistance(sourceLat, sourceLng, destinationLat, destinationLng);
                }
                else
                {
                    //There are two ways to get distance value. One is counting from waypoint list.
                    //The other is getting from google service. The result value is different.

                    ////Count distance from waypoint list
                    //var googleWalk = GoogleWalk.Get(googleResult);
                    //List<GeoCoordinate> points = googleWalk.Waypoints;
                    //double distance = 0;
                    //for (var i = 0; i < points.Count; i++)
                    //{
                    //    if (i > 0)
                    //    {
                    //        distance += LocationUtils.CalculateDistanceInMeters(points[i - 1], points[i]);
                    //    }
                    //}
                    //return distance;

                    //Get distance from Google service
                    return googleResult.GetDistance();
                }
            }
            else
            {
                return 1.5 * base.CalculateDistance(sourceLat, sourceLng, destinationLat, destinationLng);
            }
            */
        }
    }
}