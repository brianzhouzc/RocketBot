#region using directives

using NecroBot2.Logic.State;
using NecroBot2.Logic.Tasks;

#endregion

namespace NecroBot2.Logic.Service
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