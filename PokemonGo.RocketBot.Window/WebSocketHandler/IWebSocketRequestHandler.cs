using PokemonGo.RocketBot.Logic.State;
using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketBot.Window.WebSocketHandler
{
    interface IWebSocketRequestHandler
    {
        string Command { get; }
        Task Handle(ISession session, WebSocketSession webSocketSession, dynamic message);
    }
}
