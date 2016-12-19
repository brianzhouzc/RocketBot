using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Forms_Gui.State;
using PokemonGo.RocketBot.Window.WebSocketHandler.GetCommands.Events;
using SuperSocket.WebSocket;

namespace PokemonGo.RocketBot.Window.WebSocketHandler.GetCommands.Tasks
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