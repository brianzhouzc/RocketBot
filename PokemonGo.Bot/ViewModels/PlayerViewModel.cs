using AllEnum;
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

        int xp;
        public int Xp
        {
            get { return xp; }
            set { if (Xp != value) { xp = value; RaisePropertyChanged(); } }
        }

        int nextLevelXP;
        public int NextLevelXP
        {
            get { return nextLevelXP; }
            set { if (NextLevelXP != value) { nextLevelXP = value; RaisePropertyChanged(); } }
        }

        int level;
        public int Level
        {
            get { return level; }
            set { if (Level != value) { level = value; RaisePropertyChanged(); } }
        }

        int stardust;
        public int Stardust
        {
            get { return stardust; }
            set { if (Stardust != value) { stardust = value; RaisePropertyChanged(); } }
        }

        int pokecoins;
        public int Pokecoins
        {
            get { return pokecoins; }
            set { if (Pokecoins != value) { pokecoins = value; RaisePropertyChanged(); } }
        }

        string username;
        public string Username
        {
            get { return username; }
            set { if (Username != value) { username = value; RaisePropertyChanged(); } }
        }

        string team;
        public string Team
        {
            get { return team; }
            set { if (Team != value) { team = value; RaisePropertyChanged(); } }
        }



        GlobalSettings settings;

        public GlobalSettings Settings
        {
            get { return settings; }
            set { if (Settings != value) { settings = value; RaisePropertyChanged(); } }
        }

        PositionViewModel position;

        public PositionViewModel Position
        {
            get { return position; }
            set { if (Position != value) { position = value; RaisePropertyChanged(); } }
        }

        bool isLoggedIn;

        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
            set { if (IsLoggedIn != value) { isLoggedIn = value; RaisePropertyChanged(); }    }
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
            LoadProfile = new AsyncRelayCommand(async () =>
            {
                var profile = (await client.GetProfile()).Profile;
                Username = profile.Username;
                Team = Enum.GetName(typeof(TeamColor), profile.Team);
                Stardust = profile.Currency.Where(c => c.Type == "STARDUST").Sum(c => c.Amount);
                Pokecoins = profile.Currency.Where(c => c.Type == "POKECOIN").Sum(c => c.Amount);
            });

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
                //MessengerInstance.Send<Message>(new Message("GetMapObjects"));
                //await map.GetMapObjects.ExecuteAsync();
                MessengerInstance.Send<Message>(new Message("LoadInventory"));
                await Inventory.Load.ExecuteAsync();
                IsLoggedIn = true;

            });
            Move = new AsyncRelayCommand<PositionViewModel>(MoveToAsync);
        }
    }
}
