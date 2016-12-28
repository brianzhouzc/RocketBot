using System.Collections.Generic;
using POGOProtos.Inventory.Item;
using PoGo.NecroBot.Logic.Event;

namespace NecroBot2.Logic.Event
{
    public class InventoryListEvent : IEvent
    {
        public List<ItemData> Items;
    }
}