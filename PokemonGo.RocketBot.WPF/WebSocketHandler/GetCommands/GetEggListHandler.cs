using System.Threading.Tasks;
using PokemonGo.RocketBot.Logic.State;
using PokemonGo.RocketBot.WPF.WebSocketHandler.GetCommands.Tasks;
using SuperSocket.WebSocket;

namespace PokemonGo.RocketBot.WPF.WebSocketHandler.GetCommands
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