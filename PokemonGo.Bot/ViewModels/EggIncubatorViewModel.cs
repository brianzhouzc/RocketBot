using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using POGOProtos.Inventory;
using POGOProtos.Networking.Responses;
using PokemonGo.Bot.Messages;
using PokemonGo.Bot.MVVMLightUtils;
using PokemonGo.RocketAPI;
using System;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PokemonGo.Bot.ViewModels
{
    public class EggIncubatorViewModel : ViewModelBase, IUpdateable<EggIncubatorViewModel>
    {
        readonly SessionViewModel session;
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

        #region PutEggIntoIncubator

        AsyncRelayCommand<EggViewModel> putEggIntoIncubator;

        public AsyncRelayCommand<EggViewModel> PutEggIntoIncubator
        {
            get
            {
                if (putEggIntoIncubator == null)
                    putEggIntoIncubator = new AsyncRelayCommand<EggViewModel>(ExecutePutEggIntoIncubator, CanExecutePutEggIntoIncubator);

                return putEggIntoIncubator;
            }
        }

        async Task ExecutePutEggIntoIncubator(EggViewModel param)
        {
            var response = await session.UseItemEggIncubator(Id, param.Id);
            if (response.Result == UseItemEggIncubatorResponse.Types.Result.Success)
            {
                param.IncubatorId = Id;
                PokemonId = param.Id;
            }
            else
            {
                MessengerInstance.Send(new Message(Colors.Red, $"Failed to put Egg into Incubator. Response was {Enum.GetName(typeof(UseItemEggIncubatorResponse.Types.Result), response.Result)}."));
            }
        }

        bool CanExecutePutEggIntoIncubator(EggViewModel param) => !IsInUse;

        #endregion PutEggIntoIncubator

        public EggIncubatorViewModel(EggIncubator incubator, SessionViewModel session)
        {
            this.session = session;
            Id = incubator.Id;
            IsUnlimited = incubator.ItemId == POGOProtos.Inventory.Item.ItemId.ItemIncubatorBasicUnlimited;
            PokemonId = incubator.PokemonId;
            StartKmWalked = incubator.StartKmWalked;
            TargetKmWalked = incubator.TargetKmWalked;
            UsesRemaining = incubator.UsesRemaining;
        }

        public override int GetHashCode() => Id.GetHashCode();
        public override bool Equals(object obj) => Equals(obj as EggIncubatorViewModel);
        public bool Equals(EggIncubatorViewModel other) => Id == other?.Id;

        public override string ToString()
            => $"{Id} - {(IsInUse ? "In use" : "Not in use")} - {(IsUnlimited ? "Unlimited" : $"{UsesRemaining} uses remaining.")}";

        public void UpdateWith(EggIncubatorViewModel other)
        {
            if (!Equals(other))
                throw new ArgumentException($"Expected an Incubator with Id {Id} but got {other?.Id}");

            PokemonId = other.PokemonId;
            StartKmWalked = other.StartKmWalked;
            TargetKmWalked = other.TargetKmWalked;
            UsesRemaining = other.UsesRemaining;
        }
    }
}