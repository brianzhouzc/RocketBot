using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POGOProtos.Inventory.Item;

namespace PoGo.NecroBot.Logic.Event.Inventory
{
    public class InventoryItemUpdateEvent : IEvent
    {
        public ItemData Item { get; internal set; }
    }
}
