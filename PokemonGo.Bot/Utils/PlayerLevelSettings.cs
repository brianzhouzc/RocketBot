using System.Collections.ObjectModel;
using System.Linq;

namespace PokemonGo.Bot.Utils
{
    public class PlayerLevelSettings
    {
        public PlayerLevelSettings(POGOProtos.Settings.Master.PlayerLevelSettings playerLevelTemplate)
        {
            CpMultiplier = playerLevelTemplate.CpMultiplier.ToList().AsReadOnly();
            MaxEggPlayerLevel = playerLevelTemplate.MaxEggPlayerLevel;
            MaxEncounterPlayerLevel = playerLevelTemplate.MaxEncounterPlayerLevel;
            RankNum = playerLevelTemplate.RankNum.ToList().AsReadOnly();
            RequiredExperience = playerLevelTemplate.RequiredExperience.ToList().AsReadOnly();
        }

        public ReadOnlyCollection<float> CpMultiplier { get; private set; }
        public int MaxEggPlayerLevel { get; private set; }
        public int MaxEncounterPlayerLevel { get; private set; }
        public ReadOnlyCollection<int> RankNum { get; private set; }
        public ReadOnlyCollection<int> RequiredExperience { get; private set; }
    }
}