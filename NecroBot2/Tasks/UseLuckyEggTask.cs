using System;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Logging;
using PoGo.NecroBot.Logic.State;
using POGOProtos.Networking.Responses;

namespace NecroBot2.Tasks
{
    public class UseLuckyEggTask
    {
        public static async Task Execute(Session session)
        {
            var response = await session.Client.Inventory.UseItemXpBoost();
            switch (response.Result)
            {
                case UseItemXpBoostResponse.Types.Result.Success:
                    Logger.Write($"Using a Lucky Egg");
                    Logger.Write($"Lucky Egg valid until: {DateTime.Now.AddMinutes(30)}");
                    break;
                case UseItemXpBoostResponse.Types.Result.ErrorXpBoostAlreadyActive:
                    Logger.Write($"A Lucky Egg is already active!", LogLevel.Warning);
                    break;
                case UseItemXpBoostResponse.Types.Result.ErrorLocationUnset:
                    Logger.Write($"Bot must be running first!", LogLevel.Error);
                    break;
                case UseItemXpBoostResponse.Types.Result.Unset:
                    break;
                case UseItemXpBoostResponse.Types.Result.ErrorInvalidItemType:
                    break;
                case UseItemXpBoostResponse.Types.Result.ErrorNoItemsRemaining:
                    break;
                default:
                    Logger.Write($"Failed using a Lucky Egg!", LogLevel.Error);
                    break;
            }
        }
    }
}