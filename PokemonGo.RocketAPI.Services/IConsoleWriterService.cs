using System;

namespace PokemonGo.RocketAPI.Services
{
    public interface IConsoleWriterService
    {
        void Write(ConsoleColor color, string text);
    }
}
