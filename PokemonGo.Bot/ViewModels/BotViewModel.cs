using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PokemonGo.Bot.BotActions;
using PokemonGo.Bot.Utils;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Bot;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PokemonGo.Bot.ViewModels
{
    public class BotViewModel : ViewModelBase
    {
        public PlayerViewModel Player { get; }

        BotAction currentAction;

        public BotAction CurrentAction
        {
            get
            {
                return currentAction;
            }
            private set
            {
                if (CurrentAction != value)
                {
                    currentAction = value;
                    RaisePropertyChanged();
                }
            }
        }


        public ObservableCollection<BotAction> UpcomingActions { get; } = new ObservableCollection<BotAction>();
        readonly ActionFactory actionFactory;
        public RelayCommand<BotActionType> AddAction { get; }

        public MapViewModel Map { get; }

        public BotViewModel(PlayerViewModel player, MapViewModel map)
        {
            Map = map;
            Start = new RelayCommand(AdvanceToNextAction, HasNextAction);
            Stop = new RelayCommand(StopCurrentAction, IsExecutingAction);
            Player = player;
            actionFactory = new ActionFactory(this);
            AddAction = new RelayCommand<BotActionType>(param =>
            {
                UpcomingActions.Add(actionFactory.Get(param));
                Start.RaiseCanExecuteChanged();
                Stop.RaiseCanExecuteChanged();
            });
        }

        public RelayCommand Stop { get; }

        public RelayCommand Start { get; }

        void AdvanceToNextAction()
        {
            CurrentAction = DequeueAction();
            StartCurrentAction();
        }

        BotAction DequeueAction()
        {
            if(HasNextAction())
            {
                var firstAction = UpcomingActions[0];
                UpcomingActions.RemoveAt(0);
                return firstAction;
            }
            return new StoppedAction(this);
        }

        bool HasNextAction() => UpcomingActions.Count > 0;

        bool IsExecutingAction() => CurrentAction?.State == ActionState.Running;

        void StartCurrentAction() => CurrentAction?.StartAsync();

        void StopCurrentAction() => CurrentAction?.StopAsync();
    }
}