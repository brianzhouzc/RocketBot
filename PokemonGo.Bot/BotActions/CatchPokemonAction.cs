using POGOProtos.Networking.Responses;
using PokemonGo.Bot.ViewModels;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Bot;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonGo.Bot.BotActions
{
    public class CatchPokemonAction : BotAction
    {
        private readonly Settings settings;
        private readonly Client client;

        public CatchPokemonAction(BotViewModel bot, Client client, Settings settings) : base(bot, "Catch Pokemon")
        {
            this.client = client;
            this.settings = settings;
        }

        protected override async Task OnStartAsync()
        {
            var pokemonOnMap = bot.Map.CatchablePokemon.ToList();

            foreach (var pokemon in pokemonOnMap)
            {
                await bot.Player.Move.ExecuteAsync(pokemon.Position);

                var encounterPokemonResponse = await client.Encounter.EncounterPokemon(pokemon.EncounterId, pokemon.SpawnPointId);
                var pokemonEncounter = new WildPokemonViewModel(encounterPokemonResponse.WildPokemon);
                var pokemonCP = pokemonEncounter.CombatPoints;
                var pokemonIV = pokemonEncounter.PerfectPercentage;
                CatchPokemonResponse caughtPokemonResponse;
                do
                {
                    if (settings.RazzBerryMode == "cp" && pokemonCP > settings.RazzBerrySetting ||
                        settings.RazzBerryMode == "probability" && encounterPokemonResponse.CaptureProbability.CaptureProbability_.First() < settings.RazzBerrySetting)
                        await client.Encounter.UseCaptureItem(pokemon.EncounterId, POGOProtos.Inventory.Item.ItemId.ItemRazzBerry, pokemon.SpawnPointId);

                    // TODO calculate pokeball based on encountered pokemon
                    caughtPokemonResponse = await client.Encounter.CatchPokemon(pokemon.EncounterId, pokemon.SpawnPointId, POGOProtos.Inventory.Item.ItemId.ItemPokeBall);
                } while (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed || caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchEscape);

                if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchSuccess)
                {
                    bot.Player.Xp += caughtPokemonResponse.CaptureAward.Xp.Sum();
                    bot.Player.Stardust += caughtPokemonResponse.CaptureAward.Stardust.Sum();
                    // TODO CANDY
                }
            }
        }
    }
}