#region using directives

using POGOProtos.Networking.Responses;

#endregion

namespace PoGo.NecroBot.Logic.Forms_Gui.Event
{
    public class ProfileEvent : IEvent
    {
        public GetPlayerResponse Profile;
    }
}