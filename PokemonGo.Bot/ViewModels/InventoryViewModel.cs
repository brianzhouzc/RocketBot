using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using POGOProtos.Data.Player;
using POGOProtos.Inventory;
using POGOProtos.Inventory.Item;
using PokemonGo.Bot.TransferPokemonAlgorithms;
using PokemonGo.RocketAPI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonGo.Bot.ViewModels
{
    public class InventoryViewModel : ViewModelBase
    {
        public AsyncRelayCommand Load { get; }

        public AsyncRelayCommand<ulong> TransferSinglePokemon { get; }

        private Task ExecuteTransferSinglePokemonAsync(ulong pokemonId) => client.Inventory.TransferPokemon(pokemonId);

        public AsyncRelayCommand TransferPokemonWithAlgorithm { get; }

        private Task ExecuteTransferPokemonWithAlgorithmAsync(TransferPokemonAlgorithm transferAlgorithm)
        {
            var algorithm = transferPokemonAlgorithmFactory.Get(transferAlgorithm);
            var pokemonToTransfer = algorithm.Apply(Pokemon);
            return Task.WhenAll(pokemonToTransfer.Select(p => client.Inventory.TransferPokemon(p.Id)));
        }

        public AsyncRelayCommand<ItemType> Recycle { get; }

        private TransferPokemonAlgorithm transferPokemonAlgorithm;

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

        private InventoryDelta inventory;

        private InventoryDelta Inventory
        {
            get
            {
                return inventory;
            }
            set
            {
                if (Inventory != value)
                {
                    inventory = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(Pokemon));
                    RaisePropertyChanged(nameof(Items));
                    PlayerStats = value.InventoryItems.Where(i => i.InventoryItemData.PlayerStats != null).Select(i => i.InventoryItemData.PlayerStats).FirstOrDefault();
                }
            }
        }

        public IEnumerable<PokemonDataViewModel> Pokemon => Inventory?.InventoryItems.Select(i => i.InventoryItemData?.PokemonData).Where(p => p?.PokemonId > 0).Select(p => new PokemonDataViewModel(p));
        public IEnumerable<ItemViewModel> Items => Inventory?.InventoryItems.Select(i => i.InventoryItemData?.Item).Where(i => i != null).Select(i => new ItemViewModel(i));

        private PlayerStats playerStats;

        public PlayerStats PlayerStats
        {
            get
            {
                return playerStats;
            }
            set
            {
                if (PlayerStats != value)
                {
                    playerStats = value;
                    RaisePropertyChanged();
                }
            }
        }

        private readonly Client client;
        private readonly TransferPokemonAlgorithmFactory transferPokemonAlgorithmFactory;

        public InventoryViewModel(Client client, TransferPokemonAlgorithmFactory transferPokemonAlgorithmFactory)
        {
            this.transferPokemonAlgorithmFactory = transferPokemonAlgorithmFactory;
            TransferPokemonAlgorithm = transferPokemonAlgorithmFactory.GetDefaultFromSettings();
            this.client = client;

            Load = new AsyncRelayCommand(async () => Inventory = (await client.Inventory.GetInventory()).InventoryDelta);
            TransferPokemonWithAlgorithm = new AsyncRelayCommand(async () => await ExecuteTransferPokemonWithAlgorithmAsync(TransferPokemonAlgorithm));
            TransferSinglePokemon = new AsyncRelayCommand<ulong>(async param => await ExecuteTransferSinglePokemonAsync(param));
            Recycle = new AsyncRelayCommand<ItemType>(async itemType => await client.Inventory.RecycleItem((ItemId)itemType, 1));
        }
    }
}