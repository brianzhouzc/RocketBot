#region using directives

using POGOProtos.Enums;

#endregion

namespace PoGo.RocketBot.Logic.Event
{
    public class SnipeScanEvent : IEvent
    {
        public Location Bounds { get; set; }
        public PokemonId PokemonId { get; set; }
        public double Iv { get; set; }
    }
}