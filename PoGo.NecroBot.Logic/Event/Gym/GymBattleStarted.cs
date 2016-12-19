namespace PoGo.NecroBot.Logic.Event.Gym
{
    public class GymBattleStarted : IEvent
    {
        public string GymName { get; internal set; }
    }
}
