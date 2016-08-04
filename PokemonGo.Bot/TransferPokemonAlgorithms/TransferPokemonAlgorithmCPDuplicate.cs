using PokemonGo.Bot.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    internal class TransferPokemonAlgorithmCPDuplicate : ITranferPokemonAlgorithm
    {
        public IEnumerable<CaughtPokemonViewModel> Apply(IEnumerable<CaughtPokemonViewModel> allPokemon)
            => allPokemon
                // find duplicates
                .GroupBy(p => p.PokemonId)
                .Where(g => g.Count() > 1)
                // all duplicates except the highest CP that are not favorited
                .SelectMany(g => g.OrderByDescending(p => p.CombatPoints).Skip(1).Where(p => !p.IsFavorite));
    }
}