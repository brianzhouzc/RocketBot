using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PokemonGo.Bot.BotActions;
using PokemonGo.RocketAPI;
using System.Collections.Generic;

namespace PokemonGo.Bot.ViewModels
{
    public class BotViewModel : ViewModelBase
    {
        public PlayerViewModel Player { get; }
        readonly Client client;
        BotAction currentAction;
        readonly Queue<BotAction> upcomingActions = new Queue<BotAction>();

        public MapViewModel Map { get; }

        public BotViewModel(Client client, PlayerViewModel player, MapViewModel map)
        {
            Map = map;
            this.client = client;
            Start = new RelayCommand(AdvanceToNextAction, HasNextAction);
            Stop = new RelayCommand(StopCurrentAction, IsExecutingAction);
            Player = player;
        }

        public RelayCommand Stop { get; }

        private RelayCommand Start { get; }

        private void AddAction(BotAction action) => upcomingActions.Enqueue(action);

        private void AdvanceToNextAction()
        {
            currentAction = HasNextAction() ? upcomingActions.Dequeue() : new StoppedAction(this);
        }

        private bool HasNextAction() => upcomingActions.Count > 0;

        private bool IsExecutingAction() => currentAction?.State == ActionState.Running;

        private void StartCurrentAction() => currentAction?.StartAsync();

        private void StopCurrentAction() => currentAction?.StopAsync();
    }
}