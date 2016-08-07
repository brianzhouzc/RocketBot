using PokemonGo.Bot.ViewModels;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Bot;
using PokemonGo.RocketAPI.Bot.utils;
using PokemonGo.RocketAPI.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonGo.Bot.BotActions
{
    public class FarmingAction : BotAction
    {
        private readonly Settings settings;
        private readonly Client client;
        private bool shouldStop;
        private IEnumerable<PokestopViewModel> route;

        public FarmingAction(BotViewModel bot, Client client, Settings settings) : base(bot, "Farm")
        {
            this.client = client;
            this.settings = settings;
        }

        [SuppressMessage("Await.Warning", "CS4014:Await.Warning", Justification = "ExecuteAsync runs in an infinite loop.")]
        protected override async Task OnStartAsync()
        {
            if (!bot.Player.IsLoggedIn)
                await bot.Player.Login.ExecuteAsync();

            await CalculateRouteAsync();
            ExecuteAsync();
        }

        private async Task CalculateRouteAsync()
        {
            await bot.Map.GetMapObjects.ExecuteAsync();
            var pokestopsNotOnCooldown = bot.Map.Pokestops.Where(p => p.IsActive);
            route = RouteOptimizer.Optimize(pokestopsNotOnCooldown, bot.Player.Position);
        }

        protected override Task OnStopAsync()
        {
            shouldStop = true;
            return Task.CompletedTask;
        }

        private async Task ExecuteAsync()
        {
            var enumerator = route.GetEnumerator();
            if (!enumerator.MoveNext())
                await StopAsync();

            while (!shouldStop)
            {
                var currentPokeStop = enumerator.Current;

                await MoveToPokestopAsync(currentPokeStop);
                await FarmPokestopAsync(currentPokeStop);

                await CatchNearbyPokemonAsync();
                await TransferUnwantedPokemonAsync();

                // queue next pokestop for farming
                if (!enumerator.MoveNext())
                {
                    enumerator = route.GetEnumerator();
                    enumerator.MoveNext();
                }
            }
        }

        private async Task TransferUnwantedPokemonAsync()
        {
            await new TransferPokemonWithAlgorithmAction(bot).StartAsync();
        }

        private async Task CatchNearbyPokemonAsync()
        {
            await new CatchPokemonAction(bot, client, settings).StartAsync();
        }

        private async Task MoveToPokestopAsync(PokestopViewModel currentPokeStop)
        {
            await bot.Player.Move.ExecuteAsync(currentPokeStop.Position);
        }

        private async Task FarmPokestopAsync(PokestopViewModel currentPokeStop)
        {
            var fortInfo = await client.Fort.GetFort(currentPokeStop.Id, currentPokeStop.Position.Latitude, currentPokeStop.Position.Longitude);
            await currentPokeStop.Search.ExecuteAsync();
        }
    }
}