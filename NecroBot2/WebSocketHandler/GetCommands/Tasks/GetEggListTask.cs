#region using directives

using System.Linq;
using System.Threading.Tasks;
using NecroBot2.WebSocketHandler.GetCommands.Events;
using NecroBot2.WebSocketHandler.GetCommands.Helpers;
using PoGo.NecroBot.Logic.State;
using POGOProtos.Inventory.Item;
using SuperSocket.WebSocket;
using PoGo.NecroBot.Logic.Model;

#endregion

namespace NecroBot2.WebSocketHandler.GetCommands.Tasks
{
    internal class GetEggListTask
    {
        public static async Task Execute(ISession session, WebSocketSession webSocketSession, string requestID)
        {
           // using (var blocker = new BlockableScope(session, BotActions.Eggs))
            {
               // if (!await blocker.WaitToRun()) return;

                var incubators = (await session.Inventory.GetEggIncubators())
                .Where(x => x.UsesRemaining > 0 || x.ItemId == ItemId.ItemIncubatorBasicUnlimited)
                .OrderByDescending(x => x.ItemId == ItemId.ItemIncubatorBasicUnlimited)
                .ToList();

                var unusedEggs = (await session.Inventory.GetEggs())
                    .Where(x => string.IsNullOrEmpty(x.EggIncubatorId))
                    .OrderBy(x => x.EggKmWalkedTarget - x.EggKmWalkedStart)
                    .ToList();


                var list = new EggListWeb
                {
                    Incubators = incubators,
                    UnusedEggs = unusedEggs
                };
                webSocketSession.Send(EncodingHelper.Serialize(new EggListResponce(list, requestID)));
            }
        }
    }
}