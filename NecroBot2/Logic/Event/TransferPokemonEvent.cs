#region using directives

using PoGo.NecroBot.Logic.Event;
using POGOProtos.Enums;

#endregion

namespace NecroBot2.Logic.Event
{
    public class TransferPokemonEvent : IEvent
    {
        public int BestCp;
        public double BestPerfection;
        public int Cp;
        public int FamilyCandies;
        public PokemonId Id;
        public double Perfection;
    }
}