using System.Collections.Generic;
using System.Device.Location;
using PoGo.NecroBot.Logic.Event;

namespace RocketBot2.Logic.Event
{
    public class GetHumanizeRouteEvent : IEvent
    {
        public List<GeoCoordinate> Points;
    }
}