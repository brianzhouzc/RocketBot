namespace PokemonGo.RocketBot.Logic.Event
{
    public class FortFailedEvent : IEvent
    {
        public bool Looted;
        public int Max;
        public string Name;
        public int Try;
    }
}