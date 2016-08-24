using PokemonGo.Bot.Utils;
using PokemonGo.Bot.ViewModels;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Bot;
using System;

namespace PokemonGo.Bot.BotActions
{
    public class ActionFactory
    {
        readonly BotViewModel bot;

        public ActionFactory(BotViewModel bot)
        {
            this.bot = bot;
        }

        public BotAction Get(BotActionType action)
        {
            switch (action)
            {
                case BotActionType.Farm:
                    return new FarmingAction(bot);

                case BotActionType.ForceUnban:
                    return new ForceUnbanAction(bot);

                case BotActionType.TransferPokemon:
                    return new TransferPokemonWithAlgorithmAction(bot);

                default:
                    throw new ArgumentOutOfRangeException(nameof(action), "Unknown action");
            }
        }
    }
}