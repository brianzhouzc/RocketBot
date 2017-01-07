#region using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.PoGoUtils;
using PoGo.NecroBot.Logic.State;
using POGOProtos.Inventory.Item;
using POGOProtos.Map.Fort;
using PoGo.NecroBot.Logic.Logging;
using POGOProtos.Networking.Responses;
using POGOProtos.Enums;
using PoGo.NecroBot.Logic.Event.Gym;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.Rpc;
using POGOProtos.Data;
using POGOProtos.Data.Battle;
using PokemonGo.RocketAPI.Exceptions;

#endregion

namespace PoGo.NecroBot.Logic.Tasks
{
    public class UseGymBattleTask
    {
        public static async Task Execute(ISession session, CancellationToken cancellationToken, FortData gym, FortDetailsResponse fortInfo)
        {
            if (!session.LogicSettings.GymAllowed || gym.Type != FortType.Gym) return;

            cancellationToken.ThrowIfCancellationRequested();
            var distance = session.Navigation.WalkStrategy.CalculateDistance(session.Client.CurrentLatitude, session.Client.CurrentLongitude, gym.Latitude, gym.Longitude);
            if (fortInfo != null)
            {
                session.EventDispatcher.Send(new GymWalkToTargetEvent()
                {
                    Name = fortInfo.Name,
                    Distance = distance,
                    Latitude = fortInfo.Latitude,
                    Longitude = fortInfo.Longitude
                });

                var fortDetails = await session.Client.Fort.GetGymDetails(gym.Id, gym.Latitude, gym.Longitude);

                if (fortDetails.Result == GetGymDetailsResponse.Types.Result.Success)
                {
                    if (fortDetails.Result == GetGymDetailsResponse.Types.Result.Success)
                    {
                        var player = session.Profile.PlayerData;
                        await EnsureJoinTeam(session, player);

                        //Do gym tutorial - tobe coded

                        session.EventDispatcher.Send(new GymDetailInfoEvent()
                        {
                            Team = fortDetails.GymState.FortData.OwnedByTeam,
                            Point = gym.GymPoints,
                            Name = fortDetails.Name,
                        });

                        if (player.Team != TeamColor.Neutral && fortDetails.GymState.FortData.OwnedByTeam == player.Team)
                        {
                            //trainning logic will come here
                            await DeployPokemonToGym(session, fortInfo, fortDetails);
                        }
                        else
                        {
                            //await StartGymAttackLogic(session, fortInfo, fortDetails, gym, cancellationToken);
                            //Logger.Write($"No action... This gym is defending by other color", LogLevel.Gym, ConsoleColor.White);
                        }
                    }
                    else
                    {
                        Logger.Write($"You are not level 5 yet, come back later...", LogLevel.Gym, ConsoleColor.White);
                    }
                }
            }
            else
            {
                // ReSharper disable once PossibleNullReferenceException
                Logger.Write($"Ignoring  Gym : {fortInfo.Name} - ", LogLevel.Gym, ConsoleColor.Cyan);
            }
        }

