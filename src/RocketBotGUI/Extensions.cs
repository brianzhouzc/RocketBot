using POGOProtos.Data;

namespace PokemonGo.RocketAPI.Window
{
    public static class Extensions
    {
        public static float GetIV(this PokemonData poke)
        {
            return (poke.IndividualAttack + poke.IndividualDefense + poke.IndividualStamina)/45.0f;
        }
    }
}