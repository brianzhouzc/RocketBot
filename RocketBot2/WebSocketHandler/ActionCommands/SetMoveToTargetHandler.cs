using System.Threading.Tasks;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Tasks;
using SuperSocket.WebSocket;
using RocketBot2.WebSocketHandler;

namespace RocketoBot2.WebSocketHandler.ActionCommands
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
            await SetMoveToTargetTask.Execute((double) message.Latitude, (double) message.Longitude, (string) message.FortId
            );
        }
    }
}