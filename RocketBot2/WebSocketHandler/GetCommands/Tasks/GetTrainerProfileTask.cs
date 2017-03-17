#region using directives

using System.Linq;
using System.Threading.Tasks;
using RocketBot2.WebSocketHandler.GetCommands.Events;
using RocketBot2.WebSocketHandler.GetCommands.Helpers;
using PoGo.NecroBot.Logic.State;
using SuperSocket.WebSocket;

#endregion

namespace RocketBot2.WebSocketHandler.GetCommands.Tasks
{
    internal class GetTrainerProfileTask
    {
        public static async Task Execute(ISession session, WebSocketSession webSocketSession, string requestID)
        {
            //using (var blocker = new BlockableScope(session, BotActions.GetProfile))
            {
                // if (!await blocker.WaitToRun()) return;

                var playerStats = (await session.Inventory.GetPlayerStats()).FirstOrDefault();
                if (playerStats == null)
                    return;
                var tmpData = new TrainerProfileWeb(session.Profile.PlayerData, playerStats);
                webSocketSession.Send(EncodingHelper.Serialize(new TrainerProfileResponce(tmpData, requestID)));
            }
        }
    }
}