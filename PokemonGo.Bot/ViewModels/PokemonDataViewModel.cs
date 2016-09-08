using POGOProtos.Data;
using POGOProtos.Enums;
using System;

namespace PokemonGo.Bot.ViewModels
{
    public class PokemonDataViewModel : PokemonViewModel
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

        public double PerfectPercentage { get { return (IndividualAttack + IndividualDefense + IndividualStamina) / 45.0; } }
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

        float cpMultiplier;
        public float CpMultiplier
        {
            get { return cpMultiplier; }
            set { if (CpMultiplier != value) { cpMultiplier = value; RaisePropertyChanged(); } }
        }



        public PokemonDataViewModel(PokemonData pokemon) : base(pokemon.PokemonId, pokemon.Id)
        {
            CombatPoints = pokemon.Cp;
            IsFavorite = pokemon.Favorite != 0;
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
            CpMultiplier = pokemon.CpMultiplier;

        }

        public void UpdateWith(PokemonDataViewModel other)
        {
            if (!Equals(other))
                throw new ArgumentException($"Expected a {Name} with Id {Id} but got a {other?.Name} with Id {other?.Id}", nameof(other));

            CombatPoints = other.CombatPoints;
            IsFavorite = other.IsFavorite;
            Nickname = other.Nickname;
            Stamina = other.Stamina;
        }
    }
}