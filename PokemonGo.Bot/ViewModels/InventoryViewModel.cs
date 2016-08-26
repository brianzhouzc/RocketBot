using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using POGOProtos.Inventory;
using POGOProtos.Inventory.Item;
using PokemonGo.Bot.TransferPokemonAlgorithms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        public InventoryViewModel(SessionViewModel session, TransferPokemonAlgorithmFactory transferPokemonAlgorithmFactory)
        {
            this.transferPokemonAlgorithmFactory = transferPokemonAlgorithmFactory;
            TransferPokemonAlgorithm = transferPokemonAlgorithmFactory.GetDefaultFromSettings();
            this.session = session;

            Pokemon = new ObservableCollection<CaughtPokemonViewModel>();
            Eggs = new ObservableCollection<EggViewModel>();
            EggIncubators = new ObservableCollection<EggIncubatorViewModel>();
            Items = new ObservableCollection<ItemViewModel>();
        }

        internal void UpdateWith(IEnumerable<InventoryItem> inventory)
        {
            if (inventory != null)
            {
                UpdatePlayer(inventory);
                UpdatePokemon(inventory);
                UpdateEggIncubators(inventory);
                UpdateEggs(inventory);
                UpdateItems(inventory);
                UpdateCandy(inventory);
            }
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
            Pokemon.UpdateWith(inventory.Select(i => i.InventoryItemData?.PokemonData).Where(p => p?.PokemonId > 0).Select(p => new CaughtPokemonViewModel(p, session, this)));
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