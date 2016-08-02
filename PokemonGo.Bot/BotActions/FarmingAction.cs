using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.Bot.ViewModels;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.Bot.utils;
using PokemonGo.RocketAPI.GeneratedCode;
using PokemonGo.RocketAPI;
using System.Diagnostics.CodeAnalysis;

namespace PokemonGo.Bot.BotActions
{
    public class FarmingAction : BotAction
    {
        readonly ISettings settings;
        readonly Client client;
        bool shouldStop;
        IEnumerable<FortData> route;

        public FarmingAction(BotViewModel bot, Client client, ISettings settings) : base(bot)
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

        async Task CalculateRouteAsync()
        {
            await bot.Map.GetMapObjects.ExecuteAsync();
            var pokestopsNotOnCooldown = bot.Map.Pokestops.Where(p => p.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime());
            route = RouteOptimizer.Optimize(pokestopsNotOnCooldown, bot.Player.Position);
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


            while(!shouldStop)
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

        async Task TransferUnwantedPokemonAsync()
        {
            await new TransferPokemonWithAlgorithmAction(bot).StartAsync();
        }

        async Task CatchNearbyPokemonAsync()
        {
            await new CatchPokemonAction(bot, client, settings).StartAsync();
        }

        async Task MoveToPokestopAsync(FortData currentPokeStop)
        {
            await bot.Player.Move.ExecuteAsync(new PositionViewModel(currentPokeStop.Latitude, currentPokeStop.Longitude));
        }

        async Task FarmPokestopAsync(FortData currentPokeStop)
        {
            var fortInfo = await client.GetFort(currentPokeStop.Id, currentPokeStop.Latitude, currentPokeStop.Longitude);
            var fortSearch = await client.SearchFort(currentPokeStop.Id, currentPokeStop.Latitude, currentPokeStop.Longitude);
            currentPokeStop.CooldownCompleteTimestampMs = DateTime.UtcNow.ToUnixTime() + 300000;
        }
    }
}
