using System.Collections.Generic;
using POGOProtos.Inventory.Item;

namespace PokemonGo.RocketBot.Logic.Event
{
    public class InventoryListEvent : IEvent
    {
        public List<ItemData> Items;
    }
}