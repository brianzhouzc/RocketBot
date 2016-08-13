#region using directives

using System;
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