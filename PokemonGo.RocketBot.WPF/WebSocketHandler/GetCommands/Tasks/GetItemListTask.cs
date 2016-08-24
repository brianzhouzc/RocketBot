using System.Threading.Tasks;
using PokemonGo.RocketBot.Logic.State;
using PokemonGo.RocketBot.WPF.WebSocketHandler.GetCommands.Events;
using SuperSocket.WebSocket;

namespace PokemonGo.RocketBot.WPF.WebSocketHandler.GetCommands.Tasks
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