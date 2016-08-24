using System.Threading.Tasks;
using PokemonGo.RocketBot.Logic.State;
using PokemonGo.RocketBot.Window.WebSocketHandler.GetCommands.Tasks;
using SuperSocket.WebSocket;

namespace PokemonGo.RocketBot.Window.WebSocketHandler.GetCommands
{
    internal class GetItemsListHandler : IWebSocketRequestHandler
    {
        public GetItemsListHandler()
        {
            Command = "GetItemsList";
        }

        public string Command { get; }

        public async Task Handle(ISession session, WebSocketSession webSocketSession, dynamic message)
        {
            await GetItemListTask.Execute(session, webSocketSession, (string) message.RequestID);
        }
    }
}