using POGOProtos.Data;
using POGOProtos.Networking.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Event.Inventory
{
    public class FavoriteEvent   : IEvent
    {
        public PokemonData Pokemon { get; set; }
        public FavoriteEvent(PokemonData pkm,SetFavoritePokemonResponse res)
        {
            this.Pokemon = pkm;
            FavoritePokemonResponse = res;
        }

        public SetFavoritePokemonResponse FavoritePokemonResponse { get;  set; }
    }
}
