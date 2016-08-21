using System.Collections.Generic;
using GeoCoordinatePortable;

namespace PokemonGo.RocketBot.Logic.Event
{
    public class GetHumanizeRouteEvent : IEvent
    {
        public GeoCoordinate Destination;
        public List<GeoCoordinate> Route;
    }
}