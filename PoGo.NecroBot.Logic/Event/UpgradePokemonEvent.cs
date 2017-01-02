#region using directives

using POGOProtos.Enums;

#endregion

namespace PoGo.NecroBot.Logic.Event
{
    public class UpgradePokemonEvent : IEvent
    {
        public int BestCp;
        public double BestPerfection;
        public int Cp;
        public int FamilyCandies;
        public PokemonId PokemonId;
        public ulong Id;
        public double Perfection;
    }
}