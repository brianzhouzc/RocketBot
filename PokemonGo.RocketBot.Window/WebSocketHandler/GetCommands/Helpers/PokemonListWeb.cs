using POGOProtos.Data;

namespace PokemonGo.RocketBot.Window.WebSocketHandler.GetCommands.Helpers
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
            get
            {
                return Logic.PoGoUtils.PokemonInfo.CalculatePokemonPerfection(Base);
            }
        }

    }
}



