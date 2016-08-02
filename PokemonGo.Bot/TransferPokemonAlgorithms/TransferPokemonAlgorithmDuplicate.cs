using PokemonGo.RocketAPI.GeneratedCode;
using System.Collections.Generic;
using System.Linq;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    public class TransferPokemonAlgorithmDuplicate : ITranferPokemonAlgorithm
    {
        public IEnumerable<PokemonData> Apply(IEnumerable<PokemonData> allPokemon)
            => allPokemon
                // find duplicates
                .GroupBy(p => p.PokemonId)
                .Where(g => g.Count() > 1)
                // all duplicates that are not favorited
                .SelectMany(g => g.OrderByDescending(p => p.Favorite).Skip(1).Where(p => p.Favorite == 0));
    }
}