        private static async Task StartGymAttackLogic(ISession session, FortDetailsResponse fortInfo,
            GetGymDetailsResponse fortDetails, FortData gym, CancellationToken cancellationToken)
        {
            bool fighting = true;

            while (fighting)
            {
                await session.Inventory.RefreshCachedInventory();
                var badassPokemon = await session.Inventory.GetHighestCpForGym(6);
                var pokemonDatas = badassPokemon as PokemonData[] ?? badassPokemon.ToArray();

                // Heal pokemon
                foreach (var pokemon in pokemonDatas)
                {
                    if (pokemon.Stamina <= 0)
                        await RevivePokemon(session, pokemon);
                    if (pokemon.Stamina < pokemon.StaminaMax)
                        await HealPokemon(session, pokemon);
                }
                // await Task.Delay(4000);

                int tries = 0;
                StartGymBattleResponse result = null;
                do
                {
                    tries++;
                    try
                    {
                        result = await StartBattle(session, pokemonDatas, gym);
                        if (result.Result == StartGymBattleResponse.Types.Result.Unset)
                        {
                            // Try to refresh information about pokemons and gym
                            await session.Inventory.RefreshCachedInventory();
                            badassPokemon = await session.Inventory.GetHighestCpForGym(6);
                            pokemonDatas = badassPokemon as PokemonData[] ?? badassPokemon.ToArray();
                            Logger.Write($"Failed to Start Gym Battle at try: {tries}, waiting 20 seconds before make another one.", LogLevel.Gym, ConsoleColor.Red);
                            await Task.Delay(2000);
                        }
                    }
                    catch (APIBadRequestException)
                    {
                        Logger.Write("Start battle failed....", LogLevel.Warning);
                    }
                } while (result.Result == StartGymBattleResponse.Types.Result.Unset && tries <= 10);

                // If we can't start battle in 10 tries, let's skip the gym
                if (result.Result == StartGymBattleResponse.Types.Result.Unset)
                {
                    session.EventDispatcher.Send(new GymErrorUnset { GymName = fortInfo.Name });
                    break;
                }

                if (result.Result != StartGymBattleResponse.Types.Result.Success) break;
                switch (result.BattleLog.State)
                {
                    case BattleState.Active:
                        Logger.Write($"Time to start Attack Mode", LogLevel.Gym, ConsoleColor.Red);
                        await AttackGym(session, cancellationToken, gym, result);
                        break;
                    case BattleState.Defeated:
                        break;
                    case BattleState.StateUnset:
                        break;
                    case BattleState.TimedOut:
                        break;
                    case BattleState.Victory:
                        fighting = false;
                        break;
                    default:
                        Logger.Write($"Unhandled result starting gym battle:\n{result}");
                        break;
                }

                fortDetails = await session.Client.Fort.GetGymDetails(gym.Id, gym.Latitude, gym.Longitude);
                if (fortDetails.GymState.FortData.OwnedByTeam == TeamColor.Neutral ||
                    fortDetails.GymState.FortData.OwnedByTeam == session.Profile.PlayerData.Team)
                    break;
            }

            // Finished battling.. OwnedByTeam should be neutral when we reach here
            if (fortDetails.GymState.FortData.OwnedByTeam == TeamColor.Neutral ||
                fortDetails.GymState.FortData.OwnedByTeam == session.Profile.PlayerData.Team)
            {
                await Execute(session, cancellationToken, gym, fortInfo);
            }
            else
            {
                Logger.Write($"Hmmm, for some reason the gym was not taken over...");
            }
        }

