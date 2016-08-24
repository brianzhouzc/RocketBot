using System.Linq;
using System.Threading.Tasks;
using PokemonGo.RocketBot.Logic.State;
using PokemonGo.RocketBot.WPF.WebSocketHandler.GetCommands.Events;
using PokemonGo.RocketBot.WPF.WebSocketHandler.GetCommands.Helpers;
using SuperSocket.WebSocket;

namespace PokemonGo.RocketBot.WPF.WebSocketHandler.GetCommands.Tasks
{
    internal class GetTrainerProfileTask
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