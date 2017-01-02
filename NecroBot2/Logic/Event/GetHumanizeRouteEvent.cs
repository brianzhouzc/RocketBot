using System.Collections.Generic;
using GeoCoordinatePortable;
using PoGo.NecroBot.Logic.Event;

namespace NecroBot2.Logic.Event
{
    public class GetHumanizeRouteEvent : IEvent
    {
        public GeoCoordinate Destination;
        public List<GeoCoordinate> Route;
    }
}