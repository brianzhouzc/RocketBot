using GalaSoft.MvvmLight;
using PokemonGo.Bot.Messages;
using PokemonGo.Bot.ViewModels;
using System;
using System.Threading.Tasks;

namespace PokemonGo.Bot.BotActions
{
    public abstract class BotAction : ViewModelBase
    {
        protected readonly BotViewModel bot;
        ActionState state;
        public ActionState State
        {
            get { return state; }
            private set { if (State != value) { state = value; RaisePropertyChanged(); } }
        }


        public string DisplayName { get; }

        protected BotAction(BotViewModel bot, string displayName)
        {
            this.bot = bot;
            DisplayName = displayName;
            State = ActionState.Stopped;
        }

        public async Task StartAsync()
        {
            State = ActionState.Running;
            while (true)
            {
                try
                {
                    await OnStartAsync();
                }
                catch (Exception e)
                {
                    MessengerInstance.Send(new Message("Error, restarting current Bot Action " + e));
                }
            }
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