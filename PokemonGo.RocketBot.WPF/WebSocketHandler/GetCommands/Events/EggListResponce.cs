namespace PokemonGo.RocketBot.WPF.WebSocketHandler.GetCommands.Events
{
    public class EggListResponce : IWebSocketResponce
    {
        public EggListResponce(dynamic data, string requestID)
        {
            Command = "EggListWeb";
            Data = data;
            RequestID = requestID;
        }

        public string RequestID { get; }
        public string Command { get; }
        public dynamic Data { get; }
    }
}