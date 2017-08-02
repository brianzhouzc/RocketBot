#region using directives

using System;
using System.Windows.Forms;
using RocketBot2.Forms;
using RocketBot2.Win32;

#endregion

namespace RocketBot2
{
    internal class Program
    {
        [STAThread]

        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            ConsoleHelper.AllocConsole();
            if (args.Length < 1)
                ConsoleHelper.HideConsoleWindow();
            Application.Run(new MainForm(args));
        }
    }
}