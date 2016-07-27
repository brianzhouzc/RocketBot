using System;
using System.IO;
using PokemonGo.RocketAPI.Services;

namespace PokemonGo.RocketAPI.Console
{
    public class CommandLineConsoleWriterService : IConsoleWriterService
    {
        public void Write(ConsoleColor color, string text)
        {
            ConsoleColor originalColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color;
            System.Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss tt") + "] " + text);
            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Logs.txt", "[" + DateTime.Now.ToString("HH:mm:ss tt") + "] " + text + "\n");
            System.Console.ForegroundColor = originalColor;
        }
    }
}
