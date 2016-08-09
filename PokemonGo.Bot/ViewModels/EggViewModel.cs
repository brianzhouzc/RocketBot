using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POGOProtos.Data;

namespace PokemonGo.Bot.ViewModels
{
    public class EggViewModel : PokemonDataViewModel
    {
        public string IncubatorId { get; }
        public bool IsInUnlimitedIncubator => !string.IsNullOrEmpty(IncubatorId) && IncubatorId.Contains('-');
        public bool IsInNormalIncubator => !string.IsNullOrEmpty(IncubatorId) && !IncubatorId.Contains('-');

        double kmWalked;
        public double KmWalked
        {
            get { return kmWalked; }
            set { if (KmWalked != value) { kmWalked = value; RaisePropertyChanged(); } }
        }

        public double KmTarget { get; }

        public EggViewModel(PokemonData pokemon) : base(pokemon)
        {
            if (!pokemon.IsEgg)
                throw new ArgumentOutOfRangeException(nameof(pokemon.PokemonId), $"{pokemon} is not an egg.");

            IncubatorId = pokemon.EggIncubatorId;
            KmWalked = pokemon.EggKmWalkedStart;
            KmTarget = pokemon.EggKmWalkedTarget;
        }
    }
}
