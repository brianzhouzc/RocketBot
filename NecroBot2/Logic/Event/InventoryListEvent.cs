using System.Collections.Generic;
using POGOProtos.Inventory.Item;

namespace NecroBot2.Logic.Event
{
    public class InventoryListEvent : IEvent
    {
        public List<ItemData> Items;
    }
}