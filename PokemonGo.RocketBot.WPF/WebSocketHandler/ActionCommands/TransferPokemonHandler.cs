using System.Threading.Tasks;
using PokemonGo.RocketBot.Logic.State;
using PokemonGo.RocketBot.Logic.Tasks;
using SuperSocket.WebSocket;

namespace PokemonGo.RocketBot.WPF.WebSocketHandler.ActionCommands
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
            await TransferSpecificPokemonTask.Execute(session, (ulong) message.PokemonId);
        }
    }
}