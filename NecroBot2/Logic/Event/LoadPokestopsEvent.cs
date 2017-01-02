using System.Collections.Generic;
using POGOProtos.Map.Fort;
using PoGo.NecroBot.Logic.Event;

namespace NecroBot2.Logic.Event
{
    public class LoadPokestopsEvent : IEvent
    {
        public List<FortData> PokeStops { get; set; }
    }
}