using PokemonGo.RocketAPI.GeneratedCode;
using System.Collections.Generic;
using System.Linq;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    internal class TransferPokemonAlgorithmCPDuplicate : ITranferPokemonAlgorithm
    {
        public IEnumerable<PokemonData> Apply(IEnumerable<PokemonData> allPokemon)
            => allPokemon
                // find duplicates
                .GroupBy(p => p.PokemonId)
                .Where(g => g.Count() > 1)
                // all duplicates except the highest CP that are not favorited
                .SelectMany(g => g.OrderByDescending(p => p.Cp).Skip(1).Where(p => p.Favorite == 0));
    }
}