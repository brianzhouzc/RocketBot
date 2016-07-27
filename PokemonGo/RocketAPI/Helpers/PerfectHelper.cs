#region
using System;
using PokemonGo.RocketAPI.GeneratedCode;
#endregion


namespace PokemonGo.RocketAPI.Helpers
{
    public static class PerfectHelper
    {
        public static float Perfect(PokemonData poke)
        {
            return ((float)(poke.IndividualAttack + poke.IndividualDefense + poke.IndividualStamina) / (3.0f * 15.0f)) * 100.0f;
        }
    }
}