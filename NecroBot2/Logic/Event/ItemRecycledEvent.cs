#region using directives

using PoGo.NecroBot.Logic.Event;
using POGOProtos.Inventory.Item;

#endregion

namespace NecroBot2.Logic.Event
{
    public class ItemRecycledEvent : IEvent
    {
        public int Count;
        public ItemId Id;
    }
}