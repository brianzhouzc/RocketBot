using System.Threading;
using System.Threading.Tasks;
using NecroBot2.Logic.Common;
using NecroBot2.Logic.Logging;
using NecroBot2.Logic.State;
using POGOProtos.Inventory.Item;
using POGOProtos.Networking.Responses;

namespace NecroBot2.Logic.Tasks
{
    internal class UseIncenseConstantlyTask
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
            Logger.Write(session.Translation.GetTranslation(TranslationString.UseIncenseAmount, currentAmountOfIncense));

            var UseIncense = await session.Inventory.UseIncenseConstantly();

            if (UseIncense.Result == UseIncenseResponse.Types.Result.Success)
            {
                Logger.Write(session.Translation.GetTranslation(TranslationString.UsedIncense));
            }
            else if (UseIncense.Result == UseIncenseResponse.Types.Result.NoneInInventory)
            {
                Logger.Write(session.Translation.GetTranslation(TranslationString.NoIncenseAvailable));
            }
            else if (UseIncense.Result == UseIncenseResponse.Types.Result.IncenseAlreadyActive ||
                     (UseIncense.AppliedIncense == null))
            {
                Logger.Write(session.Translation.GetTranslation(TranslationString.UseIncenseActive));
            }
        }
    }
}