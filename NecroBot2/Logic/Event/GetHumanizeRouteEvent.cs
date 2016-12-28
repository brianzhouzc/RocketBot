using System.Collections.Generic;
using GeoCoordinatePortable;

namespace NecroBot2.Logic.Event
{
    public class GetHumanizeRouteEvent : IEvent
    {
        public GeoCoordinate Destination;
        public List<GeoCoordinate> Route;
    }
}