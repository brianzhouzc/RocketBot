using System;

namespace PokemonGo.RocketAPI.Services
{
    public interface IExperienceService
    {
        int GetExperienceForLevel(int level);
    }
}
