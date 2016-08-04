using System.Collections.Generic;
using System.Linq;
using PokemonGo.Bot.ViewModels;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    class TransferPokemonAlgorithmNone : ITranferPokemonAlgorithm
    {
        public IEnumerable<CaughtPokemonViewModel> Apply(IEnumerable<CaughtPokemonViewModel> allPokemon) => Enumerable.Empty<CaughtPokemonViewModel>();
    }
}
