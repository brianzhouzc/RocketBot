using System;
using System.Collections.Generic;
using PokemonGo.RocketAPI.GeneratedCode;
using System.Linq;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    public class TransferPokemonAlgorithmIV : ITranferPokemonAlgorithm
    {
        readonly double threshold;

        public TransferPokemonAlgorithmIV(double threshold)
        {
            this.threshold = threshold;
        }
        public IEnumerable<PokemonData> Apply(IEnumerable<PokemonData> allPokemon) => allPokemon.Where(p => p.GetIV() < threshold);
    }
}