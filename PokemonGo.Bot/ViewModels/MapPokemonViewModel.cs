using GalaSoft.MvvmLight.Command;
using POGOProtos.Map.Pokemon;
using POGOProtos.Networking.Responses;
using PokemonGo.Bot.Messages;
using PokemonGo.Bot.MVVMLightUtils;
using PokemonGo.Bot.Utils;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Bot;
using System.Linq;
using System.Windows.Media;
using System;
using POGOProtos.Map.Fort;

namespace PokemonGo.Bot.ViewModels
{
    public class MapPokemonViewModel : PokemonViewModel, IUpdateable<MapPokemonViewModel>
    {
        readonly SessionViewModel session;

        public MapPokemonViewModel(MapPokemon pokemon, SessionViewModel session, Settings settings, PlayerViewModel player, MapViewModel map)
            : base(pokemon.PokemonId, pokemon.EncounterId)
        {
            this.session = session;
            EncounterId = pokemon.EncounterId;
            ExpirationTimestampMs = pokemon.ExpirationTimestampMs;
            SpawnPointId = pokemon.SpawnPointId;
            Position = new Position2DViewModel(pokemon.Latitude, pokemon.Longitude);

            Catch = new AsyncRelayCommand(async () =>
            {
                var encounterPokemonResponse = await session.EncounterPokemon(pokemon.EncounterId, pokemon.SpawnPointId);
                if (encounterPokemonResponse.Status == EncounterResponse.Types.Status.EncounterSuccess)
                {
                    var pokemonEncounter = new WildPokemonViewModel(encounterPokemonResponse.WildPokemon);
                    var pokemonCP = pokemonEncounter.CombatPoints;
                    var pokemonIV = pokemonEncounter.PerfectPercentage;
                    CatchPokemonResponse caughtPokemonResponse;
                    do
                    {
                        // returns an exception at the moment.
                        //if (settings.UseRazzBerryWhenPokemonIsAboveCP <= pokemonCP ||
                        //    settings.UseRazzBerryWhenCatchProbabilityIsBelow <= encounterPokemonResponse.CaptureProbability.CaptureProbability_.First())
                        //    await session.UseCaptureItem(pokemon.EncounterId, POGOProtos.Inventory.Item.ItemId.ItemRazzBerry, pokemon.SpawnPointId);

                        // TODO calculate pokeball based on encountered pokemon
                        caughtPokemonResponse = await session.CatchPokemon(pokemon.EncounterId, pokemon.SpawnPointId, POGOProtos.Inventory.Item.ItemId.ItemPokeBall);
                    } while (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed || caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchEscape);

                    if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchSuccess)
                    {
                        encounterPokemonResponse.WildPokemon.PokemonData.Id = caughtPokemonResponse.CapturedPokemonId;
                        var caughtPokemon = new CaughtPokemonViewModel(encounterPokemonResponse.WildPokemon.PokemonData, session, player.Inventory);
                        var xp = caughtPokemonResponse.CaptureAward.Xp.Sum();
                        player.Xp += xp;
                        var stardust = caughtPokemonResponse.CaptureAward.Stardust.Sum();
                        player.Stardust += stardust;
                        var candy = caughtPokemonResponse.CaptureAward.Candy.Sum();
                        player.Inventory.AddCandyForFamily(candy, caughtPokemon.FamilyId);
                        MessengerInstance.Send(new Message(Colors.Green, $"Caught a {pokemonEncounter.Name}. {xp} Xp - {candy} Candy - {stardust} Stardust."));
                        player.Inventory.Pokemon.AddOrUpdate(caughtPokemon);
                        map.CatchablePokemon.Remove(this);
                    }
                    else if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchFlee)
                    {
                        MessengerInstance.Send(new Message(Colors.Red, $"Failed to catch a {pokemonEncounter.Name}, because it fled."));
                    }
                }
            });
        }


        public MapPokemonViewModel(FortData fort, SessionViewModel session, Settings settings, PlayerViewModel player, MapViewModel map)
            : base(fort.LureInfo.ActivePokemonId, fort.LureInfo.EncounterId)
        {
            var lureInfo = fort.LureInfo;
            this.session = session;
            EncounterId = lureInfo.EncounterId;
            ExpirationTimestampMs = lureInfo.LureExpiresTimestampMs;
            SpawnPointId = lureInfo.FortId;
            Position = new Position2DViewModel(fort.Latitude, fort.Longitude);

            Catch = new AsyncRelayCommand(async () =>
            {
                var encounterPokemonResponse = await session.EncounterDiskPokemon(lureInfo.EncounterId, lureInfo.FortId);
                if (encounterPokemonResponse.Result == DiskEncounterResponse.Types.Result.Success)
                {
                    var pokemonEncounter = new WildPokemonViewModel(encounterPokemonResponse.PokemonData, fort);
                    var pokemonCP = pokemonEncounter.CombatPoints;
                    var pokemonIV = pokemonEncounter.PerfectPercentage;
                    CatchPokemonResponse caughtPokemonResponse;
                    do
                    {
                        // returns an exception at the moment.
                        //if (settings.UseRazzBerryWhenPokemonIsAboveCP <= pokemonCP ||
                        //    settings.UseRazzBerryWhenCatchProbabilityIsBelow <= encounterPokemonResponse.CaptureProbability.CaptureProbability_.First())
                        //    await session.UseCaptureItem(pokemon.EncounterId, POGOProtos.Inventory.Item.ItemId.ItemRazzBerry, pokemon.SpawnPointId);

                        // TODO calculate pokeball based on encountered pokemon
                        caughtPokemonResponse = await session.CatchPokemon(lureInfo.EncounterId, lureInfo.FortId, POGOProtos.Inventory.Item.ItemId.ItemPokeBall);
                    } while (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed || caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchEscape);

                    if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchSuccess)
                    {
                        encounterPokemonResponse.PokemonData.Id = caughtPokemonResponse.CapturedPokemonId;
                        var caughtPokemon = new CaughtPokemonViewModel(encounterPokemonResponse.PokemonData, session, player.Inventory);
                        var xp = caughtPokemonResponse.CaptureAward.Xp.Sum();
                        player.Xp += xp;
                        var stardust = caughtPokemonResponse.CaptureAward.Stardust.Sum();
                        player.Stardust += stardust;
                        var candy = caughtPokemonResponse.CaptureAward.Candy.Sum();
                        player.Inventory.AddCandyForFamily(candy, caughtPokemon.FamilyId);
                        MessengerInstance.Send(new Message(Colors.Green, $"Caught a {pokemonEncounter.Name}. {xp} Xp - {candy} Candy - {stardust} Stardust."));
                        player.Inventory.Pokemon.AddOrUpdate(caughtPokemon);
                        map.CatchablePokemon.Remove(this);
                    }
                    else if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchFlee)
                    {
                        MessengerInstance.Send(new Message(Colors.Red, $"Failed to catch a {pokemonEncounter.Name}, because it fled."));
                    }
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

        public void UpdateWith(MapPokemonViewModel other)
        {
            if (!Equals(other))
                throw new ArgumentException($"Expected a {Name} with Id {Id} but got a {other?.Name} with Id {other?.Id}", nameof(other));

            // nothing to update
        }
    }
}