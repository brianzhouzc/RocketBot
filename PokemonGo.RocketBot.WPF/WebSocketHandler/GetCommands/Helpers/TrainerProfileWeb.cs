using POGOProtos.Data;
using POGOProtos.Data.Player;

namespace PokemonGo.RocketBot.WPF.WebSocketHandler.GetCommands.Helpers
{
    internal class TrainerProfileWeb
    {
        public PlayerData Profile;
        public PlayerStats Stats;

        public TrainerProfileWeb(PlayerData profile, PlayerStats stats)
        {
            Profile = profile;
            Stats = stats;
        }
    }
}