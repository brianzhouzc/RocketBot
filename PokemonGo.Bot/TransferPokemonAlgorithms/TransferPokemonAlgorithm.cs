using PokemonGo.RocketAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    public enum TransferPokemonAlgorithm
    {
        CPDuplicate,
        All,
        Duplicate,
        IVDuplicate,
        CP,
        IV,
        None,
        IVDuplicateUnderCPThreshold
    }
}
