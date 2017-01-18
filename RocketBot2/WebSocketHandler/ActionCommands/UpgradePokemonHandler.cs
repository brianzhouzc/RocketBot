#region using directives

using System.Threading.Tasks;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Tasks;
using SuperSocket.WebSocket;

#endregion

namespace RocketBot2.WebSocketHandler.ActionCommands
{
    public class UpgradePokemonHandler : IWebSocketRequestHandler
    {
        public UpgradePokemonHandler()
        {
            Command = "UpgradePokemon";
        }

        public string Command { get; }

        public async Task Handle(ISession session, WebSocketSession webSocketSession, dynamic message)
        {
            await UpgradeSinglePokemonTask.Execute(session, (ulong) message.PokemonId, (bool) message.Max);
        }
    }
}