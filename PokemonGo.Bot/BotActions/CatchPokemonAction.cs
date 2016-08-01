using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.Bot.ViewModels;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.GeneratedCode;

namespace PokemonGo.Bot.BotActions
{
    public class CatchPokemonAction : BotAction
    {
        private readonly ISettings settings;
        readonly Client client;

        public CatchPokemonAction(BotViewModel bot, Client client, ISettings settings) : base(bot)
        {
            this.client = client;
            this.settings = settings;
        }

        protected override async Task OnStartAsync()
        {
            var pokemonOnMap = bot.Map.CatchablePokemon.ToList();

            foreach (var pokemon in pokemonOnMap)
            {
                await bot.Player.Move.ExecuteAsync(new PositionViewModel(pokemon.Latitude, pokemon.Longitude));

                var encounterPokemonResponse = await client.EncounterPokemon(pokemon.EncounterId, pokemon.SpawnpointId);
                var pokemonEncounter = new WilPokemonViewModel(encounterPokemonResponse.WildPokemon);
                var pokemonCP = pokemonEncounter.PokemonData.Cp;
                var pokemonIV = pokemonEncounter.PokemonData.GetIV();
                CatchPokemonResponse caughtPokemonResponse;
                do
                {
                    if (settings.RazzBerryMode == "cp")
                        if (pokemonCP > settings.RazzBerrySetting)
                            await client.UseRazzBerry(client, pokemon.EncounterId, pokemon.SpawnpointId);
                    if (settings.RazzBerryMode == "probability")
                        if (encounterPokemonResponse.CaptureProbability.CaptureProbability_.First() < settings.RazzBerrySetting)
                            await client.UseRazzBerry(client, pokemon.EncounterId, pokemon.SpawnpointId);
                    caughtPokemonResponse = await client.CatchPokemon(pokemon.EncounterId, pokemon.SpawnpointId, pokemon.Latitude, pokemon.Longitude, MiscEnums.Item.ITEM_POKE_BALL, pokemonCP); ; //note: reverted from settings because this should not be part of settings but part of logic
                } while (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed || caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchEscape);

                if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchSuccess)
                {
                    Color c = Color.LimeGreen;
                    if (pokemonIV >= 80)
                    {
                        c = Color.Yellow;
                    }
                    ColoredConsoleWrite(c, $"We caught a {pokemonName} with {pokemonCP} CP and {pokemonIV}% IV");
                    foreach (int xp in caughtPokemonResponse.Scores.Xp)
                        TotalExperience += xp;
                    TotalPokemon += 1;
                }
                else
                    ColoredConsoleWrite(Color.Red, $"{pokemonName} with {pokemonCP} CP and {pokemonIV}% IV got away..");
            }
        }
    }
}
