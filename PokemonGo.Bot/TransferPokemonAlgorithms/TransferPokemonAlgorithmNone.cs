using PokemonGo.Bot.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    internal class TransferPokemonAlgorithmNone : ITranferPokemonAlgorithm
    {
        public IEnumerable<PokemonDataViewModel> Apply(IEnumerable<PokemonDataViewModel> allPokemon) => Enumerable.Empty<PokemonDataViewModel>();
    }
}