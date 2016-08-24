namespace PokemonGo.RocketBot.WPF.WebSocketHandler.GetCommands.Events
{
    internal class TrainerProfileResponce : IWebSocketResponce
    {
        public TrainerProfileResponce(dynamic data, string requestID)
        {
            Command = "TrainerProfile";
            Data = data;
            RequestID = requestID;
        }

        public string RequestID { get; }
        public string Command { get; }
        public dynamic Data { get; }
    }
}