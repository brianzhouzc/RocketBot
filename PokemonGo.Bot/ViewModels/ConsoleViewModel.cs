using GalaSoft.MvvmLight;
using PokemonGo.Bot.Messages;
using System.Collections.ObjectModel;

namespace PokemonGo.Bot.ViewModels
{
    public class ConsoleViewModel : ViewModelBase
    {
        public ObservableCollection<Message> Lines { get; } = new ObservableCollection<Message>();

        public ConsoleViewModel()
        {
            MessengerInstance.Register<Message>(this, Lines.Add);
            if(IsInDesignMode || IsInDesignModeStatic)
            {
                Lines.Add(new Message("test"));
            }
        }
    }
}