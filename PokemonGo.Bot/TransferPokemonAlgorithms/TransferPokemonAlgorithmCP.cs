using PokemonGo.Bot.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    public class TransferPokemonAlgorithmCP : ITranferPokemonAlgorithm
    {
        private readonly int threshold;

        public TransferPokemonAlgorithmCP(int threshold)
        {
            this.threshold = threshold;
        }

        public IEnumerable<PokemonDataViewModel> Apply(IEnumerable<PokemonDataViewModel> allPokemon) => allPokemon.Where(p => p.CombatPoints < threshold);
    }
}