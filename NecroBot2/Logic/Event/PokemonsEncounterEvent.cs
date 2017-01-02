using System.Collections.Generic;
using POGOProtos.Map.Pokemon;
using PoGo.NecroBot.Logic.Event;

namespace NecroBot2.Logic.Event
{
    public class PokemonsEncounterEvent : IEvent
    {
        public List<MapPokemon> EncounterPokemons;
    }
}