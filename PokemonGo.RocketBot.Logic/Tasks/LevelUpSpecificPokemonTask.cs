using POGOProtos.Networking.Responses;
using PokemonGo.RocketBot.Logic.Logging;
using PokemonGo.RocketBot.Logic.State;
using PokemonGo.RocketBot.Logic.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketBot.Logic.Tasks
{
    public class LevelUpSpecificPokemonTask
    {
        public static async Task Execute(ISession session, ulong pokemonId)
        {
            var upgradeResult = await session.Inventory.UpgradePokemon(pokemonId);
            if (upgradeResult.Result == UpgradePokemonResponse.Types.Result.Success)
            {
                Logger.Write("Pokemon Upgraded: " +
                             session.Translation.GetPokemonTranslation(
                                 upgradeResult.UpgradedPokemon.PokemonId) + ": " +
                             upgradeResult.UpgradedPokemon.Cp, LogLevel.LevelUp);
            }
            else
            {
                Logger.Write("Pokemon Upgrade Failed.", LogLevel.Warning);
            }

            DelayingUtils.Delay(session.LogicSettings.DelayBetweenPlayerActions, 0);
        }
    }
}
