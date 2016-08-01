using PokemonGo.Bot.ViewModels;
using System.Threading.Tasks;

namespace PokemonGo.Bot.BotActions
{
    public abstract class BotAction
    {
        protected readonly BotViewModel bot;
        public ActionState State { get; private set; }

        protected BotAction(BotViewModel bot)
        {
            this.bot = bot;
            State = ActionState.Stopped;
        }

        public async Task StartAsync()
        {
            await OnStartAsync();
            State = ActionState.Running;
        }
        protected virtual Task OnStartAsync() => Task.CompletedTask;

        public async Task StopAsync()
        {
            await OnStopAsync();
            State = ActionState.Stopped;
        }
        protected virtual Task OnStopAsync() => Task.CompletedTask;
    }
}