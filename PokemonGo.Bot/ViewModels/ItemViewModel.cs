using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using POGOProtos.Inventory.Item;
using PokemonGo.Bot.MVVMLightUtils;
using System;

namespace PokemonGo.Bot.ViewModels
{
    public class ItemViewModel : ViewModelBase, IUpdateable<ItemViewModel>
    {
        int count;

        public int Count
        {
            get { return count; }
            set { if (Count != value) { count = value; RaisePropertyChanged(); Recycle?.RaiseCanExecuteChanged(); } }
        }

        public Enums.ItemType ItemType { get; }

        public AsyncRelayCommand<int> Recycle { get; }

        public ItemViewModel(ItemData item, SessionViewModel session)
            : this(item.Count, (Enums.ItemType)item.ItemId, session)
        {
        }

        public ItemViewModel(ItemAward item, SessionViewModel session)
            : this(item.ItemCount, (Enums.ItemType)item.ItemId, session)
        {
        }

        ItemViewModel(int count, Enums.ItemType itemType, SessionViewModel session)
        {
            Count = count;
            ItemType = itemType;

            Recycle = new AsyncRelayCommand<int>(async i =>
            {
                await session.RecycleInventoryItem((ItemId)ItemType, i);
                Count -= i;
            },
            i =>
            {
                System.Diagnostics.Debug.WriteLine($"i: {i} Count: {Count} {i <= Count}");
                return true;
            });
        }

        public override int GetHashCode() => ItemType.GetHashCode();

        public override bool Equals(object obj) => Equals(obj as ItemViewModel);

        public bool Equals(ItemViewModel other) => ItemType == other?.ItemType;

        public void UpdateWith(ItemViewModel other)
        {
            if (!Equals(other))
                throw new ArgumentException($"Expected an item of type {ItemType} but got {other?.ItemType}.", nameof(other));

            Count = other.Count;
        }
    }
}