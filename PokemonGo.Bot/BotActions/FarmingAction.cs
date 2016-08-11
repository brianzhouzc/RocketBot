using GalaSoft.MvvmLight.Command;
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
        readonly Settings settings;
        readonly Client client;
        bool shouldStop;
        IEnumerable<PokestopViewModel> route;

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
            await ExecuteAsync();
        }

        async Task CalculateRouteAsync()
        {
            //await bot.Map.GetMapObjects.ExecuteAsync();
            var pokestopsNotOnCooldown = bot.Map.Pokestops.Where(p => p.IsActive);
            route = RouteOptimizer.Optimize(pokestopsNotOnCooldown, bot.Player.Position);
            Route.Clear();
            Route.AddRange(route.Select(r => r.Position));
            RaisePropertyChanged(nameof(Route));
        }

        protected override Task OnStopAsync()
        {
            shouldStop = true;
            return Task.CompletedTask;
        }

        async Task ExecuteAsync()
        {
            var enumerator = route.GetEnumerator();
            if (!enumerator.MoveNext())
                await StopAsync();

            while (!shouldStop)
            {
                var currentPokeStop = enumerator.Current;

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

        async Task TransferUnwantedPokemonAsync()
        {
            await bot.Player.Inventory.Load.ExecuteAsync();
            await new TransferPokemonWithAlgorithmAction(bot).StartAsync();
        }

        async Task CatchNearbyPokemonAsync()
        {
            // have to use a backwards for loop here, because pokemon.Catch will remove the caught pokemon from the map.
            for (int i = bot.Map.CatchablePokemon.Count - 1; i >= 0; i--)
            {
                var pokemon = bot.Map.CatchablePokemon[i];
                await pokemon.Catch.ExecuteAsync();
            }
        }

        Task TransferPokemonWhileMoving(PokestopViewModel targetPokestop)
            => Task.WhenAll(bot.Player.Move.ExecuteAsync(targetPokestop.Position), TransferUnwantedPokemonAsync());

        async Task FarmPokestopAsync(PokestopViewModel currentPokeStop)
        {
            await bot.Player.Move.ExecuteAsync(currentPokeStop.Position);
            var fortInfo = await client.Fort.GetFort(currentPokeStop.Id, currentPokeStop.Position.Latitude, currentPokeStop.Position.Longitude);
            await currentPokeStop.Search.ExecuteAsync();
        }
    }
}