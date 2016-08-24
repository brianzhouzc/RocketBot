namespace PokemonGo.RocketBot.Logic.Event
{
    public class NoticeEvent : IEvent
    {
        public string Message = "";

        public override string ToString()
        {
            return Message;
        }
    }
}