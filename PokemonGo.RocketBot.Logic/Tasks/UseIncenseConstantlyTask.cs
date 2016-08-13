using System.Threading;
using System.Threading.Tasks;
using POGOProtos.Inventory.Item;
using PokemonGo.RocketBot.Logic.Common;
using PokemonGo.RocketBot.Logic.Logging;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketBot.Logic.State;

namespace PokemonGo.RocketBot.Logic.Tasks
{
    class UseIncenseConstantlyTask
    {
        public static async Task Execute(ISession session, CancellationToken cancellationToken)
        {
            if (!session.LogicSettings.UseIncenseConstantly)
                return;

            var currentAmountOfIncense = await session.Inventory.GetItemAmountByType(ItemId.ItemIncenseOrdinary);
            if (currentAmountOfIncense == 0)
            {
                Logger.Write(session.Translation.GetTranslation(TranslationString.NoIncenseAvailable));
                return;
            }
            else
            {
                Logger.Write(session.Translation.GetTranslation(TranslationString.UseIncenseAmount, currentAmountOfIncense));
            }

            var UseIncense = await session.Inventory.UseIncenseConstantly();

            if (UseIncense.Result == UseIncenseResponse.Types.Result.Success)
            {
                Logger.Write(session.Translation.GetTranslation(TranslationString.UsedIncense));
            }
            else if (UseIncense.Result == UseIncenseResponse.Types.Result.NoneInInventory)
            {
                Logger.Write(session.Translation.GetTranslation(TranslationString.NoIncenseAvailable));
            }
            else if (UseIncense.Result == UseIncenseResponse.Types.Result.IncenseAlreadyActive || (UseIncense.AppliedIncense == null))
            {
                Logger.Write(session.Translation.GetTranslation(TranslationString.UseIncenseActive));
            }
        }
    }
}
