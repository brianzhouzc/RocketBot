#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NecroBot2.Logic.Event;
using NecroBot2.Logic.PoGoUtils;
using NecroBot2.Logic.State;
using NecroBot2.Logic.Utils;
using POGOProtos.Data;

#endregion

namespace NecroBot2.Logic.Tasks
{
    internal class TransferWeakPokemonTask
    {
        public static async Task Execute(ISession session, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var pokemons = await session.Inventory.GetPokemons();
            var pokemonDatas = pokemons as IList<PokemonData> ?? pokemons.ToList();
            var pokemonsFiltered =
                pokemonDatas.Where(pokemon => !session.LogicSettings.PokemonsNotToTransfer.Contains(pokemon.PokemonId))
                    .ToList().OrderBy(poke => poke.Cp);

            if (session.LogicSettings.KeepPokemonsThatCanEvolve)
                pokemonsFiltered =
                    pokemonDatas.Where(pokemon => !session.LogicSettings.PokemonsToEvolve.Contains(pokemon.PokemonId))
                        .ToList().OrderBy(poke => poke.Cp);

            var orderedPokemon = pokemonsFiltered.OrderBy(poke => poke.Cp);

            foreach (var pokemon in orderedPokemon)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if ((pokemon.Cp >= session.LogicSettings.KeepMinCp) ||
                    (PokemonInfo.CalculatePokemonPerfection(pokemon) >= session.LogicSettings.KeepMinIvPercentage &&
                     session.LogicSettings.PrioritizeIvOverCp) ||
                    (PokemonInfo.GetLevel(pokemon) >= session.LogicSettings.KeepMinLvl &&
                     session.LogicSettings.UseKeepMinLvl) ||
                    pokemon.Favorite == 1)
                    continue;

                await session.Client.Inventory.TransferPokemon(pokemon.Id);
                await session.Inventory.DeletePokemonFromInvById(pokemon.Id);
                var bestPokemonOfType = (session.LogicSettings.PrioritizeIvOverCp
                    ? await session.Inventory.GetHighestPokemonOfTypeByIv(pokemon)
                    : await session.Inventory.GetHighestPokemonOfTypeByCp(pokemon)) ?? pokemon;

                var setting = session.Inventory.GetPokemonSettings()
                    .Result.Single(q => q.PokemonId == pokemon.PokemonId);
                var family = session.Inventory.GetPokemonFamilies().Result.First(q => q.FamilyId == setting.FamilyId);

                family.Candy_++;

                session.EventDispatcher.Send(new TransferPokemonEvent
                {
                    Id = pokemon.PokemonId,
                    Perfection = PokemonInfo.CalculatePokemonPerfection(pokemon),
                    Cp = pokemon.Cp,
                    BestCp = bestPokemonOfType.Cp,
                    BestPerfection = PokemonInfo.CalculatePokemonPerfection(bestPokemonOfType),
                    FamilyCandies = family.Candy_
                });

                DelayingUtils.Delay(session.LogicSettings.DelayBetweenPlayerActions, 0);
            }
        }
    }
}