#region using directives

using POGOProtos.Inventory.Item;

#endregion

namespace PoGo.NecroBot.Logic.Forms_Gui.Event
{
    public class ItemRecycledEvent : IEvent
    {
        public int Count;
        public ItemId Id;
    }
}