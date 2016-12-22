using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Forms_Gui.State;
using SuperSocket.WebSocket;

namespace PokemonGo.RocketBot.Window.WebSocketHandler
{
    internal interface IWebSocketRequestHandler
    {
        string Command { get; }
        Task Handle(ISession session, WebSocketSession webSocketSession, dynamic message);
    }
}