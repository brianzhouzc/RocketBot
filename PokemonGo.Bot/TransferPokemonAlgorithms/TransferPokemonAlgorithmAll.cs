using System.Collections.Generic;
using PokemonGo.Bot.ViewModels;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    class TransferPokemonAlgorithmAll : ITranferPokemonAlgorithm
    {
        public IEnumerable<CaughtPokemonViewModel> Apply(IEnumerable<CaughtPokemonViewModel> allPokemon) => allPokemon;
    }
}
