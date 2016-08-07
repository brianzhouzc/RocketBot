#region using directives

using PoGo.RocketBot.Logic.State;
using PoGo.RocketBot.Logic.Tasks;

#endregion

namespace PoGo.RocketBot.Logic.Service
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