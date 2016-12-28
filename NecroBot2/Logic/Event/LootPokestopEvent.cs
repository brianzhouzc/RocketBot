using POGOProtos.Map.Fort;

namespace NecroBot2.Logic.Event
{
    public class LootPokestopEvent : IEvent
    {
        public FortData Pokestop;
    }
}