#region using directives

using System.Collections.Generic;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Tasks;
using SuperSocket.WebSocket;

#endregion

namespace RocketBot2.WebSocketHandler.ActionCommands
{
    public class TransferPokemonHandler : IWebSocketRequestHandler
    {
        public TransferPokemonHandler()
        {
            Command = "TransferPokemon";
        }

        public string Command { get; }

        public async Task Handle(ISession session, WebSocketSession webSocketSession, dynamic message)
        {
            await TransferPokemonTask.Execute(
                session,
                session.CancellationTokenSource.Token,
                new List<ulong>
                {
                    (ulong) message.PokemonId
                }
            );
        }
    }
}