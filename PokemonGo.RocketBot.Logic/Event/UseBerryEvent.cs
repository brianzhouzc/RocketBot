using POGOProtos.Inventory.Item;

namespace PokemonGo.RocketBot.Logic.Event
{
    public class UseBerryEvent : IEvent
    {
        public ItemId BerryType;
        public int Count;
    }
}