namespace PoGo.NecroBot.Logic.Forms_Gui.Event
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