#region using directives

using System.Collections.Generic;
using POGOProtos.Data.Player;
using POGOProtos.Networking.Responses;

#endregion

namespace PoGo.NecroBot.Logic.Event
{
    public class ProfileEvent : IEvent
    {
        public GetPlayerResponse Profile;

        public IEnumerable<PlayerStats> Stats { get;  set; }
    }
}