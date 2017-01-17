using System.Threading.Tasks;
using PoGo.NecroBot.Logic.State;
using SuperSocket.WebSocket;
using PoGo.NecroBot.Logic.Tasks;

namespace RocketBot2.WebSocketHandler.ActionCommands
{
    public class SetMoveToTargetHandler : IWebSocketRequestHandler
    {
        public string Command { get; private set; }

        public SetMoveToTargetHandler()
        {
            Command = "SetMoveToTarget";
        }

        public async Task Handle(ISession session, WebSocketSession webSocketSession, dynamic message)
        {
            await SetMoveToTargetTask.Execute(session,(double)message.Latitude, (double)message.Longitude, (string)message.FortId);
        }
    }
}