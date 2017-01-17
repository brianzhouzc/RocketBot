using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Event.Inventory
{
    public class FinishUpgradeEvent :IEvent
    {
        public ulong PokemonId { get; set; }
        public bool AllowUpgrade { get; set; }
    }
}
