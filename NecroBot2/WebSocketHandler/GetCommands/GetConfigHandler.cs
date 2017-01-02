#region using directives

using System.Threading.Tasks;
using NecroBot2.WebSocketHandler.GetCommands.Tasks;
using PoGo.NecroBot.Logic.State;
using SuperSocket.WebSocket;

#endregion

namespace NecroBot2.WebSocketHandler.GetCommands
{
    internal class GetConfigHandler : IWebSocketRequestHandler
    {
        public GetConfigHandler()
        {
            Command = "GetConfig";
        }

        public string Command { get; }

        public async Task Handle(ISession session, WebSocketSession webSocketSession, dynamic message)
        {
            await GetConfigTask.Execute(session, webSocketSession, (string) message.RequestID);
        }
    }
}