using PokemonGo.Bot.ViewModels;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Bot;
using System;

namespace PokemonGo.Bot.BotActions
{
    public class ActionFactory
    {
        readonly Settings settings;
        readonly Client client;
        readonly BotViewModel bot;

        public ActionFactory(BotViewModel bot, Client client, Settings settings)
        {
            this.bot = bot;
            this.client = client;
            this.settings = settings;
        }

        public BotAction Get(BotActionType action)
        {
            switch (action)
            {
                case BotActionType.Farm:
                    return new FarmingAction(bot, client, settings);

                case BotActionType.ForceUnban:
                    return new ForceUnbanAction(bot, client);

                case BotActionType.TransferPokemon:
                    return new TransferPokemonWithAlgorithmAction(bot);

                default:
                    throw new ArgumentOutOfRangeException(nameof(action), "Unknown action");
            }
        }
    }
}