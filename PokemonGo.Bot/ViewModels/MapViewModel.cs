using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using POGOProtos.Map.Fort;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Bot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using PokemonGo.Bot.Utils;
using System.Windows.Threading;
using System.Windows.Media;
using POGOProtos.Map;

namespace PokemonGo.Bot.ViewModels
{
    public class MapViewModel : ViewModelBase
    {
        public ObservableCollection<PokestopViewModel> Pokestops { get; } = new ObservableCollection<PokestopViewModel>();
        public ObservableCollection<GymViewModel> Gyms { get; } = new ObservableCollection<GymViewModel>();
        public ObservableCollection<WildPokemonViewModel> WildPokemon { get; } = new ObservableCollection<WildPokemonViewModel>();
        public ObservableCollection<MapPokemonViewModel> CatchablePokemon { get; } = new ObservableCollection<MapPokemonViewModel>();

        Position3DViewModel lastClickedPosition;
        public Position3DViewModel LastClickedPosition
        {
            get { return lastClickedPosition; }
            set { if (LastClickedPosition != value) { lastClickedPosition = value; RaisePropertyChanged(); } }
        }

        public RelayCommand<Position3DViewModel> SetLastPosition { get; }
        readonly Settings settings;

        PlayerViewModel player;
        readonly SessionViewModel session;

        public PlayerViewModel Player
        {
            get { return player; }
            set { if (Player != value) { player = value; RaisePropertyChanged(); } }
        }

        public MapViewModel(SessionViewModel session, Settings settings)
        {
            this.session = session;
            this.settings = settings;


            SetLastPosition = new RelayCommand<Position3DViewModel>(pos => LastClickedPosition = pos);
        }

        internal void UpdateWith(IEnumerable<MapCell> mapCells)
        {
            var pokestopsFromResponse = mapCells.SelectMany(m => m.Forts).Where(f => f.Type == FortType.Checkpoint).Select(f => new PokestopViewModel(f, session, Player, settings)).ToList();
            if (pokestopsFromResponse.Any())
                Pokestops.UpdateWith(pokestopsFromResponse);

            var gymsFromResponse = mapCells.SelectMany(m => m.Forts).Where(f => f.Type == FortType.Gym).Select(f => new GymViewModel(f, session)).ToList();
            if (gymsFromResponse.Any())
                Gyms.UpdateWith(gymsFromResponse);

            var wildPokemonFromResponse = mapCells.SelectMany(m => m.WildPokemons).Select(p => new WildPokemonViewModel(p, settings)).ToList();
            if (wildPokemonFromResponse.Any())
                WildPokemon.UpdateWith(wildPokemonFromResponse);

            var catchablePokemonFromLure = mapCells.SelectMany(m => m.Forts).Where(f => f.LureInfo != null).Select(f => new MapPokemonViewModel(f, session, settings, player, this));
            var catchablePokemonFromResponse = mapCells.SelectMany(m => m.CatchablePokemons).Select(p => new MapPokemonViewModel(p, session, settings, Player, this));
            var catchablePokemon = catchablePokemonFromLure.Concat(catchablePokemonFromResponse).ToList();
            if (catchablePokemon.Any())
                CatchablePokemon.UpdateWith(catchablePokemon);
        }
    }
}
