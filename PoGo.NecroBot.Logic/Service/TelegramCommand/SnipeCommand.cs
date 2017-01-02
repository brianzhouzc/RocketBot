using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Tasks;
using POGOProtos.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class SnipeCommand : ICommand
    {
        public string Command  => "/snipe";
        public string Description => "add snipe item <pokemon,lat,lng>";
        public bool StopProcess => true;

        public async Task<bool> OnCommand(ISession session,string commandText, Action<string> Callback)
        {
            var cmd = commandText.Split(' ');

            if (cmd[0].ToLower() == Command)
            {
                var pokemonData = cmd[1].Split(',');
                PokemonId pid = (PokemonId)Enum.Parse(typeof(PokemonId), pokemonData[0].Trim(), true);

                await MSniperServiceTask.AddSnipeItem(session, new MSniperServiceTask.MSniperInfo2() {
                    PokemonId = (short)pid,
                    Latitude = Convert.ToDouble(pokemonData[1].Trim()),
                    Longitude = Convert.ToDouble(pokemonData[2].Trim())
                }, true);
                //Callback("Snipe pokemon added");
                return true;
            }
            return false;
        }
    }
}
