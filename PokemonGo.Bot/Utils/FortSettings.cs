namespace PokemonGo.Bot.Utils
{
    public class FortSettings
    {
        public FortSettings()
        {
        }

        public FortSettings(POGOProtos.Settings.FortSettings fortSettings)
        {
            DeployAttackMultiplier = fortSettings.DeployAttackMultiplier;
            DeployStaminaMultiplier = fortSettings.DeployStaminaMultiplier;
            FarInteractionRangeMeters = fortSettings.FarInteractionRangeMeters;
            InteractionRangeMeters = fortSettings.InteractionRangeMeters;
            MaxPlayerDeployedPokemon = fortSettings.MaxPlayerDeployedPokemon;
            MaxTotalDeployedPokemon = fortSettings.MaxTotalDeployedPokemon;
        }

        public double DeployAttackMultiplier { get; }
        public double DeployStaminaMultiplier { get; }
        public double FarInteractionRangeMeters { get; }
        public double InteractionRangeMeters { get; }
        public int MaxPlayerDeployedPokemon { get; }
        public int MaxTotalDeployedPokemon { get; }
    }
}