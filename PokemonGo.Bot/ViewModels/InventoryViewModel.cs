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
        private Task ExecuteTransferSinglePokemon(ulong pokemonId)
        {
            return client.TransferPokemon(pokemonId);
        }

        public AsyncRelayCommand TransferPokemonWithAlgorithm { get; }
        private Task ExecuteTransferPokemonWithAlgorithm(TransferPokemonAlgorithm transferAlgorithm)
        {
            var algorithm = TransferPokemonAlgorithms.TransferPokemonAlgorithms.Get(transferAlgorithm);
            var pokemonToTransfer = algorithm.Apply(Pokemon);
            return Task.WhenAll(pokemonToTransfer.Select(p => client.TransferPokemon(p.Id)));
        }

        public AsyncRelayCommand Recycle { get; }

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
                    PlayerStats = value.InventoryItems.Where(i => i.InventoryItemData.PlayerStats != null).Select(i => i.InventoryItemData.PlayerStats).FirstOrDefault();
                }
            }
        }
        public IEnumerable<PokemonData> Pokemon => Inventory.InventoryItems.Select(i => i.InventoryItemData?.Pokemon).Where(p => p?.PokemonId > 0);

        private PlayerStats playerStats;
        private PlayerStats PlayerStats
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

        public InventoryViewModel(Client client)
        {
            this.client = client;

            Load = new AsyncRelayCommand(async () => Inventory = (await client.GetInventory()).InventoryDelta);
            TransferPokemonWithAlgorithm = new AsyncRelayCommand(async () => await ExecuteTransferPokemonWithAlgorithm(TransferPokemonAlgorithm));
            TransferSinglePokemon = new AsyncRelayCommand<ulong>(async param => await ExecuteTransferSinglePokemon(param));
            Recycle = new AsyncRelayCommand(async () => await client.RecycleItems(client));
        }
    }
}
