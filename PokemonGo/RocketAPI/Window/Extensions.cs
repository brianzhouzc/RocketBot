using POGOProtos.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.Window
{
    public static class Extensions
    {
        public static float GetIV(this PokemonData poke)
        {
            return (poke.IndividualAttack + poke.IndividualDefense + poke.IndividualStamina) / 45.0f;
        }
    }
}
