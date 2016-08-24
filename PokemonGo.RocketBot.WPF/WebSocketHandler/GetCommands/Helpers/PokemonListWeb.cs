using PokemonGo.RocketBot.Logic.PoGoUtils;
using POGOProtos.Data;

namespace PokemonGo.RocketBot.WPF.WebSocketHandler.GetCommands.Helpers
{
    public class PokemonListWeb
    {
        public PokemonData Base;

        public PokemonListWeb(PokemonData data)
        {
            Base = data;
        }

        public double IvPerfection
        {
            get { return PokemonInfo.CalculatePokemonPerfection(Base); }
        }
    }
}