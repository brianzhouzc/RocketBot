#region using directives

using System.Linq;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Utils;
using System.Threading;
using POGOProtos.Data;
using System.Collections.Generic;

#endregion

namespace PoGo.NecroBot.Logic.Tasks
{
    public class TransferPokemonTask
    {
        public static async Task Execute(ISession session, CancellationToken cancellationToken, List<ulong> pokemonIds)
        {
            using (var blocker = new BlockableScope(session, Model.BotActions.Transfer))
            {
                if (!await blocker.WaitToRun()) return;

                var all = await session.Inventory.GetPokemons();
                List<PokemonData> pokemonToTransfer = new List<PokemonData>();
                var pokemons = all.OrderBy(x => x.Cp).ThenBy(n => n.StaminaMax);

                foreach (var item in pokemonIds)
                {
                    var pokemon = pokemons.FirstOrDefault(p => p.Id == item);

                    if (pokemon == null) return;
                    pokemonToTransfer.Add(pokemon);
                }

                var pokemonSettings = await session.Inventory.GetPokemonSettings();
                var pokemonFamilies = await session.Inventory.GetPokemonFamilies();

                await session.Client.Inventory.TransferPokemons(pokemonIds);

                foreach (var pokemon in pokemonToTransfer)
                {
                    await session.Inventory.DeletePokemonFromInvById(pokemon.Id);
                    var bestPokemonOfType = (session.LogicSettings.PrioritizeIvOverCp
                  ? await session.Inventory.GetHighestPokemonOfTypeByIv(pokemon)
                  : await session.Inventory.GetHighestPokemonOfTypeByCp(pokemon)) ?? pokemon;

                    var setting = pokemonSettings.Single(q => q.PokemonId == pokemon.PokemonId);
                    var family = pokemonFamilies.First(q => q.FamilyId == setting.FamilyId);

                    family.Candy_++;

                    // Broadcast event as everyone would benefit
                    session.EventDispatcher.Send(new Logic.Event.TransferPokemonEvent
                    {
                        Id = pokemon.Id,
                        PokemonId = pokemon.PokemonId,
                        Perfection = Logic.PoGoUtils.PokemonInfo.CalculatePokemonPerfection(pokemon),
                        Cp = pokemon.Cp,
                        BestCp = bestPokemonOfType.Cp,
                        BestPerfection = Logic.PoGoUtils.PokemonInfo.CalculatePokemonPerfection(bestPokemonOfType),
                        FamilyCandies = family.Candy_  ,
                        FamilyId = family.FamilyId
                    });
                }

                await DelayingUtils.DelayAsync(session.LogicSettings.TransferActionDelay, 0, cancellationToken);
            }
        }
    }
}