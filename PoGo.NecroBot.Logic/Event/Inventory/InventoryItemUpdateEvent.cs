using POGOProtos.Inventory.Item;

namespace PoGo.NecroBot.Logic.Event.Inventory
{
    public class InventoryItemUpdateEvent : IEvent
    {
        public ItemData Item { get; internal set; }
    }
}