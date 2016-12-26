using System.Collections.Generic;
using POGOProtos.Map.Pokemon;

namespace PoGo.NecroBot.Logic.Forms_Gui.Event
{
    public class PokemonsEncounterEvent : IEvent
    {
        public List<MapPokemon> EncounterPokemons;
    }
}