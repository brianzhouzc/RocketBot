#region using directives

using System;
using System.Collections.Generic;
using POGOProtos.Data;
using POGOProtos.Enums;
using PoGo.NecroBot.Logic.Event;

#endregion

namespace NecroBot2.Logic.Event
{
    public class DisplayHighestsPokemonEvent : IEvent
    {
        //PokemonData | CP | IV | Level | MOVE1 | MOVE2 | Candy
        public List<Tuple<PokemonData, int, double, double, PokemonMove, PokemonMove, int>> PokemonList;
        public string SortedBy;
    }
}