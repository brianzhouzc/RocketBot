namespace PoGo.NecroBot.Logic.Event.Gym
{
    public class GymErrorUnset : IEvent
    {
        public string GymName { get; internal set; }
    }
}
