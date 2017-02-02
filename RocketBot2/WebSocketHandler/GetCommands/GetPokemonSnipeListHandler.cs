using System.Threading.Tasks;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Tasks;
using SuperSocket.WebSocket;

namespace RocketBot2.WebSocketHandler.GetCommands
{
    class GetPokemonSnipeListHandler : IWebSocketRequestHandler
    {
        public string Command { get; private set; }

        public GetPokemonSnipeListHandler()
        {
            Command = "PokemonSnipeList";
        }

        public async Task Handle(ISession session, WebSocketSession webSocketSession, dynamic message)
        {
            await HumanWalkSnipeTask.ExecuteFetchData(session);
            //await GetPokemonSnipeListTask.Execute(session, webSocketSession, (string)message.RequestID);
        }
    }
}