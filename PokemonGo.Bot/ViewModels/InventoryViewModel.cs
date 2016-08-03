using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PokemonGo.Bot.TransferPokemonAlgorithms;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.GeneratedCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.Bot.ViewModels
{
    public class InventoryViewModel : ViewModelBase
    {
        public AsyncRelayCommand Load { get; }

        public AsyncRelayCommand<ulong> TransferSinglePokemon { get; }
        Task ExecuteTransferSinglePokemonAsync(ulong pokemonId) => client.TransferPokemon(pokemonId);

        public AsyncRelayCommand TransferPokemonWithAlgorithm { get; }
        Task ExecuteTransferPokemonWithAlgorithmAsync(TransferPokemonAlgorithm transferAlgorithm)
        {
            var algorithm = transferPokemonAlgorithmFactory.Get(transferAlgorithm);
            var pokemonToTransfer = algorithm.Apply(Pokemon);
            return Task.WhenAll(pokemonToTransfer.Select(p => client.TransferPokemon(p.Id)));
        }

        public AsyncRelayCommand Recycle { get; }

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



        InventoryDelta inventory;
        InventoryDelta Inventory
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
                    PlayerStats = value.InventoryItems.Where(i => i.InventoryItemData.PlayerStats != null).Select(i => i.InventoryItemData.PlayerStats).FirstOrDefault();
                }
            }
        }
        public IEnumerable<CatchedPokemonViewModel> Pokemon => Inventory?.InventoryItems.Select(i => i.InventoryItemData?.Pokemon).Where(p => p?.PokemonId > 0).Select(p => new CatchedPokemonViewModel(p));

        PlayerStats playerStats;
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

        readonly Client client;
        readonly TransferPokemonAlgorithmFactory transferPokemonAlgorithmFactory;

        public InventoryViewModel(Client client, TransferPokemonAlgorithmFactory transferPokemonAlgorithmFactory)
        {
            this.transferPokemonAlgorithmFactory = transferPokemonAlgorithmFactory;
            TransferPokemonAlgorithm = transferPokemonAlgorithmFactory.GetDefaultFromSettings();
            this.client = client;

            Load = new AsyncRelayCommand(async () => Inventory = (await client.GetInventory()).InventoryDelta);
            TransferPokemonWithAlgorithm = new AsyncRelayCommand(async () => await ExecuteTransferPokemonWithAlgorithmAsync(TransferPokemonAlgorithm));
            TransferSinglePokemon = new AsyncRelayCommand<ulong>(async param => await ExecuteTransferSinglePokemonAsync(param));
            Recycle = new AsyncRelayCommand(async () => await client.RecycleItems(client));
        }
    }
}
