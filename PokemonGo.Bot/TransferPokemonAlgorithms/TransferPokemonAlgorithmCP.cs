using System;
using System.Collections.Generic;
using PokemonGo.RocketAPI.GeneratedCode;
using System.Linq;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    public class TransferPokemonAlgorithmCP : ITranferPokemonAlgorithm
    {
        readonly int threshold;

        public TransferPokemonAlgorithmCP(int threshold)
        {
            this.threshold = threshold;
        }
        public IEnumerable<PokemonData> Apply(IEnumerable<PokemonData> allPokemon) => allPokemon.Where(p => p.Cp < threshold);
    }
}