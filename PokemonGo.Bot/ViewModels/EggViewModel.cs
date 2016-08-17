using POGOProtos.Data;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace PokemonGo.Bot.ViewModels
{
    public class EggViewModel : PokemonDataViewModel
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

        public double KmTarget { get; }

        public ObservableCollection<EggIncubatorViewModel> EggIncubators { get; }

        public EggViewModel(PokemonData pokemon, ObservableCollection<EggIncubatorViewModel> eggIncubators) : base(pokemon)
        {
            if (!pokemon.IsEgg)
                throw new ArgumentOutOfRangeException(nameof(pokemon.PokemonId), $"{pokemon} is not an egg.");

            EggIncubators = eggIncubators;
            IncubatorId = pokemon.EggIncubatorId;
            KmWalked = pokemon.EggKmWalkedStart;
            KmTarget = pokemon.EggKmWalkedTarget;
        }
    }
}