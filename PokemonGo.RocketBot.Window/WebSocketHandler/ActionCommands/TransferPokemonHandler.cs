using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Forms_Gui.State;
using PoGo.NecroBot.Logic.Forms_Gui.Tasks;
using SuperSocket.WebSocket;

namespace PokemonGo.RocketBot.Window.WebSocketHandler.ActionCommands
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