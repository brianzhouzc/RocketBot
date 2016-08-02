using System;
using System.Collections.Generic;
using PokemonGo.RocketAPI.GeneratedCode;
using System.Linq;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    public class TransferPokemonAlgorithmIVDuplicate : ITranferPokemonAlgorithm
    {
        public IEnumerable<PokemonData> Apply(IEnumerable<PokemonData> allPokemon)
            => allPokemon
                // find duplicates
                .GroupBy(p => p.PokemonId)
                .Where(g => g.Count() > 1)
                // all duplicates except the highest IV that are not favorited
                .SelectMany(g => g.OrderByDescending(p => p.GetIV()).Skip(1).Where(p => p.Favorite == 0));
    }
}