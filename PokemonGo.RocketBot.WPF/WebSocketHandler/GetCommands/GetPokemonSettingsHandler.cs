using System.Threading.Tasks;
using PokemonGo.RocketBot.Logic.State;
using PokemonGo.RocketBot.WPF.WebSocketHandler.GetCommands.Tasks;
using SuperSocket.WebSocket;

namespace PokemonGo.RocketBot.WPF.WebSocketHandler.GetCommands
{
    public class GetPokemonSettingsHandler : IWebSocketRequestHandler
    {
        public GetPokemonSettingsHandler()
        {
            Command = "GetPokemonSettings";
        }

        public string Command { get; }

        public async Task Handle(ISession session, WebSocketSession webSocketSession, dynamic message)
        {
            await GetPokemonSettingsTask.Execute(session, webSocketSession, (string) message.RequestID);
        }
    }
}