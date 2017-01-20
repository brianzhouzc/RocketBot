#region using directives

using System.Collections.Generic;
using POGOProtos.Map.Fort;
using POGOProtos.Map.Pokemon;

#endregion

namespace PoGo.NecroBot.Logic.Event
{
    public class PokeStopListEvent : IEvent
    {
        public List<FortData> Forts;
        public List<NearbyPokemon> NearbyPokemons;

        public PokeStopListEvent(List<FortData> forts)
        {
            Forts = forts;
        }

        public PokeStopListEvent(List<FortData> forts, List<NearbyPokemon> nearbyPokemons) : this(forts)
        {
            this.NearbyPokemons = nearbyPokemons;
        }
    }
}