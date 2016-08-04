using System.Collections.Generic;
using System.Linq;
using PokemonGo.Bot.ViewModels;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    public class TransferPokemonAlgorithmIVDuplicate : ITranferPokemonAlgorithm
    {
        public IEnumerable<CaughtPokemonViewModel> Apply(IEnumerable<CaughtPokemonViewModel> allPokemon)
            => allPokemon
                // find duplicates
                .GroupBy(p => p.PokemonId)
                .Where(g => g.Count() > 1)
                // all duplicates except the highest IV that are not favorited
                .SelectMany(g => g.OrderByDescending(p => p.PerfectPercentage).Skip(1).Where(p => !p.IsFavorite));
    }
}