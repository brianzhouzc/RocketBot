using System.Threading.Tasks;
using PokemonGo.RocketBot.Logic.State;
using PokemonGo.RocketBot.WPF.WebSocketHandler.GetCommands.Tasks;
using SuperSocket.WebSocket;

namespace PokemonGo.RocketBot.WPF.WebSocketHandler.GetCommands
{
    public class GetPokemonListHandler : IWebSocketRequestHandler
    {
        public GetPokemonListHandler()
        {
            Command = "GetPokemonList";
        }

        public string Command { get; }

        public async Task Handle(ISession session, WebSocketSession webSocketSession, dynamic message)
        {
            await GetPokemonListTask.Execute(session, webSocketSession, (string) message.RequestID);
        }
    }
}