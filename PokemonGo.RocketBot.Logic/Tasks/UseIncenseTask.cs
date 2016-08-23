using System;
using System.Threading.Tasks;
using PokemonGo.RocketBot.Logic.Logging;
using PokemonGo.RocketBot.Logic.State;
using POGOProtos.Inventory.Item;
using POGOProtos.Networking.Responses;

namespace PokemonGo.RocketBot.Logic.Tasks
{
    public class UseIncenseTask
    {
        public static async Task Execute(Session session)
        {
            var response = await session.Client.Inventory.UseIncense(ItemId.ItemIncenseOrdinary);
            switch (response.Result)
            {
                case UseIncenseResponse.Types.Result.Success:
                    Logger.Write($"Incense valid until: {DateTime.Now.AddMinutes(30)}");
                    break;
                case UseIncenseResponse.Types.Result.IncenseAlreadyActive:
                    Logger.Write($"An incense is already active!", LogLevel.Warning);
                    break;
                case UseIncenseResponse.Types.Result.LocationUnset:
                    Logger.Write($"Bot must be running first!", LogLevel.Error);
                    break;
                case UseIncenseResponse.Types.Result.Unknown:
                    break;
                case UseIncenseResponse.Types.Result.NoneInInventory:
                    break;
                default:
                    Logger.Write($"Failed using an incense!", LogLevel.Error);
                    break;
            }
        }
    }
}