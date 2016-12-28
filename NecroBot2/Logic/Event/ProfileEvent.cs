#region using directives

using PoGo.NecroBot.Logic.Event;
using POGOProtos.Networking.Responses;

#endregion

namespace NecroBot2.Logic.Event
{
    public class ProfileEvent : IEvent
    {
        public GetPlayerResponse Profile;
    }
}