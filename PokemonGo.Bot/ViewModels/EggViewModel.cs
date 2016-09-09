using GalaSoft.MvvmLight.Command;
using POGOProtos.Data;
using PokemonGo.Bot.MVVMLightUtils;
using PokemonGo.Bot.Utils;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace PokemonGo.Bot.ViewModels
{
    public class EggViewModel : PokemonDataViewModel, IUpdateable<EggViewModel>, IComparable<EggViewModel>
    {
        string incubatorId;

        public string IncubatorId
        {
            get { return incubatorId; }
            set
            {
                if (IncubatorId != value)
                {
                    incubatorId = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(IsInIncubator));
                    RaisePropertyChanged(nameof(IsInUnlimitedIncubator));
                    RaisePropertyChanged(nameof(IsInNormalIncubator));
                }
            }
        }

        public bool IsInIncubator => !string.IsNullOrEmpty(IncubatorId);
        public bool IsInUnlimitedIncubator => IsInIncubator && IncubatorId.Contains('-');
        public bool IsInNormalIncubator => IsInIncubator && !IncubatorId.Contains('-');

        double kmWalked;

        public double KmWalked
        {
            get { return kmWalked; }
            set { if (KmWalked != value) { kmWalked = value; RaisePropertyChanged(); } }
        }

        public double KmWalkedStart { get; }
        public double KmTarget { get; }

        public ObservableCollection<EggIncubatorViewModel> EggIncubators { get; }

        public EggViewModel(PokemonData pokemon, ObservableCollection<EggIncubatorViewModel> eggIncubators, float playerKmWalked, Settings settings) : base(pokemon, settings)
        {
            if (!pokemon.IsEgg)
                throw new ArgumentOutOfRangeException(nameof(pokemon.PokemonId), $"{pokemon} is not an egg.");


            EggIncubators = eggIncubators;
            IncubatorId = pokemon.EggIncubatorId;
            KmWalkedStart = pokemon.EggKmWalkedStart;
            KmTarget = pokemon.EggKmWalkedTarget;

            if (IsInIncubator)
            {
                var incubator = EggIncubators.SingleOrDefault(i => i.Id == IncubatorId);
                if (incubator != null)
                {
                    KmWalked = playerKmWalked - incubator.StartKmWalked;
                }
            }
        }

        public void UpdateWith(EggViewModel other)
        {
            IncubatorId = other.IncubatorId;
            KmWalked = other.KmWalked;
            EggIncubators.UpdateWith(other.EggIncubators);

            base.UpdateWith(other);
        }

        public int CompareTo(EggViewModel other)
        {
            var result = Nullable.Compare(IsInUnlimitedIncubator, other?.IsInUnlimitedIncubator);
            if (result == 0)
                result = Nullable.Compare(IsInNormalIncubator, other?.IsInNormalIncubator);
            if (result == 0)
                result = Nullable.Compare(KmTarget, other?.KmTarget);
            if (result == 0)
                result = Nullable.Compare(KmWalked, other?.KmWalked);
            return result;
        }
    }
}