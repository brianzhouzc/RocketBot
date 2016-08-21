namespace PokemonGo.RocketBot.Logic.Event
{
    public class HumanWalkingEvent : IEvent
    {
        public double CurrentWalkingSpeed;
        public double OldWalkingSpeed;
    }
}