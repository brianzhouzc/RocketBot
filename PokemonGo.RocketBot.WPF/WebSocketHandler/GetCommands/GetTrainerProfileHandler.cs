using System.Threading.Tasks;
using PokemonGo.RocketBot.Logic.State;
using PokemonGo.RocketBot.WPF.WebSocketHandler.GetCommands.Tasks;
using SuperSocket.WebSocket;

namespace PokemonGo.RocketBot.WPF.WebSocketHandler.GetCommands
{
    internal class GetTrainerProfileHandler : IWebSocketRequestHandler
    {
        public GetTrainerProfileHandler()
        {
            Command = "GetTrainerProfile";
        }

        public string Command { get; }

        public async Task Handle(ISession session, WebSocketSession webSocketSession, dynamic message)
        {
            await GetTrainerProfileTask.Execute(session, webSocketSession, (string) message.RequestID);
        }
    }
}