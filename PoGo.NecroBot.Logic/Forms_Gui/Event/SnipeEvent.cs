namespace PoGo.NecroBot.Logic.Forms_Gui.Event
{
    public class SnipeEvent : IEvent
    {
        public string Message = "";

        public override string ToString()
        {
            return Message;
        }
    }
}