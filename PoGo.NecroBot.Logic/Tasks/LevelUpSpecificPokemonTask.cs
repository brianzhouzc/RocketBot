#region using directives

using System.Linq;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.Logging;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Utils;

#endregion

namespace PoGo.NecroBot.Logic.Tasks
{
    public class LevelUpSpecificPokemonTask
    {
        //this task is duplicated, may need remove to clean up. 
        public static async Task Execute(ISession session, ulong pokemonId)
        {
            using (var blocker = new BlockableScope(session, Model.BotActions.Upgrade))
            {
                if (!await blocker.WaitToRun()) return;

                var all = await session.Inventory.GetPokemons();
                var pokemons = all.OrderByDescending(x => x.Cp).ThenBy(n => n.StaminaMax);
                var pokemon = pokemons.FirstOrDefault(p => p.Id == pokemonId);

                if (pokemon == null) return;

                var upgradeResult = await session.Inventory.UpgradePokemon(pokemon.Id);
                if (upgradeResult.Result.ToString().ToLower().Contains("success"))
                {
                    Logger.Write("Pokemon Upgraded:" + session.Translation.GetPokemonTranslation(upgradeResult.UpgradedPokemon.PokemonId) + ":" + upgradeResult.UpgradedPokemon.Cp, LogLevel.LevelUp);

                    session.EventDispatcher.Send(new PokemonLevelUpEvent
                    {
                        Id = upgradeResult.UpgradedPokemon.PokemonId,
                        Cp = upgradeResult.UpgradedPokemon.Cp,
                        UniqueId = pokemon.Id
                    });

                }
                DelayingUtils.Delay(session.LogicSettings.DelayBetweenPlayerActions, 0);
            }
        }
    }
}
