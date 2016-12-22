using PoGo.NecroBot.Logic.Model.Settings;
using POGOProtos.Enums;
using System;

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
