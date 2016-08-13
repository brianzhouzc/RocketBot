using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.WebSocket;
using PokemonGo.RocketBot.Window.WebSocketHandler.GetCommands.Tasks;
using PokemonGo.RocketBot.Logic.State;

namespace PokemonGo.RocketBot.Window.WebSocketHandler.ActionCommands
{
    public class TransferPokemonHandler : IWebSocketRequestHandler
    {
        public string Command { get; private set;}

        public TransferPokemonHandler()
        {
            Command = "TransferPokemon";
        }

        public async Task Handle(ISession session, WebSocketSession webSocketSession, dynamic message)
        {
            await Logic.Tasks.TransferPokemonTask.Execute(session, (ulong)message.PokemonId);
        }
    }
}
