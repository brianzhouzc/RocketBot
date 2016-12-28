#region using directives

using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
using POGOProtos.Networking.Responses;

#endregion

namespace NecroBot2.Logic.Event
{
    public class PokemonCaptureEvent : IEvent
    {
        public int Attempt;
        public int BallAmount;
        public string CatchType;
        public int Cp;
        public double Distance;
        public int Exp;
        public int FamilyCandies;
        public PokemonId Id;
        public double Latitude;
        public double Level;
        public double Longitude;
        public int MaxCp;
        public double Perfection;
        public ItemId Pokeball;
        public double Probability;
        public int Stardust;
        public CatchPokemonResponse.Types.CatchStatus Status;
    }
}