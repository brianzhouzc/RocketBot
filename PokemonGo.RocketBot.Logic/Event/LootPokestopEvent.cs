using POGOProtos.Map.Fort;

namespace PokemonGo.RocketBot.Logic.Event
{
    public class LootPokestopEvent : IEvent
    {
        public FortData Pokestop;
    }
}