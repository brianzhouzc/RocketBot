namespace PokemonGo.Bot.Utils
{
    public class LevelSettings
    {
        public LevelSettings()
        {
        }

        public LevelSettings(POGOProtos.Settings.LevelSettings levelSettings)
        {
            TrainerCpModifier = levelSettings.TrainerCpModifier;
            TrainerDifficultyModifier = levelSettings.TrainerDifficultyModifier;
        }

        public double TrainerCpModifier { get; }
        public double TrainerDifficultyModifier { get; }
    }
}