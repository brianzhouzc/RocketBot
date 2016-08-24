using GalaSoft.MvvmLight;
using POGOProtos.Enums;
using System;

namespace PokemonGo.Bot.ViewModels
{
    public abstract class PokemonViewModel : ViewModelBase
    {
        public ulong Id { get; }

        protected PokemonViewModel(PokemonId pokemonId, ulong id)
        {
            Id = id;
            PokemonId = (int)pokemonId;

            var familyId = PokemonId;
            while (familyId > 0 && !Enum.IsDefined(typeof(PokemonFamilyId), familyId))
                familyId--;
            FamilyId = familyId;
        }

        int pokemonId;
        public int PokemonId
        {
            get { return pokemonId; }
            set { if (PokemonId != value) { pokemonId = value; RaisePropertyChanged(); } }
        }


        public int FamilyId { get; }
        public virtual string Name => Enum.GetName(typeof(PokemonId), PokemonId);

        public override bool Equals(object obj) => Equals(obj as PokemonViewModel);
        public bool Equals(PokemonViewModel other) => Id == other?.Id;

        public override int GetHashCode() => Id.GetHashCode();
    }
}