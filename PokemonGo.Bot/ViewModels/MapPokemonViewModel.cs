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
using System.Threading.Tasks;
using POGOProtos.Data;

namespace PokemonGo.Bot.ViewModels
{
    public class MapPokemonViewModel : PokemonViewModel, IUpdateable<MapPokemonViewModel>
    {
        readonly SessionViewModel session;
        readonly Settings settings;
        readonly PlayerViewModel player;
        readonly MapViewModel map;
        readonly bool isPokemonFromLure;
        readonly FortData fort;

        AsyncRelayCommand @catch;

        public AsyncRelayCommand Catch
        {
            get
            {
                if (@catch == null)
                    @catch = new AsyncRelayCommand(ExecuteCatchCommand);

                return @catch;
            }
        }

        async Task ExecuteCatchCommand()
        {
            var encounterPokemonResponse = await Encounter();
            if (encounterPokemonResponse.IsSuccess)
            {
                MessengerInstance.Send(new Message(Colors.Green, $"Encountered a {encounterPokemonResponse.Pokemon.Name}"));
                var pokeball = GetPokeBall();
                CatchPokemonResponse caughtPokemonResponse = null;
                do
                {
                    // returns an exception at the moment.
                    //if (settings.UseRazzBerryWhenPokemonIsAboveCP <= pokemonCP ||
                    //    settings.UseRazzBerryWhenCatchProbabilityIsBelow <= encounterPokemonResponse.CaptureProbability.CaptureProbability_.First())
                    //    await session.UseCaptureItem(pokemon.EncounterId, POGOProtos.Inventory.Item.ItemId.ItemRazzBerry, pokemon.SpawnPointId);

                    // TODO calculate pokeball based on encountered pokemon
                    if (pokeball.Count > 0)
                    {
                        MessengerInstance.Send(new Message(Colors.Green, $"Using a {pokeball.Name}"));
                        pokeball.Count--;
                        caughtPokemonResponse = await session.CatchPokemon(EncounterId, SpawnPointId, (POGOProtos.Inventory.Item.ItemId)pokeball.ItemType);
                    }
                    else
                    {
                        MessengerInstance.Send(new Message(Colors.Yellow, "No pokeballs left."));
                    }
                } while (pokeball.Count > 0 && (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed || caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchEscape));

                if (caughtPokemonResponse != null)
                {
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
                        MessengerInstance.Send(new Message(Colors.Green, $"Caught a {encounterPokemonResponse.Pokemon.Name}. {xp} Xp - {candy} Candy - {stardust} Stardust."));
                        player.Inventory.Pokemon.AddOrUpdate(caughtPokemon);
                        map.CatchablePokemon.Remove(this);
                    }
                    else if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchFlee)
                    {
                        MessengerInstance.Send(new Message(Colors.Red, $"Failed to catch a {encounterPokemonResponse.Pokemon.Name}, because it fled."));
                    }
                }
            }
        }

        private ItemViewModel GetPokeBall()
        {
            var pokeballs = player.Inventory.Items.FirstOrDefault(i => i.ItemType == Enums.ItemType.ItemPokeBall);
            if (pokeballs.Count > 0)
                return pokeballs;
            var greatBalls = player.Inventory.Items.FirstOrDefault(i => i.ItemType == Enums.ItemType.ItemGreatBall);
            return greatBalls;
        }

        async Task<EncounterResponse> Encounter()
        {
            WildPokemonViewModel pokemonEncounter = null;
            PokemonData pokemonData = null;
            if (!isPokemonFromLure)
            {
                var encounterPokemonResponse = await session.EncounterPokemon(EncounterId, SpawnPointId);
                if (encounterPokemonResponse.Status == POGOProtos.Networking.Responses.EncounterResponse.Types.Status.EncounterSuccess)
                {
                    pokemonEncounter = new WildPokemonViewModel(encounterPokemonResponse.WildPokemon);
                    pokemonData = encounterPokemonResponse.WildPokemon.PokemonData;
                }
            }
            else
            {
                var encounterPokemonResponse = await session.EncounterDiskPokemon(EncounterId, SpawnPointId);
                if (encounterPokemonResponse.Result == DiskEncounterResponse.Types.Result.Success)
                {
                    pokemonEncounter = new WildPokemonViewModel(encounterPokemonResponse.PokemonData, fort);
                    pokemonData = encounterPokemonResponse.PokemonData;
                }
            }

            return new EncounterResponse(pokemonEncounter, pokemonData);
        }

        class EncounterResponse
        {
            public EncounterResponse(WildPokemonViewModel pokemon, PokemonData pokemonData)
            {
                IsSuccess = pokemon != null;
                Pokemon = pokemon;
                PokemonData = pokemonData;
            }
            public bool IsSuccess { get; }
            public WildPokemonViewModel Pokemon { get; }
            public PokemonData PokemonData { get; }
        }


        public MapPokemonViewModel(MapPokemon pokemon, SessionViewModel session, Settings settings, PlayerViewModel player, MapViewModel map)
            : base(pokemon.PokemonId, pokemon.EncounterId)
        {
            this.session = session;
            this.settings = settings;
            this.player = player;
            this.map = map;

            EncounterId = pokemon.EncounterId;
            ExpirationTimestampMs = pokemon.ExpirationTimestampMs;
            SpawnPointId = pokemon.SpawnPointId;
            Position = new Position2DViewModel(pokemon.Latitude, pokemon.Longitude);
        }


        public MapPokemonViewModel(FortData fort, SessionViewModel session, Settings settings, PlayerViewModel player, MapViewModel map)
            : base(fort.LureInfo.ActivePokemonId, fort.LureInfo.EncounterId)
        {
            this.session = session;
            this.settings = settings;
            this.player = player;
            this.map = map;

            isPokemonFromLure = true;
            this.fort = fort;
            var lureInfo = fort.LureInfo;
            EncounterId = lureInfo.EncounterId;
            ExpirationTimestampMs = lureInfo.LureExpiresTimestampMs;
            SpawnPointId = lureInfo.FortId;
            Position = new Position2DViewModel(fort.Latitude, fort.Longitude);
        }

        public ulong EncounterId { get; }
        public long ExpirationTimestampMs { get; }

        public Position2DViewModel Position { get; }
        public string SpawnPointId { get; }

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