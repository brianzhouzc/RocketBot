namespace PokemonGo.RocketBot.Logic.Event
{
    public class SnipeEvent : IEvent
    {
        public string Message = "";

        public override string ToString()
        {
            return Message;
        }
    }
}