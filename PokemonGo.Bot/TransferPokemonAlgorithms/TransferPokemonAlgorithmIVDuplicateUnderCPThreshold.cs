using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.GeneratedCode;
using PokemonGo.Bot.ViewModels;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    public class TransferPokemonAlgorithmIVDuplicateUnderCPThreshold : ITranferPokemonAlgorithm
    {
        readonly int threshold;

        public TransferPokemonAlgorithmIVDuplicateUnderCPThreshold(int threshold)
        {
            this.threshold = threshold;
        }

        public IEnumerable<CatchedPokemonViewModel> Apply(IEnumerable<CatchedPokemonViewModel> allPokemon)
            => allPokemon
                // find duplicates
                .GroupBy(p => p.PokemonId)
                .Where(g => g.Count() > 1)
                // all duplicates except the highest IV that are not favorited and are under the given threshold
                .SelectMany(g => g.OrderByDescending(p => p.PerfectPercentage).Skip(1).Where(p => !p.IsFavorite && p.CombatPoints < threshold));
    }
}
