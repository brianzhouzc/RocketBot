#region using directives

using POGOProtos.Enums;

#endregion

namespace PokemonGo.RocketBot.Logic.Event
{
    public class EggHatchedEvent : IEvent
    {
        public int Cp;
        public ulong Id;
        public double Level;
        public int MaxCp;
        public double Perfection;
        public PokemonId PokemonId;
    }
}