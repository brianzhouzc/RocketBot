#region using directives

using PoGo.NecroBot.Logic.PoGoUtils;
using POGOProtos.Data;
using PoGo.NecroBot.Logic.State;

#endregion

namespace RocketBot2.WebSocketHandler.GetCommands.Helpers
{
    public class PokemonListWeb
    {
        public PokemonData Base;
        private readonly ISession _session;

        public PokemonListWeb(ISession session, PokemonData data)
        {
            Base = data;
            _session = session;
        }

        public double IvPerfection => PokemonInfo.CalculatePokemonPerfection(Base);
        public double Level => PokemonInfo.GetLevel(Base);
        public int FamilyCandies => PokemonInfo.GetCandy(_session, Base);
    }
}