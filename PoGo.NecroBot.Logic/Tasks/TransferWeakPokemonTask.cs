#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.PoGoUtils;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Utils;
using POGOProtos.Data;

#endregion

namespace PoGo.NecroBot.Logic.Tasks
{
    public class TransferWeakPokemonTask
    {
        public static async Task Execute(ISession session, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!session.LogicSettings.TransferWeakPokemon) return;

            if (session.LogicSettings.AutoFavoritePokemon)
                await FavoritePokemonTask.Execute(session, cancellationToken);

            await EvolvePokemonTask.Execute(session, cancellationToken);

            var pokemons = await session.Inventory.GetPokemons();
            int buff = session.LogicSettings.BulkTransferStogareBuffer;
            //check for bag, if bag is nearly full, then process bulk transfer.
            var maxStorage = session.Profile.PlayerData.MaxPokemonStorage;
            var totalEggs = await session.Inventory.GetEggs();
            if ((maxStorage - totalEggs.Count() - buff) > pokemons.Count()) return;


            var pokemonDatas = pokemons as IList<PokemonData> ?? pokemons.ToList();
            var pokemonsFiltered =
                pokemonDatas.Where(pokemon => !session.LogicSettings.PokemonsNotToTransfer.Contains(pokemon.PokemonId))
                    .ToList().OrderBy( poke => poke.Cp );

            if (session.LogicSettings.KeepPokemonsThatCanEvolve)
                pokemonsFiltered =
                    pokemonDatas.Where(pokemon => !session.LogicSettings.PokemonsToEvolve.Contains(pokemon.PokemonId))
                        .ToList().OrderBy( poke => poke.Cp );

            var orderedPokemon = pokemonsFiltered.OrderBy( poke => poke.Cp );
            var pokemonToTransfers = new List<PokemonData>();
            foreach (var pokemon in orderedPokemon )
            {
                cancellationToken.ThrowIfCancellationRequested();
                if ((pokemon.Cp >= session.LogicSettings.KeepMinCp) ||
                    (PokemonInfo.CalculatePokemonPerfection(pokemon) >= session.LogicSettings.KeepMinIvPercentage &&
                     session.LogicSettings.PrioritizeIvOverCp) ||
                     (PokemonInfo.GetLevel(pokemon) >= session.LogicSettings.KeepMinLvl && session.LogicSettings.UseKeepMinLvl) ||
                    pokemon.Favorite == 1)
                    continue;

                if (session.LogicSettings.UseBulkTransferPokemon)
                {
                    pokemonToTransfers.Add(pokemon);
                }
                else {
                    await session.Client.Inventory.TransferPokemon(pokemon.Id);
                    await session.Inventory.DeletePokemonFromInvById(pokemon.Id);
                    await PrintTransferedPokemonInfo(session, pokemon);

                    await DelayingUtils.DelayAsync(session.LogicSettings.TransferActionDelay, 0, cancellationToken);
                }
            }
            if (session.LogicSettings.UseBulkTransferPokemon && pokemonToTransfers.Count >0)
            {
                int page = orderedPokemon.Count() / session.LogicSettings.BulkTransferSize + 1;
                for (int i = 0; i < page; i++)
                {
                    var batchTransfer = orderedPokemon.Skip(i * session.LogicSettings.BulkTransferSize).Take(session.LogicSettings.BulkTransferSize);
                    var t = await session.Client.Inventory.TransferPokemons(batchTransfer.Select(x => x.Id).ToList());
                    if (t.Result == POGOProtos.Networking.Responses.ReleasePokemonResponse.Types.Result.Success)
                    {
                        foreach (var duplicatePokemon in batchTransfer)
                        {
                            await session.Inventory.DeletePokemonFromInvById(duplicatePokemon.Id);
                            await PrintTransferedPokemonInfo(session, duplicatePokemon);
                        }
                    }
                    else session.EventDispatcher.Send(new WarnEvent() { Message = session.Translation.GetTranslation(Common.TranslationString.BulkTransferFailed, orderedPokemon.Count()) });
                }
            }
        }

        private static async Task PrintTransferedPokemonInfo(ISession session, PokemonData pokemon)
        {
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
        }
    }
}