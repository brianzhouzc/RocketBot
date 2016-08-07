using PokemonGo.Bot.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    public class TransferPokemonAlgorithmIV : ITranferPokemonAlgorithm
    {
        private readonly double threshold;

        public TransferPokemonAlgorithmIV(double threshold)
        {
            this.threshold = threshold;
        }

        public IEnumerable<PokemonDataViewModel> Apply(IEnumerable<PokemonDataViewModel> allPokemon) => allPokemon.Where(p => p.PerfectPercentage < threshold);
    }
}