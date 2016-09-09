using POGOProtos.Enums;

namespace PokemonGo.Bot.Utils
{
    public class PokemonSettings
    {
        public PokemonSettings(POGOProtos.Settings.Master.PokemonSettings pokemonSettings)
        {
            CandyToEvolve = pokemonSettings.CandyToEvolve;
            FamilyId = (int)pokemonSettings.FamilyId;
            PokemonId = (int)pokemonSettings.PokemonId;
            BaseAttack = pokemonSettings.Stats.BaseAttack;
            BaseDefense = pokemonSettings.Stats.BaseDefense;
            BaseStamina = pokemonSettings.Stats.BaseStamina;
        }
        public PokemonSettings() { }

        public int BaseAttack { get; }
        public int BaseDefense { get; }
        public int BaseStamina { get; }
        public int CandyToEvolve { get; }
        public int FamilyId { get; }
        public int PokemonId { get; }
    }
}