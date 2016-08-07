namespace PoGo.RocketBot.Logic.Event
{
    public class UpdatePositionEvent : IEvent
    {
        public double Latitude;
        public double Longitude;
    }
}