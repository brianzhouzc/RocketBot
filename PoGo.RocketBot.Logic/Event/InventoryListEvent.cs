using POGOProtos.Inventory.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGo.RocketBot.Logic.Event
{
    public class InventoryListEvent : IEvent
    {
        public List<ItemData> Items;
    }
}
