using System.Threading.Tasks;
using PokemonGo.RocketBot.Logic.State;
using PokemonGo.RocketBot.WPF.WebSocketHandler.GetCommands.Events;
using SuperSocket.WebSocket;

namespace PokemonGo.RocketBot.WPF.WebSocketHandler.GetCommands.Tasks
{
    internal class GetPokemonSettingsTask
    {
        public static async Task Execute(ISession session, WebSocketSession webSocketSession, string requestID)
        {
            var settings = await session.Inventory.GetPokemonSettings();
            webSocketSession.Send(EncodingHelper.Serialize(new WebResponce
            {
                Command = "PokemonSettings",
                Data = settings,
                RequestID = requestID
            }));
        }
    }
}