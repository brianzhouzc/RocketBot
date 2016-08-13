using PokemonGo.RocketBot.Window.WebSocketHandler.GetCommands.Tasks;
using PokemonGo.RocketBot.Logic.State;
using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketBot.Window.WebSocketHandler.GetCommands
{
    class GetEggListHandler : IWebSocketRequestHandler
    {
        public string Command { get; private set; }

        public GetEggListHandler()
        {
            Command = "GetEggList";
        }

        public async Task Handle(ISession session, WebSocketSession webSocketSession, dynamic message)
        {
            await GetEggListTask.Execute(session, webSocketSession, (string)message.RequestID);
        }
    }
}
