using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GeoCoordinatePortable;
using PoGo.NecroBot.Logic.Service;
using PoGo.NecroBot.Logic.State;
using PokemonGo.RocketAPI;
using POGOProtos.Networking.Responses;
using PoGo.NecroBot.Logic.Model.Mapzen;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.Utils;
using PoGo.NecroBot.Logic.Model;

namespace PoGo.NecroBot.Logic.Strategies.Walk
{
    class MapzenNavigationStrategy : BaseWalkStrategy, IWalkStrategy
    {
        private MapzenDirectionsService _mapzenDirectionsService;

        public MapzenNavigationStrategy(Client client) : base(client)
        {
            _mapzenDirectionsService = null;
        }

        public override string RouteName => "Mapzen Walk";

        public override async Task<PlayerUpdateResponse> Walk(IGeoLocation targetLocation, Func<Task> functionExecutedWhileWalking, ISession session, CancellationToken cancellationToken, double walkSpeed = 0.0)
        {
            GetMapzenInstance(session);
            var sourceLocation = new GeoCoordinate(_client.CurrentLatitude, _client.CurrentLongitude, _client.CurrentAltitude);
            var destinaionCoordinate = new GeoCoordinate(targetLocation.Latitude, targetLocation.Longitude);
            MapzenWalk mapzenWalk = _mapzenDirectionsService.GetDirections(sourceLocation, destinaionCoordinate);

            if (mapzenWalk == null)
            {
                return await RedirectToNextFallbackStrategy(session.LogicSettings, targetLocation, functionExecutedWhileWalking, session, cancellationToken);
            }
            
            base.OnStartWalking(session, targetLocation, mapzenWalk.Distance);
            List<GeoCoordinate> points = mapzenWalk.Waypoints;
            return await DoWalk(points, session, functionExecutedWhileWalking, sourceLocation, destinaionCoordinate, cancellationToken, walkSpeed);
        }

        private void GetMapzenInstance(ISession session)
        {
            if (_mapzenDirectionsService == null)
                _mapzenDirectionsService = new MapzenDirectionsService(session);
        }

        public override double CalculateDistance(double sourceLat, double sourceLng, double destinationLat, double destinationLng, ISession session = null)
        {
            // Too expensive to calculate true distance.
            return 1.5 * base.CalculateDistance(sourceLat, sourceLng, destinationLat, destinationLng);

            /*
            if (session != null)
                GetMapzenInstance(session);

            if (_mapzenDirectionsService != null)
            {
                var mapzenResult = _mapzenDirectionsService.GetDirections(new GeoCoordinate(sourceLat, sourceLng), new GeoCoordinate(destinationLat, destinationLng));
                if (string.IsNullOrEmpty(mapzenResult) || mapzenResult.StartsWith("<?xml version=\"1.0\"") || mapzenResult.Contains("error"))
                {
                    return 1.5 * base.CalculateDistance(sourceLat, sourceLng, destinationLat, destinationLng);
                }
                else
                {
                    var mapzenWalk = MapzenWalk.Get(mapzenResult);
                    return mapzenWalk.Distance;
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
