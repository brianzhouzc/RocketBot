using System;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Tasks;
using POGOProtos.Enums;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class SnipeCommand : CommandMessage
    {
        public override string Command => "/snipe";
        public override string Description => "add snipe item <pokemon,lat,lng>";
        public override bool StopProcess => true;

        public SnipeCommand(TelegramUtils telegramUtils) : base(telegramUtils)
        {
        }

        public override async Task<bool> OnCommand(ISession session, string commandText, Action<string> Callback)
        {
            var cmd = commandText.Split(' ');

            if (cmd[0].ToLower() == Command)
            {
                var pokemonData = cmd[1].Split(',');
                PokemonId pid = (PokemonId) Enum.Parse(typeof(PokemonId), pokemonData[0].Trim(), true);

                await MSniperServiceTask.AddSnipeItem(session, new MSniperServiceTask.MSniperInfo2()
                {
                    PokemonId = (short) pid,
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