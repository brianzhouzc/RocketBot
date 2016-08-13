#region using directives

using POGOProtos.Networking.Responses;

#endregion

namespace PokemonGo.RocketBot.Logic.Event
{
    public class ProfileEvent : IEvent
    {
        public GetPlayerResponse Profile;
    }
}