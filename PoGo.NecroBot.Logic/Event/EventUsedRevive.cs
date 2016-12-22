namespace PoGo.NecroBot.Logic.Event
{
    public class EventUsedRevive : IEvent
    {
        public string Type;
        public string PokemonId;
        public int PokemonCp;
        public int Remaining;
    }
}