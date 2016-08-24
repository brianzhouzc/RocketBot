using PokemonGo.Bot.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    public class TransferPokemonAlgorithmIV : ITranferPokemonAlgorithm
    {
        readonly double? threshold;

        public TransferPokemonAlgorithmIV(double? threshold)
        {
            this.threshold = threshold;
        }

        public IEnumerable<CaughtPokemonViewModel> Apply(IEnumerable<CaughtPokemonViewModel> allPokemon) => allPokemon.Where(p => p.PerfectPercentage < threshold);
    }
}