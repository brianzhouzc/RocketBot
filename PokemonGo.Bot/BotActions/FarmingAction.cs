using GalaSoft.MvvmLight.Command;
using PokemonGo.Bot.ViewModels;
using PokemonGo.RocketAPI.Bot.utils;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonGo.Bot.BotActions
{
    public class FarmingAction : BotAction
    {
        bool shouldStop;
        IEnumerable<PokestopViewModel> route;

        public FarmingAction(BotViewModel bot) : base(bot, "Farm")
        {
        }

        [SuppressMessage("Await.Warning", "CS4014:Await.Warning", Justification = "ExecuteAsync runs in an infinite loop.")]
        protected override async Task OnStartAsync()
        {
            CalculateRoute();
            await ExecuteAsync();
        }

        void CalculateRoute()
        {
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
                await TransferPokemonWhileMoving(currentPokeStop);

                await FarmPokestopAsync(currentPokeStop);

                await CatchNearbyPokemonAsync();

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
            // get the pokestop from the map again as this one might have been disposed.
            currentPokeStop = bot.Map.Pokestops.SingleOrDefault(p => p.Id == currentPokeStop.Id);
            if (currentPokeStop != null)
            {
                await currentPokeStop.Details.ExecuteAsync();
                await currentPokeStop.Search.ExecuteAsync();
            }
        }
    }
}