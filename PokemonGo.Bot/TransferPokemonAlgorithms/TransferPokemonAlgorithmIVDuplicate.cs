using PokemonGo.Bot.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    public class TransferPokemonAlgorithmIVDuplicate : ITranferPokemonAlgorithm
    {
        public IEnumerable<PokemonDataViewModel> Apply(IEnumerable<PokemonDataViewModel> allPokemon)
            => allPokemon
                // find duplicates
                .GroupBy(p => p.PokemonId)
                .Where(g => g.Count() > 1)
                // all duplicates except the highest IV that are not favorited
                .SelectMany(g => g.OrderByDescending(p => p.PerfectPercentage).Skip(1).Where(p => !p.IsFavorite));
    }
}