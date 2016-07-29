using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.Window
{
    public static class ColorMap
    {
        private static Dictionary<ConsoleColor, Color> _map = new Dictionary<ConsoleColor, Color>();

        static ColorMap()
        {
            // these won't be exact matches, see http://stackoverflow.com/a/28211539
            _map.Add(ConsoleColor.Black, Color.Black);
            _map.Add(ConsoleColor.DarkBlue, Color.DarkBlue);
            _map.Add(ConsoleColor.DarkGreen, Color.DarkGreen);
            _map.Add(ConsoleColor.DarkCyan, Color.DarkCyan);
            _map.Add(ConsoleColor.DarkRed, Color.DarkRed);
            _map.Add(ConsoleColor.DarkMagenta, Color.DarkMagenta);
            _map.Add(ConsoleColor.DarkYellow, ColorTranslator.FromHtml("#808000"));
            _map.Add(ConsoleColor.Gray, Color.Gray);
            _map.Add(ConsoleColor.DarkGray, Color.DarkGray);
            _map.Add(ConsoleColor.Blue, Color.Blue);
            _map.Add(ConsoleColor.Green, Color.Green);
            _map.Add(ConsoleColor.Cyan, Color.Cyan);
            _map.Add(ConsoleColor.Red, Color.Red);
            _map.Add(ConsoleColor.Magenta, Color.Magenta);
            _map.Add(ConsoleColor.Yellow, Color.Yellow);
            _map.Add(ConsoleColor.White, Color.White);
        }

        public static Color GetColor(ConsoleColor source)
        {
            return _map[source];
        }
    }
}
