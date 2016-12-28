#region using directives

using PoGo.NecroBot.Logic.Event;
using POGOProtos.Enums;

#endregion

namespace NecroBot2.Logic.Event
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