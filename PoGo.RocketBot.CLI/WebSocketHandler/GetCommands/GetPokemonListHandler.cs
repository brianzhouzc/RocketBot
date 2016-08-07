using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.WebSocket;
using PoGo.RocketBot.CLI.WebSocketHandler.GetCommands.Tasks;
using PoGo.RocketBot.Logic.State;

namespace PoGo.RocketBot.CLI.WebSocketHandler.GetCommands
{
    public class GetPokemonListHandler : IWebSocketRequestHandler
    {
        public string Command { get; private set;}

        public GetPokemonListHandler()
        {
            Command = "GetPokemonList";
        }

        public async Task Handle(ISession session, WebSocketSession webSocketSession, dynamic message)
        {
            await GetPokemonListTask.Execute(session, webSocketSession, (string)message.RequestID);
        }
    }
}
