using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.Bot.ViewModels;

namespace PokemonGo.Bot.BotActions
{
    public class TransferPokemonWithAlgorithmAction : BotAction
    {
        public TransferPokemonWithAlgorithmAction(BotViewModel bot) : base(bot, "Transfer Pokemon")
        {

        }

        protected override async Task OnStartAsync()
        {
            await bot.Player.Inventory.TransferPokemonWithAlgorithm.ExecuteAsync();
            await StopAsync();
        }
    }
}
