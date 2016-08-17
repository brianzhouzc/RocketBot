using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using POGOProtos.Inventory;
using POGOProtos.Networking.Responses;
using PokemonGo.Bot.Messages;
using PokemonGo.RocketAPI;
using System;
using System.Windows.Media;

namespace PokemonGo.Bot.ViewModels
{
    public class EggIncubatorViewModel : ViewModelBase
    {
        public string Id { get; }
        public bool IsUnlimited { get; }
        ulong pokemonId;

        public ulong PokemonId
        {
            get { return pokemonId; }
            set
            {
                if (PokemonId != value)
                {
                    pokemonId = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(IsInUse));
                }
            }
        }
        public bool IsInUse => PokemonId != 0;

        double startKmWalked;

        public double StartKmWalked
        {
            get { return startKmWalked; }
            set { if (StartKmWalked != value) { startKmWalked = value; RaisePropertyChanged(); } }
        }

        double targetKmWalked;

        public double TargetKmWalked
        {
            get { return targetKmWalked; }
            set { if (TargetKmWalked != value) { targetKmWalked = value; RaisePropertyChanged(); } }
        }

        int usesRemaining;

        public int UsesRemaining
        {
            get { return usesRemaining; }
            set { if (UsesRemaining != value) { usesRemaining = value; RaisePropertyChanged(); } }
        }

        public AsyncRelayCommand<EggViewModel> PutEggIntoIncubator { get; }

        public EggIncubatorViewModel(EggIncubator incubator, Client client)
        {
            Id = incubator.Id;
            IsUnlimited = incubator.ItemId == POGOProtos.Inventory.Item.ItemId.ItemIncubatorBasicUnlimited;
            PokemonId = incubator.PokemonId;
            StartKmWalked = incubator.StartKmWalked;
            TargetKmWalked = incubator.TargetKmWalked;
            UsesRemaining = incubator.UsesRemaining;

            PutEggIntoIncubator = new AsyncRelayCommand<EggViewModel>(async egg =>
            {
                var response = await client.Inventory.UseItemEggIncubator(Id, egg.Id);
                if (response.Result == UseItemEggIncubatorResponse.Types.Result.Success)
                {
                    egg.IncubatorId = Id;
                    PokemonId = egg.Id;
                }
                else
                {
                    MessengerInstance.Send(new Message(Colors.Red, $"Failed to put Egg into Incubator. Response was {Enum.GetName(typeof(UseItemEggIncubatorResponse.Types.Result), response.Result)}."));
                }
            },
            _ => !IsInUse);
        }
    }
}