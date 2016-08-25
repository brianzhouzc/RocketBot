using System.Threading.Tasks;
using PokemonGo.RocketBot.Logic.Logging;
using PokemonGo.RocketBot.Logic.State;
using POGOProtos.Inventory.Item;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketBot.Logic.Utils;

namespace PokemonGo.RocketBot.Logic.Tasks
{
    public class RecycleSpecificItemTask
    {
        public static async Task Execute(Session session, ItemId itemId, int count)
        {
            var response = await session.Client.Inventory.RecycleItem(itemId, count);
            if (response.Result == RecycleInventoryItemResponse.Types.Result.Success)
            {
                Logger.Write(
                    $"Recycled {count}x {itemId.ToString().Substring(4)}",
                    LogLevel.Recycling);
            }
            else
            {
                Logger.Write(
                    $"Unable to recycle {count}x {itemId.ToString().Substring(4)}",
                    LogLevel.Error);
            }
            DelayingUtils.Delay(session.LogicSettings.DelayBetweenPlayerActions, 500);
        }
    }
}