namespace PokemonGo.RocketBot.WPF.WebSocketHandler
{
    internal interface IWebSocketResponce
    {
        string RequestID { get; }
        string Command { get; }
        dynamic Data { get; }
    }
}