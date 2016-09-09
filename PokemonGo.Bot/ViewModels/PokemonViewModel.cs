using GalaSoft.MvvmLight;
using POGOProtos.Enums;
using PokemonGo.Bot.Utils;
using System;

namespace PokemonGo.Bot.ViewModels
{
    public abstract class PokemonViewModel : ViewModelBase
    {
        protected readonly Settings settings;
        public ulong Id { get; }

        protected PokemonViewModel(PokemonId pokemonId, ulong id, Settings settings)
        {
            Id = id;
            PokemonId = (int)pokemonId;
            this.settings = settings;

            var pokemonSettings = settings.GetPokemonSettings(PokemonId);
            FamilyId = pokemonSettings.FamilyId;
            BaseAttack = pokemonSettings.BaseAttack;
            BaseDefense = pokemonSettings.BaseDefense;
            BaseStamina = pokemonSettings.BaseStamina;
            CandyToEvolve = pokemonSettings.CandyToEvolve;
        }

        int pokemonId;
        public int PokemonId
        {
            get { return pokemonId; }
            set { if (PokemonId != value) { pokemonId = value; RaisePropertyChanged(); } }
        }


        public int FamilyId { get; }
        public virtual string Name => Enum.GetName(typeof(PokemonId), PokemonId);

        public int BaseAttack { get; }
        public int BaseDefense { get; }
        public int BaseStamina { get; }
        public int CandyToEvolve { get; }

        public override bool Equals(object obj) => Equals(obj as PokemonViewModel);
        public bool Equals(PokemonViewModel other) => Id == other?.Id;

        public override int GetHashCode() => Id.GetHashCode();
    }
}