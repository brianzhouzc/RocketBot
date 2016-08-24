namespace PokemonGo.RocketBot.WPF.WebSocketHandler.GetCommands.Events
{
    internal class ItemListResponce : IWebSocketResponce
    {
        public ItemListResponce(dynamic data, string requestID)
        {
            Command = "ItemListWeb";
            Data = data;
            RequestID = requestID;
        }

        public string RequestID { get; }
        public string Command { get; }
        public dynamic Data { get; }
    }
}