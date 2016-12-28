using PoGo.NecroBot.Logic.Event;

namespace NecroBot2.Logic.Event
{
    public class EggIncubatorStatusEvent : IEvent
    {
        public string IncubatorId;
        public double KmRemaining;
        public double KmToWalk;
        public ulong PokemonId;
        public bool WasAddedNow;
        public double KmWalked => KmToWalk - KmRemaining;
    }
}