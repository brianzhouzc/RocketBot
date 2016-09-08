using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using POGOProtos.Inventory;
using PokemonGo.Bot.TransferPokemonAlgorithms;
using PokemonGo.Bot.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PokemonGo.Bot.ViewModels
{
    public class InventoryViewModel : ViewModelBase
    {
        #region TransferPokemonWithAlgorithm

        AsyncRelayCommand transferPokemonWithAlgorithm;

        public AsyncRelayCommand TransferPokemonWithAlgorithm
        {
            get
            {
                if (transferPokemonWithAlgorithm == null)
                    transferPokemonWithAlgorithm = new AsyncRelayCommand(ExecuteTransferPokemonWithAlgorithm, CanExecuteTransferPokemonWithAlgorithm);

                return transferPokemonWithAlgorithm;
            }
        }

        Task ExecuteTransferPokemonWithAlgorithm()
        {
            var algorithm = transferPokemonAlgorithmFactory.Get(TransferPokemonAlgorithm);
            var pokemonToTransfer = algorithm.Apply(Pokemon);
            return Task.WhenAll(pokemonToTransfer.Select(p => p.Transfer.ExecuteAsync()));
        }

        bool CanExecuteTransferPokemonWithAlgorithm() => true;

        #endregion TransferPokemonWithAlgorithm

        TransferPokemonAlgorithm transferPokemonAlgorithm;

        public TransferPokemonAlgorithm TransferPokemonAlgorithm
        {
            get
            {
                return transferPokemonAlgorithm;
            }
            set
            {
                if (TransferPokemonAlgorithm != value)
                {
                    transferPokemonAlgorithm = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<CaughtPokemonViewModel> Pokemon { get; }
        public ObservableCollection<ItemViewModel> Items { get; }
        public ObservableCollection<EggViewModel> Eggs { get; }
        public ObservableCollection<EggIncubatorViewModel> EggIncubators { get; }

        readonly TransferPokemonAlgorithmFactory transferPokemonAlgorithmFactory;

        PlayerViewModel player;

        public PlayerViewModel Player
        {
            get { return player; }
            set { if (Player != value) { player = value; RaisePropertyChanged(); } }
        }

        readonly Settings settings;
        int maxItems;

        public int MaxItems
        {
            get { return maxItems; }
            set { if (MaxItems != value) { maxItems = value; RaisePropertyChanged(); } }
        }
        int numItems;
        public int NumItems
        {
            get { return numItems; }
            set { if (NumItems != value) { numItems = value; RaisePropertyChanged(); } }
        }


        int maxPokemon;

        public int MaxPokemon
        {
            get { return maxPokemon; }
            set { if (MaxPokemon != value) { maxPokemon = value; RaisePropertyChanged(); } }
        }
        int numPokemon;
        public int NumPokemon
        {
            get { return numPokemon; }
            set { if (NumPokemon != value) { numPokemon = value; RaisePropertyChanged(); } }
        }


        int maxEggs;

        public int MaxEggs
        {
            get { return maxEggs; }
            set { if (MaxEggs != value) { maxEggs = value; RaisePropertyChanged(); } }
        }
        int numEggs;
        public int NumEggs
        {
            get { return numEggs; }
            set { if (NumEggs != value) { numEggs = value; RaisePropertyChanged(); } }
        }



        public InventoryViewModel(SessionViewModel session, TransferPokemonAlgorithmFactory transferPokemonAlgorithmFactory, Settings settings)
        {
            Pokemon = new ObservableCollection<CaughtPokemonViewModel>();
            Eggs = new ObservableCollection<EggViewModel>();
            EggIncubators = new ObservableCollection<EggIncubatorViewModel>();
            Items = new ObservableCollection<ItemViewModel>();

            this.settings = settings;
            this.transferPokemonAlgorithmFactory = transferPokemonAlgorithmFactory;
            TransferPokemonAlgorithm = transferPokemonAlgorithmFactory.GetDefaultFromSettings();
            this.session = session;
            UpdateCount();
            settings.PropertyChanged += Settings_PropertyChanged;
        }

        void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(settings.Inventory))
            {
                UpdateCount();
            }
        }

        void UpdateCount()
        {
            MaxItems = settings.Inventory.BaseBagItems;
            MaxPokemon = settings.Inventory.BasePokemon;
            MaxEggs = settings.Inventory.BaseEggs;

            NumItems = Items.Sum(i => i.Count);
            NumPokemon = Pokemon.Count;
            NumEggs = Eggs.Count;
        }

        internal async Task UpdateWith(IEnumerable<InventoryItem> inventory)
        {
            if (inventory != null)
            {
                UpdatePlayer(inventory);
                UpdatePokemon(inventory);
                UpdateEggIncubators(inventory);
                UpdateEggs(inventory);
                UpdateItems(inventory);
                UpdateCandy(inventory);

                UpdateCount();

                await AutoManageInventory();
            }
        }

        const int minFreeItemSlots = 10;
        const int maxFreeItemSlots = 30;
        bool isCurrentlyAutomanaging;
        async Task AutoManageInventory()
        {
            if (isCurrentlyAutomanaging)
                return;

            try
            {
                isCurrentlyAutomanaging = true;

                if (!settings.AutoManageInventory)
                    return; // do not auto manage

                if (MaxItems == 0)
                    return; // do not throw away everything if the settings are not downloaded yet

                // count the different item types
                var numItems = 0;
                var numPotions = 0;
                var numRevive = 0;
                var numPokeballs = 0;
                var numBerries = 0;
                foreach (var item in Items.ToList())
                {
                    numItems += item.Count;
                    if (potionItemTypes.Contains(item.ItemType))
                        numPotions += item.Count;
                    else if (reviveItemTypes.Contains(item.ItemType))
                        numRevive += item.Count;
                    else if (pokeballItemTypes.Contains(item.ItemType))
                        numPokeballs += item.Count;
                    else if (berryItemTypes.Contains(item.ItemType))
                        numBerries += item.Count;
                }

                if (numItems < maxItems - minFreeItemSlots)
                    return; // we have enough free item slots

                var numItemsToRecycle = numItems - (maxItems - maxFreeItemSlots);

                // the recycling order was chosen to guarantee a maximum of pokeballs to catch em all :)

                // first recycle berries because the api to use berries is broken
                var recycledBerries = await TryRecycleAsync(berryItemTypes, numItemsToRecycle, settings.MinBerries, numBerries);
                numItemsToRecycle -= recycledBerries;
                if (numItemsToRecycle <= 0)
                    return;

                // then recycle potions
                var recycledPotions = await TryRecycleAsync(potionItemTypes, numItemsToRecycle, settings.MinPotions, numPotions);
                numItemsToRecycle -= recycledPotions;
                if (numItemsToRecycle <= 0)
                    return;

                // then recycle revive
                var recycledRevive = await TryRecycleAsync(reviveItemTypes, numItemsToRecycle, settings.MinRevive, numRevive);
                numItemsToRecycle -= recycledRevive;
                if (numItemsToRecycle <= 0)
                    return;

                // at last recycle pokeballs
                var recycledPokeballs = await TryRecycleAsync(pokeballItemTypes, numItemsToRecycle, settings.MinPokeballs, numPokeballs);
            }
            finally
            {
                isCurrentlyAutomanaging = false;
            }
        }

        static ISet<Enums.ItemType> potionItemTypes = new HashSet<Enums.ItemType> { Enums.ItemType.ItemPotion, Enums.ItemType.ItemSuperPotion, Enums.ItemType.ItemHyperPotion, Enums.ItemType.ItemMaxPotion };
        static ISet<Enums.ItemType> reviveItemTypes = new HashSet<Enums.ItemType> { Enums.ItemType.ItemRevive, Enums.ItemType.ItemMaxRevive };
        static ISet<Enums.ItemType> pokeballItemTypes = new HashSet<Enums.ItemType> { Enums.ItemType.ItemPokeBall, Enums.ItemType.ItemGreatBall, Enums.ItemType.ItemUltraBall, Enums.ItemType.ItemMasterBall };
        static ISet<Enums.ItemType> berryItemTypes = new HashSet<Enums.ItemType> { Enums.ItemType.ItemRazzBerry, Enums.ItemType.ItemBlukBerry, Enums.ItemType.ItemNanabBerry, Enums.ItemType.ItemPinapBerry, Enums.ItemType.ItemWeparBerry };

        ItemViewModel GetItem(Enums.ItemType itemType) => Items.FirstOrDefault(i => i.ItemType == itemType);

        async Task<int> TryRecycleAsync(IEnumerable<Enums.ItemType> itemTypes, int maxItemsToRecycle, int minItemsForType, int numItemsForType)
        {
            var sumRecycledItems = 0;
            foreach (var itemType in itemTypes)
            {
                var recycledItems = await TryRecycleAsync(GetItem(itemType), maxItemsToRecycle, minItemsForType, numItemsForType);
                maxItemsToRecycle -= recycledItems;
                numItemsForType -= recycledItems;
                sumRecycledItems += recycledItems;
            }

            return sumRecycledItems;
        }

        static async Task<int> TryRecycleAsync(ItemViewModel item, int maxItemsToRecycle, int minItemsForType, int numItemsForType)
        {
            if (item == null)
                return 0;

            var maxItemsToRecycleForType = Math.Min(maxItemsToRecycle, numItemsForType - minItemsForType);
            if (maxItemsToRecycleForType <= 0)
                return 0;

            var numItemsToRecycle = Math.Min(item.Count, maxItemsToRecycleForType);
            if (numItemsToRecycle <= 0)
                return 0;

            await item.Recycle.ExecuteAsync(numItemsToRecycle);

            return numItemsToRecycle;
        }

        void UpdateItems(IEnumerable<InventoryItem> inventory)
        {
            Items.UpdateWith(inventory.Select(i => i.InventoryItemData?.Item).Where(i => i != null).Select(i => new ItemViewModel(i, session)));
        }

        void UpdateEggs(IEnumerable<InventoryItem> inventory)
        {
            Eggs.UpdateWith(inventory.Select(i => i.InventoryItemData?.PokemonData).Where(p => (p?.IsEgg).GetValueOrDefault()).Select(p => new EggViewModel(p, EggIncubators, Player.KmWalked)));
        }

        void UpdateEggIncubators(IEnumerable<InventoryItem> inventory)
        {
            EggIncubators.UpdateWith(inventory.Select(i => i.InventoryItemData?.EggIncubators).Where(i => i != null).SelectMany(i => i?.EggIncubator).Where(i => i != null).Select(i => new EggIncubatorViewModel(i, session)));
        }

        void UpdatePokemon(IEnumerable<InventoryItem> inventory)
        {
            Pokemon.UpdateWith(inventory.Select(i => i.InventoryItemData?.PokemonData).Where(p => p?.PokemonId > 0).Select(p => new CaughtPokemonViewModel(p, session, this, settings)));
        }

        void UpdateCandy(IEnumerable<InventoryItem> inventory)
        {
            foreach (var candy in inventory.Select(i => i.InventoryItemData?.Candy).Where(c => c != null))
            {
                var familyId = (int)candy.FamilyId;
                SetCandyForFamily(candy.Candy_, familyId);
            }
        }

        readonly IDictionary<int, int> candyForFamily = new Dictionary<int, int>();
        readonly SessionViewModel session;

        public void SetCandyForFamily(int amount, int familyId)
        {
            candyForFamily[familyId] = amount;
            var pokemonForCandy = Pokemon.Where(p => p.FamilyId == familyId);
            foreach (var pokemonWithCorrectFamily in pokemonForCandy)
            {
                pokemonWithCorrectFamily.Candy = amount;
            }
        }

        public int GetCandyForFamily(int familyId)
        {
            var amount = 0;
            candyForFamily.TryGetValue(familyId, out amount);
            return amount;
        }

        public void AddCandyForFamily(int amount, int familyId)
        {
            SetCandyForFamily(GetCandyForFamily(familyId) + amount, familyId);
        }

        void UpdatePlayer(IEnumerable<InventoryItem> inventory)
        {
            var playerStats = inventory.Where(i => i.InventoryItemData?.PlayerStats != null).Select(i => i.InventoryItemData.PlayerStats).FirstOrDefault();
            if (playerStats != null)
            {
                Player.Xp = playerStats.Experience;
                Player.NextLevelXP = playerStats.NextLevelXp;
                Player.PrevLevelXp = playerStats.PrevLevelXp;
                Player.Level = playerStats.Level;
                Player.KmWalked = playerStats.KmWalked;
            }
        }
    }
}