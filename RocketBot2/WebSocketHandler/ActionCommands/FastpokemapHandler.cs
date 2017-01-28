#region using directives

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using PoGo.NecroBot.Logic.State;
using SuperSocket.WebSocket;
using PoGo.NecroBot.Logic.Tasks;

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