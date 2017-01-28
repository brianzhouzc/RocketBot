#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RocketBot2.WebSocketHandler.GetCommands.Events;
using RocketBot2.WebSocketHandler.GetCommands.Helpers;
using PoGo.NecroBot.Logic.State;
using SuperSocket.WebSocket;

#endregion

namespace RocketBot2.WebSocketHandler.GetCommands.Tasks
{
    internal class GetPokemonListTask
    {
        public static async Task Execute(ISession session, WebSocketSession webSocketSession, string requestID)
        {
            //using (var blocker = new BlockableScope(session, BotActions.ListItems))
            {
                //if (!await blocker.WaitToRun()) return;

                var allPokemonInBag = session.Inventory.GetHighestsCp(1000);
                var families = await session.Inventory.GetPokemonFamilies();
                var settings = await session.Inventory.GetPokemonSettings();
                var list = new List<PokemonListWeb>();
                allPokemonInBag.ToList().ForEach(o => list.Add(new PokemonListWeb(o, families, settings)));
                webSocketSession.Send(EncodingHelper.Serialize(new PokemonListResponce(list, requestID)));
            }
        }
    }
}