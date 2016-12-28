using PoGo.NecroBot.Logic.Event;

namespace NecroBot2.Logic.Event
{
    public class ErrorEvent : IEvent
    {
        public string Message = "";

        public override string ToString()
        {
            return Message;
        }
    }
}