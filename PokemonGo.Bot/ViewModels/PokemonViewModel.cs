using AllEnum;
using System;

namespace PokemonGo.Bot.ViewModels
{
    public abstract class PokemonViewModel
    {
        protected PokemonViewModel(PokemonId pokemonId)
        {
            PokemonId = pokemonId;
        }

        public PokemonId PokemonId { get; }
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