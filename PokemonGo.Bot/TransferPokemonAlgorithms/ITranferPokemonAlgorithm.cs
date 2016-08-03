using PokemonGo.Bot.ViewModels;
using PokemonGo.RocketAPI.GeneratedCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    public interface ITranferPokemonAlgorithm
    {
        IEnumerable<CatchedPokemonViewModel> Apply(IEnumerable<CatchedPokemonViewModel> allPokemon);
    }
}
