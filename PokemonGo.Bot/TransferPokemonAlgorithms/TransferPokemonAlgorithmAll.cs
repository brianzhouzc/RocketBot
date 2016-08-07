using PokemonGo.Bot.ViewModels;
using System.Collections.Generic;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    internal class TransferPokemonAlgorithmAll : ITranferPokemonAlgorithm
    {
        public IEnumerable<PokemonDataViewModel> Apply(IEnumerable<PokemonDataViewModel> allPokemon) => allPokemon;
    }
}