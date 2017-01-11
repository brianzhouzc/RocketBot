#region using directives

using POGOProtos.Data;
using POGOProtos.Enums;
using POGOProtos.Inventory;
using POGOProtos.Networking.Responses;
using POGOProtos.Settings.Master;

#endregion

namespace PoGo.NecroBot.Logic.Event
{
    public class PokemonEvolveEvent : IEvent
    {
        public int Exp;
        public PokemonId Id;
        public ulong UniqueId;
        public EvolvePokemonResponse.Types.Result Result;

        public int Sequence { get;  set; }
        public PokemonData EvolvedPokemon { get;  set; }
        public ulong OriginalId { get;  set; }
        public PokemonSettings PokemonSetting { get; set; }


        public Candy  Family { get; set; }
    }
}
