using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POGOProtos.Data;
using PokemonGo.RocketAPI;
using GalaSoft.MvvmLight.Command;
using PokemonGo.Bot.Messages;
using POGOProtos.Networking.Responses;

namespace PokemonGo.Bot.ViewModels
{
    public class CaughtPokemonViewModel : PokemonDataViewModel
    {
        readonly Client client;

        public AsyncRelayCommand Transfer { get; }

        int candy;
        public int Candy
        {
            get { return candy; }
            set { if (Candy != value) { candy = value; RaisePropertyChanged(); } }
        }

        public CaughtPokemonViewModel(PokemonData pokemon, Client client, InventoryViewModel inventory) : base(pokemon)
        {
            if (Id == 0)
                throw new ArgumentException("This is not a caught pokemon.", nameof(pokemon));

            this.client = client;
            Transfer = new AsyncRelayCommand(async () =>
            {
                var response = await client.Inventory.TransferPokemon(Id);
                if(response.Result == ReleasePokemonResponse.Types.Result.Success)
                {
                    inventory.Pokemon.Remove(this);
                }
                else
                {
                    MessengerInstance.Send(new Message($"Error transferring {Name}: {Enum.GetName(typeof(ReleasePokemonResponse.Types.Result), response.Result)}"));
                }
            });
        }
    }
}
