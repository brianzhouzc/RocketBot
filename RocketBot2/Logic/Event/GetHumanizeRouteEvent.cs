using System.Collections.Generic;
using GeoCoordinatePortable;
using PoGo.NecroBot.Logic.Event;
using POGOProtos.Map.Fort;

namespace RocketBot2.Logic.Event
{
    public class GetHumanizeRouteEvent : IEvent
    {
        public List<GeoCoordinate> Route;
    }
}