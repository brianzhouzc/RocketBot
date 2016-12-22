using POGOProtos.Inventory.Item;

namespace PoGo.NecroBot.Logic.Forms_Gui.Event
{
    public class UseBerryEvent : IEvent
    {
        public ItemId BerryType;
        public int Count;
    }
}