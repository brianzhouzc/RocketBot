using POGOProtos.Inventory.Item;

namespace PoGo.RocketBot.Logic.Event
{
    public class UseBerryEvent : IEvent
    {
        public ItemId BerryType;
        public int Count;
    }
}