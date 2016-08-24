using System.Threading.Tasks;
using PokemonGo.RocketBot.Logic.State;
using PokemonGo.RocketBot.Logic.Tasks;
using SuperSocket.WebSocket;

namespace PokemonGo.RocketBot.WPF.WebSocketHandler.ActionCommands
{
    public class EvolvePokemonHandler : IWebSocketRequestHandler
    {
        public EvolvePokemonHandler()
        {
            Command = "EvolvePokemon";
        }

        public string Command { get; }

        public async Task Handle(ISession session, WebSocketSession webSocketSession, dynamic message)
        {
            await EvolveSpecificPokemonTask.Execute(session, (ulong) message.PokemonId);
        }
    }
}