        private static async Task DeployPokemonToGym(ISession session, FortDetailsResponse fortInfo, GetGymDetailsResponse fortDetails)
        {
            var maxCount = 0;
            var points = fortDetails.GymState.FortData.GymPoints;
            if (points < 1600) maxCount = 1;
            else if (points < 4000) maxCount = 2;
            else if (points < 8000) maxCount = 3;
            else if (points < 12000) maxCount = 4;
            else if (points < 16000) maxCount = 5;
            else if (points < 20000) maxCount = 6;
            else if (points < 30000) maxCount = 7;
            else if (points < 40000) maxCount = 8;
            else if (points < 50000) maxCount = 9;
            else maxCount = 10;

            var availableSlots = maxCount - fortDetails.GymState.Memberships.Count();

            if (availableSlots > 0)
            {
                var pokemon = await GetDeployablePokemon(session);
                if (pokemon != null)
                {
                    var response = await session.Client.Fort.FortDeployPokemon(fortInfo.FortId, pokemon.Id);
                    if (response.Result == FortDeployPokemonResponse.Types.Result.Success)
                    {
                        session.EventDispatcher.Send(new GymDeployEvent()
                        {
                            PokemonId = pokemon.PokemonId,
                            Name = fortDetails.Name
                        });
                        if (session.LogicSettings.GymCollectRewardAfter > 0)
                        {
                            var deployed = await session.Inventory.GetDeployedPokemons();
                            var count = deployed.Count();
                            if (count >= session.LogicSettings.GymCollectRewardAfter)
                            {
                                var collectDailyBonusResponse = await session.Client.Player.CollectDailyBonus();
                                if (collectDailyBonusResponse.Result == CollectDailyBonusResponse.Types.Result.Success)
                                {
                                    Logger.Write($"Collected {count * 10} coins", LogLevel.Gym, ConsoleColor.DarkYellow);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                string message = "No action. No FREE slots in GYM " + fortDetails.GymState.Memberships.Count() + "/" + maxCount;
                Logger.Write(message, LogLevel.Gym, ConsoleColor.White);
            }
        }

        public static async Task RevivePokemon(ISession session, PokemonData pokemon)
        {
            var normalRevives = await session.Inventory.GetItemAmountByType(ItemId.ItemRevive);
            if (normalRevives > 0 && pokemon.Stamina <= 0)
            {
                var ret = await session.Client.Inventory.UseItemRevive(ItemId.ItemRevive, pokemon.Id);
                switch (ret.Result)
                {
                    case UseItemReviveResponse.Types.Result.Success:
                        session.EventDispatcher.Send(new EventUsedRevive
                        {
                            Type = "normal",
                            PokemonCp = pokemon.Cp,
                            PokemonId = pokemon.PokemonId.ToString(),
                            Remaining = (normalRevives - 1)
                        });
                        break;
                    case UseItemReviveResponse.Types.Result.ErrorDeployedToFort:
                        Logger.Write(
                            $"Pokemon: {pokemon.PokemonId} (CP: {pokemon.Cp}) is already deployed to a gym...");
                        return;
                    case UseItemReviveResponse.Types.Result.ErrorCannotUse:
                        return;
                    default:
                        return;
                }
                return;
            }
            var maxRevives = await session.Inventory.GetItemAmountByType(ItemId.ItemMaxRevive);
            if (maxRevives > 0 && pokemon.Stamina <= 0)
            {
                var ret = await session.Client.Inventory.UseItemRevive(ItemId.ItemMaxRevive, pokemon.Id);
                switch (ret.Result)
                {
                    case UseItemReviveResponse.Types.Result.Success:
                        session.EventDispatcher.Send(new EventUsedRevive
                        {
                            Type = "max",
                            PokemonCp = pokemon.Cp,
                            PokemonId = pokemon.PokemonId.ToString(),
                            Remaining = (maxRevives - 1)
                        });
                        break;

                    case UseItemReviveResponse.Types.Result.ErrorDeployedToFort:
                        Logger.Write($"Pokemon: {pokemon.PokemonId} (CP: {pokemon.Cp}) is already deployed to a gym...");
                        return;

                    case UseItemReviveResponse.Types.Result.ErrorCannotUse:
                        return;

                    default:
                        return;
                }
            }
        }

        public static async Task HealPokemon(ISession session, PokemonData pokemon)
        {
            var normalPotions = await session.Inventory.GetItemAmountByType(ItemId.ItemPotion);
            while (normalPotions > 0 && (pokemon.Stamina < pokemon.StaminaMax))
            {
                var ret = await session.Client.Inventory.UseItemPotion(ItemId.ItemPotion, pokemon.Id);
                switch (ret.Result)
                {
                    case UseItemPotionResponse.Types.Result.Success:
                        pokemon.Stamina = ret.Stamina;
                        session.EventDispatcher.Send(new EventUsedPotion
                        {
                            Type = "normal",
                            PokemonCp = pokemon.Cp,
                            PokemonId = pokemon.PokemonId.ToString(),
                            Remaining = (normalPotions - 1)
                        });
                        break;

                    case UseItemPotionResponse.Types.Result.ErrorDeployedToFort:
                        Logger.Write($"Pokemon: {pokemon.PokemonId} (CP: {pokemon.Cp}) is already deployed to a gym...");
                        return;

                    case UseItemPotionResponse.Types.Result.ErrorCannotUse:
                        return;

                    default:
                        return;
                }
                normalPotions--;
            }

            var superPotions = await session.Inventory.GetItemAmountByType(ItemId.ItemSuperPotion);
            while (superPotions > 0 && (pokemon.Stamina < pokemon.StaminaMax))
            {
                var ret = await session.Client.Inventory.UseItemPotion(ItemId.ItemSuperPotion, pokemon.Id);
                switch (ret.Result)
                {
                    case UseItemPotionResponse.Types.Result.Success:
                        pokemon.Stamina = ret.Stamina;
                        session.EventDispatcher.Send(new EventUsedPotion
                        {
                            Type = "super",
                            PokemonCp = pokemon.Cp,
                            PokemonId = pokemon.PokemonId.ToString(),
                            Remaining = (superPotions - 1)
                        });
                        break;

                    case UseItemPotionResponse.Types.Result.ErrorDeployedToFort:
                        Logger.Write($"Pokemon: {pokemon.PokemonId} (CP: {pokemon.Cp}) is already deployed to a gym...");
                        return;

                    case UseItemPotionResponse.Types.Result.ErrorCannotUse:
                        return;

                    default:
                        return;
                }
                superPotions--;
            }

            var hyperPotions = await session.Inventory.GetItemAmountByType(ItemId.ItemHyperPotion);
            while (hyperPotions > 0 && (pokemon.Stamina < pokemon.StaminaMax))
            {
                var ret = await session.Client.Inventory.UseItemPotion(ItemId.ItemHyperPotion, pokemon.Id);
                switch (ret.Result)
                {
                    case UseItemPotionResponse.Types.Result.Success:
                        pokemon.Stamina = ret.Stamina;
                        session.EventDispatcher.Send(new EventUsedPotion
                        {
                            Type = "hyper",
                            PokemonCp = pokemon.Cp,
                            PokemonId = pokemon.PokemonId.ToString(),
                            Remaining = (hyperPotions - 1)
                        });
                        break;

                    case UseItemPotionResponse.Types.Result.ErrorDeployedToFort:
                        Logger.Write($"Pokemon: {pokemon.PokemonId} (CP: {pokemon.Cp}) is already deployed to a gym...");
                        return;

                    case UseItemPotionResponse.Types.Result.ErrorCannotUse:
                        return;

                    default:
                        return;
                }
                hyperPotions--;
            }

            var maxPotions = await session.Inventory.GetItemAmountByType(ItemId.ItemMaxPotion);
            while (maxPotions > 0 && (pokemon.Stamina < pokemon.StaminaMax))
            {
                var ret = await session.Client.Inventory.UseItemPotion(ItemId.ItemMaxPotion, pokemon.Id);
                switch (ret.Result)
                {
                    case UseItemPotionResponse.Types.Result.Success:
                        pokemon.Stamina = ret.Stamina;
                        session.EventDispatcher.Send(new EventUsedPotion
                        {
                            Type = "max",
                            PokemonCp = pokemon.Cp,
                            PokemonId = pokemon.PokemonId.ToString(),
                            Remaining = (maxPotions - 1)
                        });
                        break;

                    case UseItemPotionResponse.Types.Result.ErrorDeployedToFort:
                        Logger.Write($"Pokemon: {pokemon.PokemonId} (CP: {pokemon.Cp}) is already deployed to a gym...");
                        return;

                    case UseItemPotionResponse.Types.Result.ErrorCannotUse:
                        return;

                    default:
                        return;
                }
                maxPotions--;
            }
        }

        private static int _currentAttackerEnergy;

        // ReSharper disable once UnusedParameter.Local
        private static async Task AttackGym(ISession session, CancellationToken cancellationToken, FortData currentFortData, StartGymBattleResponse startResponse)
        {
            long serverMs = startResponse.BattleLog.BattleStartTimestampMs;
            var lastActions = startResponse.BattleLog.BattleActions.ToList();

            Logger.Write($"Gym battle started; fighting trainer: {startResponse.Defender.TrainerPublicProfile.Name}", LogLevel.Gym, ConsoleColor.Green);
            Logger.Write($"We are attacking: {startResponse.Defender.ActivePokemon.PokemonData.PokemonId}", LogLevel.Gym, ConsoleColor.White);
            int loops = 0;
            List<BattleAction> emptyActions = new List<BattleAction>();
            BattleAction emptyAction = new BattleAction();
            PokemonData attacker = null;
            PokemonData defender = null;
            _currentAttackerEnergy = 0;
            var lastState = BattleState.StateUnset;
            while (true)
            {
                var attackActionz = await GetActions(session, serverMs, attacker, defender, _currentAttackerEnergy);
                try
                {
                    var last = lastActions.LastOrDefault();

                    var attackResult =
                        await session.Client.Fort.AttackGym
                        (
                            currentFortData.Id,
                            startResponse.BattleId,
                            (last == null || lastState == BattleState.Victory?  emptyActions : attackActionz),
                            (loops > 0 ? lastActions.Last() : emptyAction)

                        );


                    loops++;

                    if (attackResult.Result == AttackGymResponse.Types.Result.Success)
                    {

                        defender = attackResult.ActiveDefender?.PokemonData;

                        switch (attackResult.BattleLog.State)
                        {
                            case BattleState.Active:
                                _currentAttackerEnergy = attackResult.ActiveAttacker.CurrentEnergy;
                                attacker = attackResult.ActiveAttacker.PokemonData;

                                Logger.Write(
                                    $"(GYM ATTACK) Defender {attackResult.ActiveDefender.PokemonData.PokemonId.ToString()  } HP {attackResult.ActiveDefender.CurrentHealth} Attacker  {attackResult.ActiveAttacker.PokemonData.PokemonId.ToString()} HP {attackResult.ActiveAttacker.CurrentHealth} health, energy: {attackResult.ActiveAttacker.CurrentEnergy}");

                                if (attackResult.ActiveAttacker.CurrentHealth <= 0) attacker = null;

                                break;
                               
                            case BattleState.Defeated:
                                Logger.Write(
                                    $"We were defeated... (AttackGym)");
                                return;
                            case BattleState.TimedOut:
                                Logger.Write(
                                    $"Our attack timed out...:");
                                return;
                            case BattleState.StateUnset:
                                Logger.Write(
                                    $"State was unset?: {attackResult}");
                                return;

                            case BattleState.Victory:
                                Logger.Write(
                                    $"We were victorious!: ");

                                break;
                            default:
                                Logger.Write(
                                    $"Unhandled attack response: {attackResult}");
                                continue;
                        }
                        Debug.Write($"{attackResult}");


                        await Task.Delay(attackActionz.Sum(x => x.DurationMs) + 100);
                        // Sleep until last sent battle action expired
                        //bool sleep = true;
                        //while (attackActionz.LastOrDefault() != null && sleep)
                        //{
                        //    await Task.Delay(1000);
                        //    DateTime currentTime = DateTime.Now;
                        //    if (currentTime.ToUnixTime() > attackActionz.LastOrDefault().DamageWindowsEndTimestampMss)
                        //    {
                        //        sleep = false;
                        //        break;
                        //    }
                        //    else
                        //    {
                        //        Logger.Write($"Sleeping until next attack, {currentTime.ToUnixTime()} < {attackActionz.LastOrDefault().DamageWindowsEndTimestampMss}");
                        //    }
                        //}
                        Logger.Write($"Finished sleeping, starting another attack");

                    }
                    else
                    {
                        Logger.Write($"Unexpected attack result:\n{attackResult}");
                        continue;
                    }

                    if (attackResult.BattleLog != null && attackResult.BattleLog.BattleActions.Count > 0)
                        lastActions.AddRange(attackResult.BattleLog.BattleActions);
                    serverMs = attackResult.BattleLog.ServerMs;
                }


                catch (APIBadRequestException)
                {
                    Logger.Write("Shit!!!! Bad request send to server -", LogLevel.Warning);
                };
            }
        }

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static DateTime DateTimeFromUnixTimestampMillis(long millis)
        {
            return UnixEpoch.AddMilliseconds(millis);
        }

        //private static int _pos;
        public static async Task<List<BattleAction>> GetActions(ISession sessison, long serverMs, PokemonData attacker, PokemonData defender, int energy)
        {
            Random rnd = new Random();
            List<BattleAction> actions = new List<BattleAction>();
            DateTime now = DateTimeFromUnixTimestampMillis(serverMs);
            //Logger.Write($"AttackGym Count: {_pos}");

            var inventory = sessison.Inventory;

            if (attacker != null && defender != null)
            {
                var move1 = PokemonMoveMetaRegistry.GetMeta(attacker.Move1);
                var move2 = PokemonMoveMetaRegistry.GetMeta(attacker.Move2);
                //  Logger.Write($"Retrieved Move Metadata, Move1: {move1.GetTime()} - Move2: {move2.GetTime()}");

                var moveSetting = await inventory.GetMoveSetting(attacker.Move1);

                var specialMove = await inventory.GetMoveSetting(attacker.Move2);

                    BattleAction action2 = new BattleAction();
                    now = now.AddMilliseconds(specialMove.DurationMs);
                    action2.Type = Math.Abs(specialMove.EnergyDelta) < energy? BattleActionType.ActionSpecialAttack : BattleActionType.ActionAttack;
                    action2.DurationMs = specialMove.DurationMs;
                    action2.ActionStartMs = now.ToUnixTime();
                    action2.TargetIndex = -1;
                    action2.ActivePokemonId = attacker.Id;

                    action2.DamageWindowsStartTimestampMs = specialMove.DamageWindowStartMs;
                    action2.DamageWindowsEndTimestampMs = specialMove.DamageWindowEndMs;
                    actions.Add(action2);

                    return actions;
            }
            BattleAction action1 = new BattleAction();
            now = now.AddMilliseconds(500);
            action1.Type = BattleActionType.ActionAttack;
            action1.DurationMs = 500;
            action1.ActionStartMs = now.ToUnixTime();
            action1.TargetIndex = -1;
            if(defender != null)
            action1.ActivePokemonId = attacker.Id;

            actions.Add(action1);

            return actions;

        }

        private static async Task<StartGymBattleResponse> StartBattle(ISession session, IEnumerable<PokemonData> pokemons, FortData currentFortData)
        {

            IEnumerable<PokemonData> currentPokemons = pokemons;
            var gymInfo = await session.Client.Fort.GetGymDetails(currentFortData.Id, currentFortData.Latitude, currentFortData.Longitude);
            if (gymInfo.Result != GetGymDetailsResponse.Types.Result.Success)
            {
                return null;
            }
            var pokemonDatas = currentPokemons as PokemonData[] ?? currentPokemons.ToArray();
            var defendingPokemon = gymInfo.GymState.Memberships.First().PokemonData.Id;
            var attackerPokemons = pokemonDatas.Select(pokemon => pokemon.Id);
            var attackingPokemonIds = attackerPokemons as ulong[] ?? attackerPokemons.ToArray();
            Logger.Write(
                $"Attacking Gym: {gymInfo.Name}, DefendingPokemons:\n{ string.Join("\n", gymInfo.GymState.Memberships.Select(p => p.PokemonData.PokemonId).ToList()) }, \nAttacking: { gymInfo.GymState.Memberships.First().PokemonData.PokemonId }"
                );
            var result = await session.Client.Fort.StartGymBattle(currentFortData.Id, defendingPokemon, attackingPokemonIds);

            if (result.Result == StartGymBattleResponse.Types.Result.Success)
            {
                switch (result.BattleLog.State)
                {
                    case BattleState.Active:
                        session.EventDispatcher.Send(new GymBattleStarted { GymName = gymInfo.Name });
                        return result;
                    case BattleState.Defeated:
                        Logger.Write($"We were defeated in battle.");
                        return result;
                    case BattleState.Victory:
                        Logger.Write($"We were victorious");
                        //_pos = 0;
                        return result;
                    case BattleState.StateUnset:
                        Logger.Write($"Error occoured: {result.BattleLog.State}");
                        break;
                    case BattleState.TimedOut:
                        Logger.Write($"Error occoured: {result.BattleLog.State}");
                        break;
                    default:
                        Logger.Write($"Unhandled occoured: {result.BattleLog.State}");
                        break;
                }
            }
            else if (result.Result == StartGymBattleResponse.Types.Result.ErrorGymBattleLockout)
            {
                return result;
            }
            else if (result.Result == StartGymBattleResponse.Types.Result.ErrorAllPokemonFainted)
            {
                return result;
            }
            else if (result.Result == StartGymBattleResponse.Types.Result.Unset)
            {
                return result;
            }
            return result;
        }

        private static async Task EnsureJoinTeam(ISession session, PlayerData player)
        {
            if (session.Profile.PlayerData.Team == TeamColor.Neutral)
            {
                var defaultTeam = session.LogicSettings.GymDefaultTeam;
                var teamResponse = await session.Client.Player.SetPlayerTeam(defaultTeam);
                if (teamResponse.Status == SetPlayerTeamResponse.Types.Status.Success)
                {
                    player.Team = defaultTeam;
                }

                session.EventDispatcher.Send(new GymTeamJoinEvent()
                {
                    Team = defaultTeam,
                    Status = teamResponse.Status
                });
            }
        }

        private bool CanVisitGym()
        {
            return true;
        }



        private static async Task<PokemonData> GetDeployablePokemon(ISession session)
        {
            var pokemonList = (await session.Inventory.GetPokemons()).ToList();
            pokemonList = pokemonList.OrderByDescending(p => p.Cp).Skip(Math.Min(pokemonList.Count - 1, session.LogicSettings.GymNumberOfTopPokemonToBeExcluded)).ToList();

            if (pokemonList.Count == 1) return pokemonList.FirstOrDefault();
            if (session.LogicSettings.GymUseRandomPokemon)
            {

                return pokemonList.ElementAt(new Random().Next(0, pokemonList.Count - 1));
            }

            var pokemon = pokemonList.FirstOrDefault(p => p.Cp <= session.LogicSettings.GymMaxCPToDeploy && PokemonInfo.GetLevel(p) <= session.LogicSettings.GymMaxLevelToDeploy && string.IsNullOrEmpty(p.DeployedFortId));
            return pokemon;
        }
    }

}

