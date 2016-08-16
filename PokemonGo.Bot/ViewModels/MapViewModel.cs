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

namespace PokemonGo.Bot.ViewModels
{
    public class MapViewModel : ViewModelBase
    {
        readonly Client client;

        public AsyncRelayCommand GetMapObjects { get; }
        public AsyncRelayCommand<Position3DViewModel> SetPosition { get; }

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
        readonly DispatcherTimer timer;
        readonly Settings settings;
        DateTime lastMapObjectsUpdateTime;
        Position3DViewModel lastMapObjectUpdatePosition;

        PlayerViewModel player;

        public PlayerViewModel Player
        {
            get { return player; }
            set { if (Player != value) { player = value; RaisePropertyChanged(); } }
        }

        public MapViewModel(Client client, Settings settings)
        {
            this.client = client;
            this.settings = settings;

            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();

            GetMapObjects = new AsyncRelayCommand(async () =>
            {
                MessengerInstance.Send(new Messages.Message("GetMapObjects"));
                var mapResponse = await client.Map.GetMapObjects();
                var pokestopsFromResponse = mapResponse.Item1.MapCells.SelectMany(m => m.Forts).Where(f => f.Type == FortType.Checkpoint).Select(f => new PokestopViewModel(f, client, Player));
                if(pokestopsFromResponse.Any())
                    Pokestops.UpdateWith(pokestopsFromResponse);

                var gymsFromResponse = mapResponse.Item1.MapCells.SelectMany(m => m.Forts).Where(f => f.Type == FortType.Gym).Select(f => new GymViewModel(f, client));
                if(gymsFromResponse.Any())
                    Gyms.UpdateWith(gymsFromResponse);

                var wildPokemonFromResponse = mapResponse.Item1.MapCells.SelectMany(m => m.WildPokemons).Select(p => new WildPokemonViewModel(p));
                if(wildPokemonFromResponse.Any())
                    WildPokemon.UpdateWith(wildPokemonFromResponse);

                var catchablePokemonFromResponse = mapResponse.Item1.MapCells.SelectMany(m => m.CatchablePokemons).Select(p => new MapPokemonViewModel(p, client, settings, Player, this));
                if(catchablePokemonFromResponse.Any())
                    CatchablePokemon.UpdateWith(catchablePokemonFromResponse);
            });

            SetPosition = new AsyncRelayCommand<Position3DViewModel>(async pos =>
            {
                //await client.Player.UpdatePlayerLocation(pos.Latitude, pos.Longitude, pos.Altitute);
                //await GetMapObjects.ExecuteAsync();
                client.Player.SetCoordinates(pos.Latitude, pos.Longitude, pos.Altitute);
                await TryUpdateMap();
            });

            SetLastPosition = new RelayCommand<Position3DViewModel>(pos => LastClickedPosition = pos);
        }

        async void Timer_Tick(object sender, EventArgs e)
        {
            await TryUpdateMap();
        }

        async Task TryUpdateMap()
        {
            if (Player != null && settings?.Map != null)
            {
                var now = DateTime.Now;
                var position = Player.Position;
                if (lastMapObjectUpdatePosition == null)
                    lastMapObjectUpdatePosition = position;

                // update when player has moved or when max update time has passed,
                // but never before min update time has passed.
                if (now >= lastMapObjectsUpdateTime.AddSeconds(settings.Map.GetMapObjectsMinRefreshSeconds) &&
                    (now.AddSeconds(1) >= lastMapObjectsUpdateTime.AddSeconds(settings.Map.GetMapObjectsMaxRefreshSeconds) ||
                    position.DistanceTo(lastMapObjectUpdatePosition) >= settings.Map.GetMapObjectsMinDistanceMeters))
                {
                    lastMapObjectsUpdateTime = now;
                    lastMapObjectUpdatePosition = position;
                    await GetMapObjects.ExecuteAsync();
                }
            }
        }
    }
}
