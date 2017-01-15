#region using directives

using System;
using System.Windows.Forms;
using NecroBot2.Forms;

#endregion

namespace NecroBot2
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.Run(new MainForm(args));
        }
    }
}