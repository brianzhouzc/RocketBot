#region using directives

using System.Threading.Tasks;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Tasks;
using SuperSocket.WebSocket;

#endregion

namespace NecroBot2.WebSocketHandler.ActionCommands
{
    public class HumanSnipeRemovePokemonHandler : IWebSocketRequestHandler
    {
        public HumanSnipeRemovePokemonHandler()
        {
            Command = "RemovePokemon";
        }

        public string Command { get; }

        public async Task Handle(ISession session, WebSocketSession webSocketSession, dynamic message)
        {
            await HumanWalkSnipeTask.RemovePokemonFromQueue(session, (string) message.Id);
        }
    }
}