using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Forms_Gui.State;
using PokemonGo.RocketBot.Window.WebSocketHandler.GetCommands.Events;
using SuperSocket.WebSocket;

namespace PokemonGo.RocketBot.Window.WebSocketHandler.GetCommands.Tasks
{
    internal class GetItemListTask
    {
        public static async Task Execute(ISession session, WebSocketSession webSocketSession, string requestID)
        {
            var allItems = await session.Inventory.GetItems();
            webSocketSession.Send(EncodingHelper.Serialize(new ItemListResponce(allItems, requestID)));
        }
    }
}