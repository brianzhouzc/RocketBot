using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PokemonGo.Bot.Messages;
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
        readonly MapViewModel map;
        readonly Client client;

        public AsyncRelayCommand Login { get; }
        public AsyncRelayCommand LoadProfile { get; }

        public InventoryViewModel Inventory { get; }
        Profile profile;

        public Profile Profile
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

        GlobalSettings settings;

        public GlobalSettings Settings
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

        PositionViewModel position;

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

        bool isLoggedIn;

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

        readonly double speedInmetersPerMillisecond;

        public AsyncRelayCommand<PositionViewModel> Move { get; }

        async Task MoveToAsync(PositionViewModel newPosition)
        {
            var waitTime = Position.DistanceTo(newPosition) / speedInmetersPerMillisecond;
            await Task.Delay((int)Math.Ceiling(waitTime));
            await map.SetPosition.ExecuteAsync(newPosition);
            Position = newPosition;
        }



        public PlayerViewModel(Client client, InventoryViewModel inventory, MapViewModel map, ISettings settings)
        {
            this.client = client;
            this.map = map;
            Inventory = inventory;
            speedInmetersPerMillisecond = settings.TravelSpeed / 3600.0;

            Position = new PositionViewModel(client.CurrentLatitude, client.CurrentLongitude);
            LoadProfile = new AsyncRelayCommand(async () => Profile = (await client.GetProfile()).Profile);

            Login = new AsyncRelayCommand(async () =>
            {
                // We do not use Task.WhenAll here because this is the order in which the android app executes these requests.
                MessengerInstance.Send<Message>(new Message("Login"));
                await client.Login();
                MessengerInstance.Send<Message>(new Message("SetServer"));
                await client.SetServer();
                MessengerInstance.Send<Message>(new Message("LoadProfile"));
                await LoadProfile.ExecuteAsync();
                MessengerInstance.Send<Message>(new Message("GetSettings"));
                Settings = (await client.GetSettings()).Settings;
                MessengerInstance.Send<Message>(new Message("GetMapObjects"));
                await map.GetMapObjects.ExecuteAsync();
                MessengerInstance.Send<Message>(new Message("LoadInventory"));
                await Inventory.Load.ExecuteAsync();
                IsLoggedIn = true;

            });
            Move = new AsyncRelayCommand<PositionViewModel>(MoveToAsync);
        }
    }
}
