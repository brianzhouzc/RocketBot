using System.Collections.Generic;
using POGOProtos.Map.Fort;

namespace NecroBot2.Logic.Event
{
    public class OptimizeRouteEvent : IEvent
    {
        public List<FortData> OptimizedRoute { get; set; }
    }
}