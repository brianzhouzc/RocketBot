using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Forms_Gui.State;
using PokemonGo.RocketBot.Window.WebSocketHandler.GetCommands.Tasks;
using SuperSocket.WebSocket;

namespace PokemonGo.RocketBot.Window.WebSocketHandler.GetCommands
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