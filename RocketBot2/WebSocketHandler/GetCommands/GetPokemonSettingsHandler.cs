#region using directives

using System.Threading.Tasks;
using RocketBot2.WebSocketHandler.GetCommands.Tasks;
using PoGo.NecroBot.Logic.State;
using SuperSocket.WebSocket;

#endregion

namespace RocketBot2.WebSocketHandler.GetCommands
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