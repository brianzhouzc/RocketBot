using POGOProtos.Data;
using POGOProtos.Networking.Responses;

namespace PoGo.NecroBot.Logic.Event.Inventory
{
    public class FavoriteEvent : IEvent
    {
        public PokemonData Pokemon { get; set; }

        public FavoriteEvent(PokemonData pkm, SetFavoritePokemonResponse res)
        {
            this.Pokemon = pkm;
            FavoritePokemonResponse = res;
        }

        public SetFavoritePokemonResponse FavoritePokemonResponse { get; set; }
    }
}