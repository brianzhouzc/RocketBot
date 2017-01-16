#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Logging;
using PoGo.NecroBot.Logic.PoGoUtils;
using PoGo.NecroBot.Logic.State;
using POGOProtos.Data;
using PoGo.NecroBot.Logic.Event;
using POGOProtos.Inventory;
using POGOProtos.Settings.Master;
using System;
using PokemonGo.RocketAPI.Exceptions;
using PoGo.NecroBot.Logic.Event.Inventory;

#endregion

namespace PoGo.NecroBot.Logic.Tasks
{
    public class UpgradeSinglePokemonTask
    {
        private static async Task<bool> UpgradeSinglePokemon(ISession session, PokemonData pokemon, List<Candy> pokemonFamilies, List<PokemonSettings> pokemonSettings)
        {
            var playerLevel = session.Inventory.GetPlayerStats().Result.FirstOrDefault().Level;
            var pokemonLevel = PokemonInfo.GetLevel(pokemon);

            if (pokemonLevel  > playerLevel ) return false;

            var settings = pokemonSettings.Single(x => x.PokemonId == pokemon.PokemonId);
            var familyCandy = pokemonFamilies.Single(x => settings.FamilyId == x.FamilyId);

            if (familyCandy.Candy_ <= settings.CandyToEvolve) return false;

            var upgradeResult = await session.Inventory.UpgradePokemon(pokemon.Id);

            await session.Inventory.UpdateCandy(familyCandy, -settings.CandyToEvolve);

            var bestPokemonOfType = (session.LogicSettings.PrioritizeIvOverCp
    ? await session.Inventory.GetHighestPokemonOfTypeByIv(upgradeResult.UpgradedPokemon)
    : await session.Inventory.GetHighestPokemonOfTypeByCp(upgradeResult.UpgradedPokemon)) ?? upgradeResult.UpgradedPokemon;

            if (upgradeResult.Result == POGOProtos.Networking.Responses.UpgradePokemonResponse.Types.Result.Success)
            {
                session.EventDispatcher.Send(new UpgradePokemonEvent()
                {
                    FamilyCandies = familyCandy.Candy_ - settings.CandyToEvolve,
                    Pokemon = upgradeResult.UpgradedPokemon ,
                    PokemonId = upgradeResult.UpgradedPokemon.PokemonId,
                    Cp = upgradeResult.UpgradedPokemon.Cp,
                    Id = upgradeResult.UpgradedPokemon.Id,
                    BestCp = bestPokemonOfType.Cp,
                    BestPerfection = PokemonInfo.CalculatePokemonPerfection(bestPokemonOfType),
                    Perfection = PokemonInfo.CalculatePokemonPerfection(upgradeResult.UpgradedPokemon)
                });
            }
            return true;

        }
        public static async Task Execute(ISession session, ulong pokemonId, bool isMax = false)
        {
            using (var block = new BlockableScope(session, Model.BotActions.Upgrade))
            {
                if (!await block.WaitToRun())
                {
                    session.EventDispatcher.Send(new FinishUpgradeEvent()
                    {
                        PokemonId = pokemonId,
                        AllowUpgrade = true
                    });
                };
                //await session.Inventory.RefreshCachedInventory();

                if (session.Inventory.GetStarDust() <= session.LogicSettings.GetMinStarDustForLevelUp)
                    return;
                var pokemonToUpgrade = await session.Inventory.GetSinglePokemon(pokemonId);

                if (pokemonToUpgrade == null)
                {
                    session.Actions.RemoveAll(x => x == Model.BotActions.Upgrade);
                    return;

                }

                var myPokemonSettings = await session.Inventory.GetPokemonSettings();
                var pokemonSettings = myPokemonSettings.ToList();

                var myPokemonFamilies = await session.Inventory.GetPokemonFamilies();
                var pokemonFamilies = myPokemonFamilies.ToList();

                bool upgradable = false;
                int upgradeTimes = 0;
                do
                {
                    try
                    {
                        upgradable = await UpgradeSinglePokemon(session, pokemonToUpgrade, pokemonFamilies, pokemonSettings); ;
                        if (upgradable)
                        {
                            await Task.Delay(session.LogicSettings.DelayBetweenPokemonUpgrade);
                        }
                        upgradeTimes++;

                    }
                    catch (CaptchaException cex)
                    {
                        throw cex;
                    }
                    catch (Exception)
                    {
                        //make sure no exception happen
                    }
                }
                while (upgradable && (isMax || upgradeTimes < session.LogicSettings.AmountOfTimesToUpgradeLoop));
                session.EventDispatcher.Send(new FinishUpgradeEvent()
                {
                    PokemonId = pokemonId,
                    AllowUpgrade = !isMax && upgradable
                });
            }
        }
    }
}