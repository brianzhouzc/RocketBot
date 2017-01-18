#region using directives

using System.Threading.Tasks;
using RocketBot2.WebSocketHandler.GetCommands.Events;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Tasks;
using SuperSocket.WebSocket;

#endregion

namespace RocketBot2.WebSocketHandler.GetCommands.Tasks
{
    internal class GetPokemonSnipeListTask
    {
        public static async Task Execute(ISession session, WebSocketSession webSocketSession, string requestID)
        {
            var allItems = await HumanWalkSnipeTask.GetCurrentQueueItems(session);

            webSocketSession.Send(EncodingHelper.Serialize(new SnipeListResponce(allItems, requestID)));
        }
    }
}