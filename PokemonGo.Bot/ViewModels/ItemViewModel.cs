using GalaSoft.MvvmLight;
using PokemonGo.Bot.Enums;
using PokemonGo.RocketAPI.GeneratedCode;

namespace PokemonGo.Bot.ViewModels
{
    public class ItemViewModel : ViewModelBase
    {
        int count;

        public int Count
        {
            get { return count; }
            set { if (Count != value) { count = value; RaisePropertyChanged(); } }
        }

        public ItemType ItemType { get; }

        public ItemViewModel(Item item)
        {
            Count = item.Count;
            ItemType = (ItemType)item.Item_;
        }
    }
}