using PokemonGo.RocketBot.Window.WebSocketHandler.GetCommands.Events;
using PokemonGo.RocketBot.Logic.State;
using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.RocketBot.Window.WebSocketHandler.GetCommands.Helpers;

namespace PokemonGo.RocketBot.Window.WebSocketHandler.GetCommands.Tasks
{
    class GetPokemonListTask
    {
    
        public static async Task Execute(ISession session, WebSocketSession webSocketSession, string requestID)
        {
            var allPokemonInBag = await session.Inventory.GetHighestsCp(1000);
            var list = new List<PokemonListWeb>();
            allPokemonInBag.ToList().ForEach(o => list.Add(new PokemonListWeb(o)));
            webSocketSession.Send(EncodingHelper.Serialize(new PokemonListResponce(list,requestID)));
        }

    }
}
