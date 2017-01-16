using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POGOProtos.Data;
using POGOProtos.Enums;

namespace PoGo.NecroBot.Logic.Event.Player
{
    public class BuddyUpdateEvent : IEvent
    {
        public BuddyPokemon Buddy { get; set; }

        public PokemonData Pokemon { get; set; }
        public BuddyUpdateEvent(BuddyPokemon updatedBuddy, PokemonData pkm)
        {
            Buddy = updatedBuddy;
            this.Pokemon = pkm;
        }
    }
}
