using PoGo.NecroBot.Logic.Event;
using POGOProtos.Map.Fort;

namespace RocketBot2.Logic.Event
{
    public class LootPokestopEvent : IEvent
    {
        public FortData Pokestop;
    }
}