#region using directives

using PokemonGo.RocketBot.Logic.State;
using PokemonGo.RocketBot.Logic.Tasks;

#endregion

namespace PokemonGo.RocketBot.Logic.Service
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