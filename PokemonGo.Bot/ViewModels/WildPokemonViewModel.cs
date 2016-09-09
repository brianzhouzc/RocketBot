using System;
using POGOProtos.Data;
using POGOProtos.Map.Pokemon;
using PokemonGo.Bot.MVVMLightUtils;
using POGOProtos.Map.Fort;
using PokemonGo.Bot.Utils;

namespace PokemonGo.Bot.ViewModels
{
    public class WildPokemonViewModel : PokemonDataViewModel, IUpdateable<WildPokemonViewModel>
    {
        public WildPokemonViewModel(WildPokemon pokemon, Settings settings) : base(pokemon.PokemonData, settings)
        {
            EncounterId = pokemon.EncounterId;
            LastModifiedTimestampMs = pokemon.LastModifiedTimestampMs;
            Position = new Position2DViewModel(pokemon.Latitude, pokemon.Longitude);
            SpawnpointId = pokemon.SpawnPointId;
            TimeTillHiddenMs = pokemon.TimeTillHiddenMs;
        }

        public WildPokemonViewModel(PokemonData pokemon, FortData fort, Settings settings)
            : base(pokemon, settings)
        {
            EncounterId = fort.LureInfo.EncounterId;
            LastModifiedTimestampMs = fort.LastModifiedTimestampMs;
            Position = new Position2DViewModel(fort.Latitude, fort.Longitude);
            SpawnpointId = fort.LureInfo.FortId;
            TimeTillHiddenMs = fort.LureInfo.LureExpiresTimestampMs;
        }

        public ulong EncounterId { get; }
        public long LastModifiedTimestampMs { get; }
        public string SpawnpointId { get; }
        long timeTillHiddenMs;
        public long TimeTillHiddenMs
        {
            get { return timeTillHiddenMs; }
            set { if (TimeTillHiddenMs != value) { timeTillHiddenMs = value; RaisePropertyChanged(); } }
        }


        Position2DViewModel Position { get; }

        public void UpdateWith(WildPokemonViewModel other)
        {
            if (!Equals(other))
                throw new ArgumentException($"Expected a {Name} with Id {Id} but got a {other?.Name} with Id {other?.Id}", nameof(other));

            TimeTillHiddenMs = other.TimeTillHiddenMs;
            base.UpdateWith(other);
        }
    }
}