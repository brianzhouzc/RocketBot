using PokemonGo.Bot.ViewModels;
using System.Collections.Generic;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    public interface ITranferPokemonAlgorithm
    {
        IEnumerable<CaughtPokemonViewModel> Apply(IEnumerable<CaughtPokemonViewModel> allPokemon);
    }
}