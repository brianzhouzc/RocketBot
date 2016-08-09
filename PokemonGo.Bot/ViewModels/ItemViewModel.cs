using GalaSoft.MvvmLight;
using POGOProtos.Inventory.Item;
using PokemonGo.Bot.Enums;

namespace PokemonGo.Bot.ViewModels
{
    public class ItemViewModel : ViewModelBase
    {
        private int count;

        public int Count
        {
            get { return count; }
            set { if (Count != value) { count = value; RaisePropertyChanged(); } }
        }

        public Enums.ItemType ItemType { get; }

        public ItemViewModel(ItemData item)
        {
            Count = item.Count;
            ItemType = (Enums.ItemType)item.ItemId;
        }

        public ItemViewModel(ItemAward item)
        {
            Count = item.ItemCount;
            ItemType = (Enums.ItemType)item.ItemId;
        }
    }
}