using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using POGOProtos.Data.Player;
using POGOProtos.Inventory;
using POGOProtos.Inventory.Item;
using PokemonGo.Bot.TransferPokemonAlgorithms;
using PokemonGo.RocketAPI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonGo.Bot.ViewModels
{
    public class InventoryViewModel : ViewModelBase
    {

        public AsyncRelayCommand Load { get; }

        public AsyncRelayCommand TransferPokemonWithAlgorithm { get; }

        Task ExecuteTransferPokemonWithAlgorithmAsync(TransferPokemonAlgorithm transferAlgorithm)
        {
            var algorithm = transferPokemonAlgorithmFactory.Get(transferAlgorithm);
            var pokemonToTransfer = algorithm.Apply(Pokemon);
            return Task.WhenAll(pokemonToTransfer.Select(p => p.Transfer.ExecuteAsync()));
        }

        public AsyncRelayCommand<ItemType> Recycle { get; }

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

        private void UpdateCandy(InventoryDelta inventory)
        {
            foreach (var candy in inventory?.InventoryItems.Select(i => i.InventoryItemData?.Candy).Where(c => c != null))
            {
                var familyId = (int)candy.FamilyId;
                SetCandyForFamily(candy.Candy_, familyId);
            }
        }

        public void SetCandyForFamily(int amount, int familyId)
        {
            var pokemonForCandy = Pokemon.Where(p => p.FamilyId == familyId);
            foreach (var pokemonWithCorrectFamily in pokemonForCandy)
            {
                pokemonWithCorrectFamily.Candy = amount;
            }
        }

        void UpdatePlayer(InventoryDelta value)
        {
            var playerStats = value.InventoryItems.Where(i => i.InventoryItemData.PlayerStats != null).Select(i => i.InventoryItemData.PlayerStats).FirstOrDefault();
            Player.Xp = playerStats.Experience;
            Player.NextLevelXP = playerStats.NextLevelXp;
            Player.PrevLevelXp = playerStats.PrevLevelXp;
            Player.Level = playerStats.Level;
        }

        public ObservableCollection<CaughtPokemonViewModel> Pokemon { get; }
        public ObservableCollection<ItemViewModel> Items { get; }
        public ObservableCollection<EggViewModel> Eggs { get; }

        readonly Client client;
        readonly TransferPokemonAlgorithmFactory transferPokemonAlgorithmFactory;

        PlayerViewModel player;
        public PlayerViewModel Player
        {
            get { return player; }
            set { if (Player != value) { player = value; RaisePropertyChanged(); } }
        }

        public InventoryViewModel(Client client, TransferPokemonAlgorithmFactory transferPokemonAlgorithmFactory)
        {
            this.transferPokemonAlgorithmFactory = transferPokemonAlgorithmFactory;
            TransferPokemonAlgorithm = transferPokemonAlgorithmFactory.GetDefaultFromSettings();
            this.client = client;

            Pokemon = new ObservableCollection<CaughtPokemonViewModel>();
            Eggs = new ObservableCollection<EggViewModel>();
            Items = new ObservableCollection<ItemViewModel>();

            Load = new AsyncRelayCommand(async () =>
            {
                var itemTemplaceResponse = await client.Download.GetItemTemplates();
                if(itemTemplaceResponse.Success)
                {
                    var templates = itemTemplaceResponse.ItemTemplates;
                }
                var inventoryResponse = await client.Inventory.GetInventory();
                if(inventoryResponse.Success)
                {
                    var inventory = inventoryResponse.InventoryDelta;
                    Pokemon.UpdateWith(inventory?.InventoryItems.Select(i => i.InventoryItemData?.PokemonData).Where(p => p?.PokemonId > 0).Select(p => new CaughtPokemonViewModel(p, client, this)));
                    Eggs.UpdateWith(inventory?.InventoryItems.Select(i => i.InventoryItemData?.PokemonData).Where(p => (p?.IsEgg).GetValueOrDefault()).Select(p => new EggViewModel(p)));
                    Items.UpdateWith(inventory?.InventoryItems.Select(i => i.InventoryItemData?.Item).Where(i => i != null).Select(i => new ItemViewModel(i)));
                    UpdateCandy(inventory);
                    UpdatePlayer(inventory);
                }
            });
            TransferPokemonWithAlgorithm = new AsyncRelayCommand(async () => await ExecuteTransferPokemonWithAlgorithmAsync(TransferPokemonAlgorithm));
            Recycle = new AsyncRelayCommand<ItemType>(async itemType => await client.Inventory.RecycleItem((ItemId)itemType, 1));
        }
    }
}