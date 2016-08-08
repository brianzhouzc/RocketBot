using GalaSoft.MvvmLight;
using POGOProtos.Enums;
using System;

namespace PokemonGo.Bot.ViewModels
{
    public abstract class PokemonViewModel : ViewModelBase
    {
        public ulong Id { get; }

        protected PokemonViewModel(PokemonId pokemonId, ulong id)
        {
            Id = id;
            PokemonId = (int)pokemonId;
        }

        public int PokemonId { get; }
        public virtual string Name => Enum.GetName(typeof(PokemonId), PokemonId);

        public override bool Equals(object obj) => Equals(obj as PokemonViewModel);
        public bool Equals(PokemonViewModel other)
        {
            return other != null &&
                PokemonId == other.PokemonId;
        }

        public override int GetHashCode()
        {
            return PokemonId.GetHashCode();
        }
    }
}