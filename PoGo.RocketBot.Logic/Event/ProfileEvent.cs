#region using directives

using POGOProtos.Networking.Responses;

#endregion

namespace PoGo.RocketBot.Logic.Event
{
    public class ProfileEvent : IEvent
    {
        public GetPlayerResponse Profile;
    }
}