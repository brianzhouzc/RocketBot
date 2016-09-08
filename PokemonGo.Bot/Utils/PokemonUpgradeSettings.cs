using System.Collections.Generic;
using System.Linq;

namespace PokemonGo.Bot.Utils
{
    public class PokemonUpgradeSettings
    {
        readonly IReadOnlyList<int> candyCost;
        readonly IReadOnlyList<int> stardustCost;

        public PokemonUpgradeSettings(POGOProtos.Settings.Master.PokemonUpgradeSettings pokemonUpgradeTemplate)
        {
            AllowedLevelsAbovePlayer = pokemonUpgradeTemplate.AllowedLevelsAbovePlayer;
            candyCost = pokemonUpgradeTemplate.CandyCost.ToList().AsReadOnly();
            stardustCost = pokemonUpgradeTemplate.StardustCost.ToList().AsReadOnly();
            UpgradesPerLevel = pokemonUpgradeTemplate.UpgradesPerLevel;
        }

        public int AllowedLevelsAbovePlayer { get; }
        public int UpgradesPerLevel { get; }

        public int GetCandyCostForUpgradeFromLevel(int level) => candyCost[level - 1];

        public int GetStardustCostForUpgradeFromLevel(int level) => stardustCost[level - 1];
    }
}