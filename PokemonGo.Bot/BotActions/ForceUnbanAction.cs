using PokemonGo.Bot.ViewModels;
using PokemonGo.RocketAPI;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonGo.Bot.BotActions
{
    public class ForceUnbanAction : BotAction
    {
        private readonly Client client;
        private bool shouldStop;

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

        private async Task<PokestopViewModel> GetNearestPokestopNotOnCooldownAsync()
        {
            await bot.Map.GetMapObjects.ExecuteAsync();
            var pokestopsNotOnCooldown = bot.Map.Pokestops.Where(p => p.IsActive);
            var nearestPokeStop = pokestopsNotOnCooldown.OrderBy(p => bot.Player.Position.DistanceTo(p.Position)).FirstOrDefault();
            return nearestPokeStop;
        }

        private async Task ForceUnbanAsync()
        {
            var pokestop = await GetNearestPokestopNotOnCooldownAsync();
            await bot.Player.Move.ExecuteAsync(pokestop.Position);
            var fortInfo = await client.Fort.GetFort(pokestop.Id, pokestop.Position.Latitude, pokestop.Position.Longitude);
            var unbanned = false;
            while (!unbanned || !shouldStop)
            {
                // TODO use PokestopViewModel
                var fortSearch = await client.Fort.SearchFort(pokestop.Id, pokestop.Position.Latitude, pokestop.Position.Longitude);
                if (fortSearch.ExperienceAwarded > 0)
                    unbanned = true;
            }
        }
    }
}