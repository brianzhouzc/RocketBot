using System;
using System.Collections.Generic;
using PokemonGo.RocketAPI.GeneratedCode;
using System.Linq;
using PokemonGo.Bot.ViewModels;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    public class TransferPokemonAlgorithmCP : ITranferPokemonAlgorithm
    {
        readonly int threshold;

        public TransferPokemonAlgorithmCP(int threshold)
        {
            this.threshold = threshold;
        }
        public IEnumerable<CatchedPokemonViewModel> Apply(IEnumerable<CatchedPokemonViewModel> allPokemon) => allPokemon.Where(p => p.CombatPoints < threshold);
    }
}