namespace PoGo.NecroBot.Logic.Event.UI
{
    public class StatusBarEvent : IEvent
    {
        public StatusBarEvent(string s)
        {
            this.Message = s;
        }

        public string Message { get; set; }
    }
}