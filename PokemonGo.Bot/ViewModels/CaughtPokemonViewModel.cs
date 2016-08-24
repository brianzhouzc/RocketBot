using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POGOProtos.Data;
using PokemonGo.RocketAPI;
using GalaSoft.MvvmLight.Command;
using PokemonGo.Bot.Messages;
using POGOProtos.Networking.Responses;
using System.Windows.Media;
using PokemonGo.Bot.MVVMLightUtils;

namespace PokemonGo.Bot.ViewModels
{
    public class CaughtPokemonViewModel : PokemonDataViewModel, IUpdateable<CaughtPokemonViewModel>
    {
        readonly SessionViewModel session;

        public AsyncRelayCommand Transfer { get; }
        public AsyncRelayCommand ToggleFavorite { get; }
        public AsyncRelayCommand Evolve { get; }
        public AsyncRelayCommand Upgrade { get; }

        int candy;
        public int Candy
        {
            get { return candy; }
            set { if (Candy != value) { candy = value; RaisePropertyChanged(); } }
        }

        public CaughtPokemonViewModel(PokemonData pokemon, SessionViewModel session, InventoryViewModel inventory) : base(pokemon)
        {
            if (Id == 0)
                throw new ArgumentException("This is not a caught pokemon.", nameof(pokemon));

            this.session = session;

            Candy = inventory.GetCandyForFamily(FamilyId);

            Transfer = new AsyncRelayCommand(async () =>
            {
                var response = await session.TransferPokemon(Id);
                if(response.Result == ReleasePokemonResponse.Types.Result.Success)
                {
                    inventory.Pokemon.Remove(this);
                    inventory.AddCandyForFamily(response.CandyAwarded, FamilyId);
                    MessengerInstance.Send(new Message(Colors.Green, $"Transferred {Name} ({CombatPoints} CP, {PerfectPercentage:P2} IV). Got {response.CandyAwarded} Candy."));
                }
                else
                {
                    MessengerInstance.Send(new Message(Colors.Red, $"Error transferring {Name}: {Enum.GetName(typeof(ReleasePokemonResponse.Types.Result), response.Result)}"));
                }
            });
            ToggleFavorite = new AsyncRelayCommand(async () =>
            {
                var targetValue = !IsFavorite;
                var response = await session.SetFavoritePokemon(Id, targetValue);
                if (response.Result == SetFavoritePokemonResponse.Types.Result.Success)
                    IsFavorite = targetValue;
            });
            Evolve = new AsyncRelayCommand(async () =>
            {
                var response = await session.EvolvePokemon(Id);
                if(response.Result == EvolvePokemonResponse.Types.Result.Success)
                {
                    inventory.AddCandyForFamily(Candy, FamilyId);
                    inventory.Player.Xp += response.ExperienceAwarded;
                    CombatPoints = response.EvolvedPokemonData.Cp;
                    PokemonId = (int)response.EvolvedPokemonData.PokemonId;
                }
            });
            Upgrade = new AsyncRelayCommand(async () =>
            {
                var response = await session.UpgradePokemon(Id);
                if(response.Result == UpgradePokemonResponse.Types.Result.Success)
                {
                    CombatPoints = response.UpgradedPokemon.Cp;
                }
            });
        }

        public void UpdateWith(CaughtPokemonViewModel other)
        {
            if (!Equals(other))
                throw new ArgumentException($"Expected a {Name} with Id {Id} but got a {other?.Name} with Id {other?.Id}", nameof(other));

            Candy = other.Candy;
            base.UpdateWith(other);
        }
    }
}
