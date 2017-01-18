using POGOProtos.Networking.Responses;

namespace PoGo.NecroBot.Logic.Event.Player
{
    public class LoggedEvent : IEvent
    {
        public GetPlayerResponse Profile { get; internal set; }
    }
}