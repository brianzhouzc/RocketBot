using POGOProtos.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POGOProtos.Networking.Responses;

namespace PoGo.NecroBot.Logic.Event.Gym
{
    public class GymTeamJoinEvent  : IEvent
    {
        public TeamColor Team;

        public SetPlayerTeamResponse.Types.Status Status { get; internal set; }
    }
}
