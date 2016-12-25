using POGOProtos.Enums;
using POGOProtos.Networking.Responses;

namespace PoGo.NecroBot.Logic.Event.Gym
{
    public class GymTeamJoinEvent  : IEvent
    {
        public TeamColor Team;

        public SetPlayerTeamResponse.Types.Status Status { get; internal set; }
    }
}
