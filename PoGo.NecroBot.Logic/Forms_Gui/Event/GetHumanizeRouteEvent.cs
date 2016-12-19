using System.Collections.Generic;
using GeoCoordinatePortable;

namespace PoGo.NecroBot.Logic.Forms_Gui.Event
{
    public class GetHumanizeRouteEvent : IEvent
    {
        public GeoCoordinate Destination;
        public List<GeoCoordinate> Route;
    }
}