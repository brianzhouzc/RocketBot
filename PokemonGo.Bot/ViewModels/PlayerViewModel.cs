using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.GeneratedCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.Bot.ViewModels
{
    public class PlayerViewModel : ViewModelBase
    {
        private readonly Client client;

        public AsyncRelayCommand Login { get; }
        public AsyncRelayCommand LoadProfile { get; }

        public InventoryViewModel Inventory { get; }
        private GetPlayerResponse profile;

        public GetPlayerResponse Profile
        {
            get
            {
                return profile;
            }
            set
            {
                if (Profile != value)
                {
                    profile = value;
                    RaisePropertyChanged();
                }
            }
        }

        private DownloadSettingsResponse settings;

        public DownloadSettingsResponse Settings
        {
            get
            {
                return settings;
            }
            set
            {
                if (Settings != value)
                {
                    settings = value;
                    RaisePropertyChanged();
                }
            }
        }

        private GetMapObjectsResponse mapObjects;

        public GetMapObjectsResponse MapObjects
        {
            get
            {
                return mapObjects;
            }
            set
            {
                if (MapObjects != value)
                {
                    mapObjects = value;
                    RaisePropertyChanged();
                }
            }
        }

        private PositionViewModel position;

        public PositionViewModel Position
        {
            get
            {
                return position;
            }
            set
            {
                if (Position != value)
                {
                    position = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool isLoggedIn;

        public bool IsLoggedIn
        {
            get
            {
                return isLoggedIn;
            }
            set
            {
                if (IsLoggedIn != value)
                {
                    isLoggedIn = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double speedInmetersPerMillisecond;

        AsyncRelayCommand Move { get; }

        public async Task MoveTo(double lat, double lon)
        {
            var newPosition = new PositionViewModel(lat, lon);
            var waitTime = Position.DistanceTo(newPosition) / speedInmetersPerMillisecond;
            await Task.Delay((int)Math.Ceiling(waitTime));
            await client.UpdatePlayerLocation(lat, lon);
            Position = newPosition;
        }

        public PlayerViewModel(Client client, InventoryViewModel inventory, ISettings settings)
        {
            this.client = client;
            Inventory = inventory;
            speedInmetersPerMillisecond = settings.TravelSpeed / 3600;

            Position = new PositionViewModel(client.CurrentLatitude, client.CurrentLongitude);

            Login = new AsyncRelayCommand(async () =>
            {
                // We do not use Task.WhenAll here because this is the order in which the android app executes these requests.
                await client.Login();
                await client.SetServer();
                Profile = await client.GetProfile();
                Settings = await client.GetSettings();
                MapObjects = await client.GetMapObjects();
                await Inventory.Load.ExecuteAsync();
                IsLoggedIn = true;

            });
            LoadProfile = new AsyncRelayCommand(async () => Profile = await client.GetProfile());
        }
    }
}
