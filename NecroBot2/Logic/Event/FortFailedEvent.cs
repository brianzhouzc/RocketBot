using PoGo.NecroBot.Logic.Event;

namespace NecroBot2.Logic.Event
{
    public class FortFailedEvent : IEvent
    {
        public bool Looted;
        public int Max;
        public string Name;
        public int Try;
    }
}