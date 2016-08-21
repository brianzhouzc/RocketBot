using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PokemonGo.RocketBot.Logic.Common;
using PokemonGo.RocketBot.Logic.Logging;
using PokemonGo.RocketBot.Logic.State;

namespace PokemonGo.RocketBot.Logic.Tasks
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