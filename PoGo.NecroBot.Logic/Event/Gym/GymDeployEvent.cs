using POGOProtos.Enums;

namespace PoGo.NecroBot.Logic.Event.Gym
{
    public class GymDeployEvent : IEvent
    {
        public string Name { get; internal set; }
        public PokemonId PokemonId { get; internal set; }
    }
}