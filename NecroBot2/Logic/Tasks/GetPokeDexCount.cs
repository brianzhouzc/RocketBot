using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NecroBot2.Logic.Common;
using NecroBot2.Logic.Logging;
using NecroBot2.Logic.State;

namespace NecroBot2.Logic.Tasks
{
    internal class GetPokeDexCount
    {
        public static async Task Execute(ISession session, CancellationToken cancellationToken)
        {
            var PokeDex = await session.Inventory.GetPokeDexItems();
            var _totalUniqueEncounters =
                PokeDex.Select(
                    i =>
                        new
                        {
                            Pokemon = i.InventoryItemData.PokedexEntry.PokemonId,
                            Captures = i.InventoryItemData.PokedexEntry.TimesCaptured
                        });
            var _totalCaptures = _totalUniqueEncounters.Count(i => i.Captures > 0);
            var _totalData = PokeDex.Count();

            Logger.Write(session.Translation.GetTranslation(TranslationString.AmountPkmSeenCaught, _totalData,
                _totalCaptures));
        }
    }
}