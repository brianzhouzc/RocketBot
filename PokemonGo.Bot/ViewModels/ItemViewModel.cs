using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using POGOProtos.Inventory.Item;
using PokemonGo.Bot.MVVMLightUtils;
using System;
using System.Threading.Tasks;

namespace PokemonGo.Bot.ViewModels
{
    public class ItemViewModel : ViewModelBase, IUpdateable<ItemViewModel>
    {
        readonly SessionViewModel session;

        int count;

        public int Count
        {
            get { return count; }
            set { if (Count != value) { count = value; RaisePropertyChanged(); recycle?.RaiseCanExecuteChanged(); } }
        }

        public Enums.ItemType ItemType { get; }

        #region Recycle

        AsyncRelayCommand<int> recycle;

        public AsyncRelayCommand<int> Recycle
        {
            get
            {
                if (recycle == null)
                    recycle = new AsyncRelayCommand<int>(ExecuteRecycle, CanExecuteRecycle);

                return recycle;
            }
        }

        async Task ExecuteRecycle(int param)
        {
            await session.RecycleInventoryItem((ItemId)ItemType, param);
            Count -= param;
        }

        bool CanExecuteRecycle(int param) => param <= Count;

        #endregion Recycle


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
            this.session = session;
            Count = count;
            ItemType = itemType;
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