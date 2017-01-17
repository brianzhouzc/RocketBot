namespace PoGo.NecroBot.Logic.Event.Gym
{
    public class GymWalkToTargetEvent : IEvent
    {
        public string Name { get; internal set; }
        public double Distance { get; internal set; }
        public double Longitude { get; internal set; }
        public double Latitude { get; internal set; }
    }
}