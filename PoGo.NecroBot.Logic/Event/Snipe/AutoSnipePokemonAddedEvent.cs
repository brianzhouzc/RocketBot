namespace PoGo.NecroBot.Logic.Event.Snipe
{
    public class AutoSnipePokemonAddedEvent : IEvent
    {
        public AutoSnipePokemonAddedEvent(EncounteredEvent data)
        {
            this.EncounteredEvent = data;
        }

        public EncounteredEvent EncounteredEvent { get; set; }
    }
}