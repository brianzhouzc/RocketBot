using PokemonGo.RocketBot.Window.WebSocketHandler.GetCommands.Events;
using PokemonGo.RocketBot.Logic.State;
using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketBot.Window.WebSocketHandler.GetCommands.Tasks
{
    class GetPokemonSettingsTask
    {
    
        public static async Task Execute(ISession session, WebSocketSession webSocketSession, string requestID)
        {
            var settings = await session.Inventory.GetPokemonSettings();
            webSocketSession.Send(EncodingHelper.Serialize(new WebResponce()
            {
                Command = "PokemonSettings",
                Data = settings,
                RequestID = requestID
            }));
        }

    }
}
