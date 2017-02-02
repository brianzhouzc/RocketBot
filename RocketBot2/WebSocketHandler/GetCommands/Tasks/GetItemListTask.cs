#region using directives

using System.Threading.Tasks;
using RocketBot2.WebSocketHandler.GetCommands.Events;
using PoGo.NecroBot.Logic.State;
using SuperSocket.WebSocket;

#endregion

namespace RocketBot2.WebSocketHandler.GetCommands.Tasks
{
    internal class GetItemListTask
    {
        // jjskuld - Ignore CS1998 warning for now.
        #pragma warning disable 1998
        public static async Task Execute(ISession session, WebSocketSession webSocketSession, string requestID)
        {
            // using (var blocker = new BlockableScope(session, BotActions.ListItems))
            {
                //if (!await blocker.WaitToRun()) return;

                var allItems = session.Inventory.GetItems();
                webSocketSession.Send(EncodingHelper.Serialize(new ItemListResponce(allItems, requestID)));
            }
        }
        #pragma warning restore 1998
    }
}