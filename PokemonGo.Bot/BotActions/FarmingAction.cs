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

namespace PokemonGo.Bot.BotActions
{
    public class FarmingAction : BotAction
    {
        readonly Client client;
        bool shouldStop;
        IEnumerable<FortData> route;

        public FarmingAction(BotViewModel bot, Client client) : base(bot)
        {
            this.client = client;
        }

        protected override async Task OnStartAsync()
        {
            if (!bot.Player.IsLoggedIn)
                await bot.Player.Login.ExecuteAsync();

            await CalculateRoute();

        }

        async Task CalculateRoute()
        {
            await bot.Map.GetMapObjects.ExecuteAsync();
            var pokestopsNotOnCooldown = bot.Map.Pokestops.Where(p => p.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime());
            route = RouteOptimizer.Optimize(pokestopsNotOnCooldown, bot.Player.Position);
        }

        protected override async Task OnStopAsync()
        {
            shouldStop = true;
        }

        async void Execute()
        {
            var enumerator = route.GetEnumerator();
            if (!enumerator.MoveNext())
                await StopAsync();

            var currentPokeStop = enumerator.Current;

            while(!shouldStop)
            {
                await bot.Player.MoveTo(currentPokeStop.Latitude, currentPokeStop.Longitude);

                var fortInfo = await client.GetFort(currentPokeStop.Id, currentPokeStop.Latitude, currentPokeStop.Longitude);
                var fortSearch = await client.SearchFort(currentPokeStop.Id, currentPokeStop.Latitude, currentPokeStop.Longitude);
                currentPokeStop.CooldownCompleteTimestampMs = DateTime.UtcNow.ToUnixTime() + 300000;



                // queue next pokestop for farming
                if (!enumerator.MoveNext())
                {
                    enumerator = route.GetEnumerator();
                    enumerator.MoveNext();
                }
            }
        }
    }
}
