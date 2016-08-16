#region using directives

using System;
using System.Collections.Generic;
using POGOProtos.Data;

#endregion

namespace PokemonGo.RocketBot.Logic.Event
{
    public class PokemonListEvent : IEvent
    {
        public List<Tuple<PokemonData, double, int>> PokemonList;
    }
}