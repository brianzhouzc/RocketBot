using POGOProtos.Data;
using POGOProtos.Map.Pokemon;

namespace PokemonGo.Bot.ViewModels
{
    public class WildPokemonViewModel : PokemonDataViewModel
    {
        public WildPokemonViewModel(WildPokemon pokemon) : base(pokemon.PokemonData)
        {
            EncounterId = pokemon.EncounterId;
            LastModifiedTimestampMs = pokemon.LastModifiedTimestampMs;
            Position = new Position2DViewModel(pokemon.Latitude, pokemon.Longitude);
            SpawnpointId = pokemon.SpawnPointId;
            TimeTillHiddenMs = pokemon.TimeTillHiddenMs;
        }

        public ulong EncounterId { get; }
        public long LastModifiedTimestampMs { get; }
        public string SpawnpointId { get; }
        public int TimeTillHiddenMs { get; }
        Position2DViewModel Position { get; }
    }
}