using PoGo.NecroBot.Logic.Event;
using POGOProtos.Inventory.Item;

namespace NecroBot2.Logic.Event
{
    public class UseBerryEvent : IEvent
    {
        public ItemId BerryType;
        public int Count;
    }
}