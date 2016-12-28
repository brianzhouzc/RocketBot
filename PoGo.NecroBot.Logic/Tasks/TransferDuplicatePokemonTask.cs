#region using directives

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.PoGoUtils;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Utils;

#endregion

namespace PoGo.NecroBot.Logic.Tasks
{
    public class TransferDuplicatePokemonTask
    {
        public static async Task Execute(ISession session, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!session.LogicSettings.TransferDuplicatePokemon) return;
            if (session.LogicSettings.UseBulkTransferPokemon)
            {
                int buff = session.LogicSettings.BulkTransferStogareBuffer;
                //check for bag, if bag is nearly full, then process bulk transfer.
                var maxStorage = session.Profile.PlayerData.MaxPokemonStorage;
                var totalPokemon = (await session.Inventory.GetPokemons());
                var totalEggs = await session.Inventory.GetEggs();
                if ((maxStorage - totalEggs.Count() - buff) > totalPokemon.Count()) return;
            }

            if (session.LogicSettings.AutoFavoritePokemon)
                await FavoritePokemonTask.Execute(session, cancellationToken);

            await EvolvePokemonTask.Execute(session, cancellationToken);

            //if (session.LogicSettings.EvolveAllPokemonWithEnoughCandy ||
            //          session.LogicSettings.EvolveAllPokemonAboveIv ||
            //          session.LogicSettings.UseLuckyEggsWhileEvolving ||
            //          session.LogicSettings.KeepPokemonsThatCanEvolve)
            //    await session.Inventory.RefreshCachedInventory();

            var duplicatePokemons =
                await
                    session.Inventory.GetDuplicatePokemonToTransfer(
                        session.LogicSettings.PokemonsNotToTransfer,
                        session.LogicSettings.PokemonsToEvolve,
                        session.LogicSettings.KeepPokemonsThatCanEvolve,
                        session.LogicSettings.PrioritizeIvOverCp);

            var orderedPokemon = duplicatePokemons.OrderBy(poke => poke.Cp);

            if (orderedPokemon.Count() == 0) return;

            var pokemonSettings = await session.Inventory.GetPokemonSettings();
            var pokemonFamilies = await session.Inventory.GetPokemonFamilies();

            if (session.LogicSettings.UseBulkTransferPokemon)
            {
                var t = await session.Client.Inventory.TransferPokemons(orderedPokemon.Select(x => x.Id).ToList());
                if (t.Result == POGOProtos.Networking.Responses.ReleasePokemonResponse.Types.Result.Success)
                {
                    foreach (var duplicatePokemon in orderedPokemon)
                    {
                        await session.Inventory.DeletePokemonFromInvById(duplicatePokemon.Id);
                        await PrintPokemonInfo(session, pokemonSettings, pokemonFamilies, duplicatePokemon);
                    }
                }
                else session.EventDispatcher.Send(new WarnEvent() { Message = session.Translation.GetTranslation(Common.TranslationString.BulkTransferFailed, orderedPokemon.Count()) });
            }
            else
                foreach (var duplicatePokemon in orderedPokemon)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await session.Client.Inventory.TransferPokemon(duplicatePokemon.Id);
                    await session.Inventory.DeletePokemonFromInvById(duplicatePokemon.Id);

                    await PrintPokemonInfo(session, pokemonSettings, pokemonFamilies, duplicatePokemon);

                    // Padding the TransferEvent with player-choosen delay before instead of after.
                    // This is to remedy too quick transfers, often happening within a second of the
                    // previous action otherwise

                    await DelayingUtils.DelayAsync(session.LogicSettings.TransferActionDelay, 0, cancellationToken);
                }
        }

        private static async Task PrintPokemonInfo(ISession session, System.Collections.Generic.IEnumerable<POGOProtos.Settings.Master.PokemonSettings> pokemonSettings, System.Collections.Generic.List<POGOProtos.Inventory.Candy> pokemonFamilies, POGOProtos.Data.PokemonData duplicatePokemon)
        {
            var bestPokemonOfType = (session.LogicSettings.PrioritizeIvOverCp
                ? await session.Inventory.GetHighestPokemonOfTypeByIv(duplicatePokemon)
                : await session.Inventory.GetHighestPokemonOfTypeByCp(duplicatePokemon)) ?? duplicatePokemon;

            var setting = pokemonSettings.SingleOrDefault(q => q.PokemonId == duplicatePokemon.PokemonId);
            var family = pokemonFamilies.FirstOrDefault(q => q.FamilyId == setting.FamilyId);

            family.Candy_++;

            session.EventDispatcher.Send(new TransferPokemonEvent
            {
                Id = duplicatePokemon.PokemonId,
                Perfection = PokemonInfo.CalculatePokemonPerfection(duplicatePokemon),
                Cp = duplicatePokemon.Cp,
                BestCp = bestPokemonOfType.Cp,
                BestPerfection = PokemonInfo.CalculatePokemonPerfection(bestPokemonOfType),
                FamilyCandies = family.Candy_
            });
        }
    }
}