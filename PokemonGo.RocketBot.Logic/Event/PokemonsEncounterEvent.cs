using System.Collections.Generic;
using POGOProtos.Map.Pokemon;

namespace PokemonGo.RocketBot.Logic.Event
{
    public class PokemonsEncounterEvent : IEvent
    {
        public List<MapPokemon> EncounterPokemons;
    }
}