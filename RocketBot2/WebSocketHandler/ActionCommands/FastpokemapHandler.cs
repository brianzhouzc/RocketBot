#region using directives

using System.Threading.Tasks;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Tasks;
using SuperSocket.WebSocket;

#endregion

namespace RocketBot2.WebSocketHandler.ActionCommands
{
    public class FastpokemapHandler : IWebSocketRequestHandler
    {
        public FastpokemapHandler()
        {
            Command = "Fastpokemap";
        }

        public string Command { get; }

        public async Task Handle(ISession session, WebSocketSession webSocketSession, dynamic message)
        {
            CatchState.AddFastPokemapItem(message.Data);

            await HumanWalkSnipeTask.AddFastPokemapItem(message.Data);
            //Console.WriteLine(JsonConvert.DeserializeObject(message.Data));
        }
    }
}