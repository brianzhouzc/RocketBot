using POGOProtos.Map.Pokemon;

namespace PokemonGo.Bot.ViewModels
{
    public class MapPokemonViewModel : PokemonViewModel
    {
        public MapPokemonViewModel(MapPokemon pokemon)
            : base(pokemon.PokemonId, pokemon.EncounterId)
        {
            EncounterId = pokemon.EncounterId;
            ExpirationTimestampMs = pokemon.ExpirationTimestampMs;
            SpawnPointId = pokemon.SpawnPointId;
            Position = new Position2DViewModel(pokemon.Latitude, pokemon.Longitude);
        }

        public ulong EncounterId { get; }
        public long ExpirationTimestampMs { get; }
        public string SpawnPointId { get; }

        public Position2DViewModel Position { get; }

        public override bool Equals(object obj) => Equals(obj as MapPokemonViewModel);

        public bool Equals(MapPokemonViewModel other)
        {
            return other != null &&
                base.Equals(other) &&
                Position.Equals(other.Position);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 13;
                hash = (hash * 7) + base.GetHashCode();
                hash = (hash * 7) + Position.GetHashCode();
                return hash;
            }
        }
    }
}