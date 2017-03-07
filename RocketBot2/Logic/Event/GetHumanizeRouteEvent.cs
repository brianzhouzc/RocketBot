using System.Collections.Generic;
using GeoCoordinatePortable;
using PoGo.NecroBot.Logic.Event;

namespace RocketBot2.Logic.Event
{
    public class GetHumanizeRouteEvent : IEvent
    {
        public List<GeoCoordinate> Points;
    }
}