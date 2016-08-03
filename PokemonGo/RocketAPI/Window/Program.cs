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
            var parameters = new Dictionary<string,string>();
            var optionSet = new OptionSet
            {
                { "s=|start=", "Need to start the bot immediately.",
                  v => parameters.Add("NeedStart", v)},
                { "l=|login=|email=", "Login or email.",
                  v => parameters.Add("Username", v)},
                { "p=|password=", "Password.",
                  v => parameters.Add("Password", v)},
                { "at=|authtype=", "Authtype.",
                  v => parameters.Add("AuthType", v)},
                { "lo=|longitude=", "Longitude.",
                  v => parameters.Add("DefaultLongitude", v)},
                { "la=|latitude=", "Latitude.",
                  v => parameters.Add("DefaultLatitude", v)},
                { "rm=|razzberrymode=", "Razzberry mode.",
                  v => parameters.Add("RazzBerryMode", v)},
                { "rs=|razzberrysetting=", "Razzberry setting.",
                  v => parameters.Add("RazzBerrySetting", v)},
                { "tt=|transfertype=", "Transfer type.",
                  v => parameters.Add("TransferType", v)},
                { "ts=|travelspeed=", "Travel speed.",
                  v => parameters.Add("TravelSpeed", v)},
                { "c=|catch=", "Catch pokemons. Default = true",
                  v => parameters.Add("CatchPokemon", v)},
                { "ev=|evolve=", "Evolve pokemons. Default = true",
                  v => parameters.Add("EvolveAllGivenPokemons", v)},
            };

            try
            {
                optionSet.Parse(args);
            }
            catch (OptionException)
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(parameters));
        }
    }
}
