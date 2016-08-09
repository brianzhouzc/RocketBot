using GalaSoft.MvvmLight.Command;
using POGOProtos.Map.Pokemon;
using POGOProtos.Networking.Responses;
using PokemonGo.Bot.Messages;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Bot;
using System.Linq;

namespace PokemonGo.Bot.ViewModels
{
    public class MapPokemonViewModel : PokemonViewModel
    {
        readonly Client client;

        public MapPokemonViewModel(MapPokemon pokemon, Client client, Settings settings, PlayerViewModel player)
            : base(pokemon.PokemonId, pokemon.EncounterId)
        {
            this.client = client;
            EncounterId = pokemon.EncounterId;
            ExpirationTimestampMs = pokemon.ExpirationTimestampMs;
            SpawnPointId = pokemon.SpawnPointId;
            Position = new Position2DViewModel(pokemon.Latitude, pokemon.Longitude);

            Catch = new AsyncRelayCommand(async () =>
            {
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
                    player.Xp += caughtPokemonResponse.CaptureAward.Xp.Sum();
                    player.Stardust += caughtPokemonResponse.CaptureAward.Stardust.Sum();
                    MessengerInstance.Send(new Message($"Cought a {pokemonEncounter.Name}"));
                    encounterPokemonResponse.WildPokemon.PokemonData.Id = caughtPokemonResponse.CapturedPokemonId;
                    player.Inventory.Pokemon.Add(new CaughtPokemonViewModel(encounterPokemonResponse.WildPokemon.PokemonData, client, player.Inventory));
                    // TODO CANDY
                }
            });
        }

        public ulong EncounterId { get; }
        public long ExpirationTimestampMs { get; }

        public Position2DViewModel Position { get; }
        public string SpawnPointId { get; }

        public AsyncRelayCommand Catch { get; }

        public override bool Equals(object obj) => Equals(obj as MapPokemonViewModel);

        public bool Equals(MapPokemonViewModel other)
        {
            return other != null &&
                base.Equals(other) &&
                Position.Equals(other.Position);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 13;
                hash = (hash * 7) + base.GetHashCode();
                hash = (hash * 7) + Position.GetHashCode();
                return hash;
            }
        }
    }
}