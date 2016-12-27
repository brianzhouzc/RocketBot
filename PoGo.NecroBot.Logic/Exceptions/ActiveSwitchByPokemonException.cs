using PoGo.NecroBot.Logic.Model.Settings;
using POGOProtos.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Exceptions
{
    public class ActiveSwitchByPokemonException : Exception
    {
        public double LastLatitude { get; set; }
        public double LastLongitude { get; set; }
        public PokemonId LastEncounterPokemonId{ get; set; }

        public AuthConfig Bot { get; set; }
    }
}
