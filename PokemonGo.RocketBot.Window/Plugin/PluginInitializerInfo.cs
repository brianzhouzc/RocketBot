using PokemonGo.RocketBot.Logic;
using PokemonGo.RocketBot.Logic.State;
using PokemonGo.RocketBot.Logic.Utils;

namespace PokemonGo.RocketBot.Window.Plugin
{
    public class PluginInitializerInfo
    {
        public Session Session { get; set; }
        public GlobalSettings Settings { get; set; }
        public ConsoleLogger Logger { get; set; }
        public Statistics Statistics { get; set; }
    }
}