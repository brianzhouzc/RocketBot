using System.Collections.Generic;
using POGOProtos.Map.Fort;

namespace PokemonGo.RocketBot.Logic.Event
{
    public class OptimizeRouteEvent : IEvent
    {
        public List<FortData> OptimizedRoute { get; set; }
    }
}