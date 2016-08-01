using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.GeneratedCode;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    class TransferPokemonAlgorithmAll : ITranferPokemonAlgorithm
    {
        public IEnumerable<PokemonData> Apply(IEnumerable<PokemonData> allPokemon) => allPokemon;
    }
}
