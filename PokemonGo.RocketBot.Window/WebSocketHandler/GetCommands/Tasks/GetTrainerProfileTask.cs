using PokemonGo.RocketBot.Window.WebSocketHandler.GetCommands.Events;
using PokemonGo.RocketBot.Window.WebSocketHandler.GetCommands.Helpers;
using PokemonGo.RocketBot.Logic.State;
using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketBot.Window.WebSocketHandler.GetCommands.Tasks
{
    class GetTrainerProfileTask
    {
        public static async Task Execute(ISession session, WebSocketSession webSocketSession, string requestID)
        {
            var playerStats = (await session.Inventory.GetPlayerStats()).FirstOrDefault();
            if (playerStats == null)
                return;
            var tmpData = new TrainerProfileWeb(session.Profile.PlayerData, playerStats);
            webSocketSession.Send(EncodingHelper.Serialize(new TrainerProfileResponce(tmpData, requestID)));
        }
    }
}
