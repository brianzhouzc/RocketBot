#region using directives

using System;
using System.Globalization;
using System.Threading;
using PokemonGo.RocketBot.Logic;
using PokemonGo.RocketBot.Logic.Common;
using PokemonGo.RocketBot.Logic.Event;
using PokemonGo.RocketBot.Logic.Logging;
using PokemonGo.RocketBot.Logic.State;
using PokemonGo.RocketBot.Logic.Tasks;
using PokemonGo.RocketBot.Logic.Utils;
using System.IO;
using System.Net;
using PokemonGo.RocketBot.Window.Resources;
using System.Reflection;
using PokemonGo.RocketBot.Window.Plugin;
using System.Windows.Forms;

#endregion

namespace PokemonGo.RocketBot.Window
{
    internal class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new MainForm());
        }
    }
}
