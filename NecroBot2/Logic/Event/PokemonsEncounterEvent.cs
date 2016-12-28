using System.Collections.Generic;
using POGOProtos.Map.Pokemon;

namespace NecroBot2.Logic.Event
{
    public class PokemonsEncounterEvent : IEvent
    {
        public List<MapPokemon> EncounterPokemons;
    }
}