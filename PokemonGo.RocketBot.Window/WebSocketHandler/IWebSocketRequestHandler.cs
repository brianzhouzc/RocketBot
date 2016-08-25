using System.Threading.Tasks;
using PokemonGo.RocketBot.Logic.State;
using SuperSocket.WebSocket;

namespace PokemonGo.RocketBot.Window.WebSocketHandler
{
    internal interface IWebSocketRequestHandler
    {
        string Command { get; }
        Task Handle(ISession session, WebSocketSession webSocketSession, dynamic message);
    }
}