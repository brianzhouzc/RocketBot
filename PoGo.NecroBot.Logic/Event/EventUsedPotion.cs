namespace PoGo.NecroBot.Logic.Event
{
    public class EventUsedPotion : IEvent
    {
        public string Type;
        public string PokemonId;
        public int PokemonCp;
        public int Remaining;
    }
}