using System.Collections.Generic;
using GeoCoordinatePortable;
using PoGo.NecroBot.Logic.Event;
using POGOProtos.Map.Fort;

namespace NecroBot2.Logic.Event
{
    public class GetHumanizeRouteEvent : IEvent
    {
        public GeoCoordinate Destination;
        public List<GeoCoordinate> Route;
        public List<FortData> pokeStops;
    }
}