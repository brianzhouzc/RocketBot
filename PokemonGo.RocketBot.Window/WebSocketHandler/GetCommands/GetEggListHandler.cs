using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Forms_Gui.State;
using PokemonGo.RocketBot.Window.WebSocketHandler.GetCommands.Tasks;
using SuperSocket.WebSocket;

namespace PokemonGo.RocketBot.Window.WebSocketHandler.GetCommands
{
    internal class GetEggListHandler : IWebSocketRequestHandler
    {
        public GetEggListHandler()
        {
            Command = "GetEggList";
        }

        public string Command { get; }

        public async Task Handle(ISession session, WebSocketSession webSocketSession, dynamic message)
        {
            await GetEggListTask.Execute(session, webSocketSession, (string) message.RequestID);
        }
    }
}