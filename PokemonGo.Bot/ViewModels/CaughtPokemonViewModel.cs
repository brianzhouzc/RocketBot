using AllEnum;
using PokemonGo.RocketAPI.GeneratedCode;
using System;

namespace PokemonGo.Bot.ViewModels
{
    public class CaughtPokemonViewModel : PokemonViewModel
    {
        int combatPoints;

        public int CombatPoints
        {
            get { return combatPoints; }
            set { if (CombatPoints != value) { combatPoints = value; RaisePropertyChanged(); } }
        }

        bool isFavorite;

        public bool IsFavorite
        {
            get { return isFavorite; }
            set { if (IsFavorite != value) { isFavorite = value; RaisePropertyChanged(); } }
        }

        public double PerfectPercentage { get; }
        public int IndividualAttack { get; }
        public int IndividualDefense { get; }
        public int IndividualStamina { get; }

        public string Move1 { get; }
        public string Move2 { get; }

        string nickname;

        public string Nickname
        {
            get { return nickname; }
            set { if (Nickname != value) { nickname = value; RaisePropertyChanged(); } }
        }

        public float HeightInMeters { get; }
        public float WeightInKilograms { get; }

        int stamina;

        public int Stamina
        {
            get { return stamina; }
            set { if (Stamina != value) { stamina = value; RaisePropertyChanged(); } }
        }

        int staminaMax;

        public int StaminaMax
        {
            get { return staminaMax; }
            set { if (StaminaMax != value) { staminaMax = value; RaisePropertyChanged(); } }
        }

        public CaughtPokemonViewModel(PokemonData pokemon) : base(pokemon.PokemonId, pokemon.Id)
        {
            CombatPoints = pokemon.Cp;
            IsFavorite = pokemon.Favorite != 0;
            PerfectPercentage = pokemon.GetIV();
            IndividualAttack = pokemon.IndividualAttack;
            IndividualDefense = pokemon.IndividualDefense;
            IndividualStamina = pokemon.IndividualStamina;
            Move1 = Enum.GetName(typeof(PokemonMove), pokemon.Move1);
            Move2 = Enum.GetName(typeof(PokemonMove), pokemon.Move2);
            Nickname = pokemon.Nickname;
            HeightInMeters = pokemon.HeightM;
            WeightInKilograms = pokemon.WeightKg;
            Stamina = pokemon.Stamina;
            StaminaMax = pokemon.StaminaMax;
        }
    }
}