using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.WebSocket;
using PokemonGo.RocketBot.Window.WebSocketHandler.GetCommands.Tasks;
using PokemonGo.RocketBot.Logic.State;

namespace PokemonGo.RocketBot.Window.WebSocketHandler.GetCommands
{
    public class GetPokemonSettingsHandler : IWebSocketRequestHandler
    {
        public string Command { get; private set;}

        public GetPokemonSettingsHandler()
        {
            Command = "GetPokemonSettings";
        }

        public async Task Handle(ISession session, WebSocketSession webSocketSession, dynamic message)
        {
            await GetPokemonSettingsTask.Execute(session, webSocketSession, (string)message.RequestID);
        }
    }
}
