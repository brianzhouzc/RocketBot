using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NDesk.Options;

namespace PokemonGo.RocketAPI.Window
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            var needStart = false;

            var p = new OptionSet() {
                { "s|start", "start bot immediately",
                  v => needStart = true}
            };

            try
            {
                p.Parse(args);
            }
            catch (OptionException)
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(needStart));
        }
    }
}
