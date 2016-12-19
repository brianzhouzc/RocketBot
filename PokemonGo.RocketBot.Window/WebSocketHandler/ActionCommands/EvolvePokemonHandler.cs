using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Forms_Gui.State;
using PoGo.NecroBot.Logic.Forms_Gui.Tasks;
using SuperSocket.WebSocket;

namespace PokemonGo.RocketBot.Window.WebSocketHandler.ActionCommands
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