using PokemonGo.Bot.ViewModels;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.GeneratedCode;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonGo.Bot.BotActions
{
    public class ForceUnbanAction : BotAction
    {
        readonly Client client;
        bool shouldStop;

        public ForceUnbanAction(BotViewModel bot, Client client) : base(bot, "Force unban")
        {
            this.client = client;
        }

        protected override Task OnStartAsync() => ForceUnbanAsync();

        protected override Task OnStopAsync()
        {
            shouldStop = true;
            return Task.CompletedTask;
        }

        async Task<FortData> GetNearestPokestopNotOnCooldownAsync()
        {
            await bot.Map.GetMapObjects.ExecuteAsync();
            var pokestopsNotOnCooldown = bot.Map.Pokestops.Where(p => p.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime());
            var nearestPokeStop = pokestopsNotOnCooldown.OrderBy(p => bot.Player.Position.DistanceTo(new PositionViewModel(p.Latitude, p.Longitude))).FirstOrDefault();
            return nearestPokeStop;
        }

        async Task ForceUnbanAsync()
        {
            var pokestop = await GetNearestPokestopNotOnCooldownAsync();
            await bot.Player.Move.ExecuteAsync(new PositionViewModel(pokestop.Latitude, pokestop.Longitude));
            var fortInfo = await client.GetFort(pokestop.Id, pokestop.Latitude, pokestop.Longitude);
            var unbanned = false;
            while (!unbanned || !shouldStop)
            {
                var fortSearch = await client.SearchFort(pokestop.Id, pokestop.Latitude, pokestop.Longitude);
                if (fortSearch.ExperienceAwarded > 0)
                    unbanned = true;
            }
        }
    }
}