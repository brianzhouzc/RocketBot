using System;
using System.Collections.Generic;
using PokemonGo.RocketAPI.GeneratedCode;
using System.Linq;
using PokemonGo.Bot.ViewModels;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    public class TransferPokemonAlgorithmIVDuplicate : ITranferPokemonAlgorithm
    {
        public IEnumerable<CatchedPokemonViewModel> Apply(IEnumerable<CatchedPokemonViewModel> allPokemon)
            => allPokemon
                // find duplicates
                .GroupBy(p => p.PokemonId)
                .Where(g => g.Count() > 1)
                // all duplicates except the highest IV that are not favorited
                .SelectMany(g => g.OrderByDescending(p => p.PerfectPercentage).Skip(1).Where(p => !p.IsFavorite));
    }
}