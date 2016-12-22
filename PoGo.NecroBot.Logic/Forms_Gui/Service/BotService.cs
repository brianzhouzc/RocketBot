#region using directives

using PoGo.NecroBot.Logic.Forms_Gui.State;
using PoGo.NecroBot.Logic.Forms_Gui.Tasks;

#endregion

namespace PoGo.NecroBot.Logic.Forms_Gui.Service
{
    public class BotService
    {
        public ILogin LoginTask;
        public ISession Session;

        public void Run()
        {
            LoginTask.DoLogin();
        }
    }
}