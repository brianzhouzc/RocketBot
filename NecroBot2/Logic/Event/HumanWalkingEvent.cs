using PoGo.NecroBot.Logic.Event;

namespace NecroBot2.Logic.Event
{
    public class HumanWalkingEvent : IEvent
    {
        public double CurrentWalkingSpeed;
        public double OldWalkingSpeed;
    }
}