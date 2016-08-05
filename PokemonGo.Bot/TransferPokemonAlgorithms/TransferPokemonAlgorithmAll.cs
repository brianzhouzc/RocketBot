using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.GeneratedCode;
using PokemonGo.Bot.ViewModels;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    class TransferPokemonAlgorithmAll : ITranferPokemonAlgorithm
    {
        public IEnumerable<CatchedPokemonViewModel> Apply(IEnumerable<CatchedPokemonViewModel> allPokemon) => allPokemon;
    }
}
