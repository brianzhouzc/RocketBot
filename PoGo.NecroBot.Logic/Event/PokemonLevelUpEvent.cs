#region using directives

using POGOProtos.Enums;

#endregion

namespace PoGo.NecroBot.Logic.Event
{
    public class PokemonLevelUpEvent : IEvent
    {
        public int Cp;
        public PokemonId Id;
        public ulong UniqueId;
    }
}