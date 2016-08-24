using PokemonGo.Bot.ViewModels;
using PokemonGo.RocketAPI;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonGo.Bot.BotActions
{
    public class ForceUnbanAction : BotAction
    {
        bool shouldStop;

        public ForceUnbanAction(BotViewModel bot) : base(bot, "Force unban")
        {
        }

        protected override Task OnStartAsync() => ForceUnbanAsync();

        protected override Task OnStopAsync()
        {
            shouldStop = true;
            return Task.CompletedTask;
        }

        PokestopViewModel GetNearestPokestopNotOnCooldown()
        {
            var pokestopsNotOnCooldown = bot.Map.Pokestops.Where(p => p.IsActive);
            var nearestPokeStop = pokestopsNotOnCooldown.OrderBy(p => bot.Player.Position.DistanceTo(p.Position)).FirstOrDefault();
            return nearestPokeStop;
        }

        async Task ForceUnbanAsync()
        {
            var pokestop = GetNearestPokestopNotOnCooldown();
            Route.Clear();
            Route.Add(pokestop.Position);
            RaisePropertyChanged(nameof(Route));
            await bot.Player.Move.ExecuteAsync(pokestop.Position);
            await pokestop.Details.ExecuteAsync();
            var unbanned = false;
            var startXp = bot.Player.Xp;
            while (!unbanned || !shouldStop)
            {
                var fortSearch = pokestop.Search.ExecuteAsync();
                if (bot.Player.Xp > startXp)
                    unbanned = true;
            }
            await StopAsync();
        }
    }
}