using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using POGOProtos.Enums;
using PokemonGo.Bot.Messages;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Bot;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonGo.Bot.ViewModels
{
    public class PlayerViewModel : ViewModelBase
    {
        private readonly MapViewModel map;
        private readonly Client client;

        public AsyncRelayCommand Login { get; }
        public AsyncRelayCommand LoadProfile { get; }

        public InventoryViewModel Inventory { get; }

        private int xp;

        public int Xp
        {
            get { return xp; }
            set { if (Xp != value) { xp = value; RaisePropertyChanged(); } }
        }

        private int nextLevelXP;

        public int NextLevelXP
        {
            get { return nextLevelXP; }
            set { if (NextLevelXP != value) { nextLevelXP = value; RaisePropertyChanged(); } }
        }

        private int level;

        public int Level
        {
            get { return level; }
            set { if (Level != value) { level = value; RaisePropertyChanged(); } }
        }

        private int stardust;

        public int Stardust
        {
            get { return stardust; }
            set { if (Stardust != value) { stardust = value; RaisePropertyChanged(); } }
        }

        private int pokecoins;

        public int Pokecoins
        {
            get { return pokecoins; }
            set { if (Pokecoins != value) { pokecoins = value; RaisePropertyChanged(); } }
        }

        private string username;

        public string Username
        {
            get { return username; }
            set { if (Username != value) { username = value; RaisePropertyChanged(); } }
        }

        private string team;

        public string Team
        {
            get { return team; }
            set { if (Team != value) { team = value; RaisePropertyChanged(); } }
        }

        private Position3DViewModel position;

        public Position3DViewModel Position
        {
            get { return position; }
            set { if (Position != value) { position = value; RaisePropertyChanged(); } }
        }

        private bool isLoggedIn;

        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
            set { if (IsLoggedIn != value) { isLoggedIn = value; RaisePropertyChanged(); } }
        }

        private readonly double speedInmetersPerMillisecond;

        public AsyncRelayCommand<Position2DViewModel> Move { get; }

        private Task MoveToAsync(Position2DViewModel newPosition)
            => MoveToAsync(newPosition.To3D(Position.Altitute));

        private async Task MoveToAsync(Position3DViewModel newPosition)
        {
            var waitTime = Position.DistanceTo(newPosition) / speedInmetersPerMillisecond;
            await Task.Delay((int)Math.Ceiling(waitTime));
            await map.SetPosition.ExecuteAsync(newPosition);
            Position = newPosition;
        }

        public PlayerViewModel(Client client, InventoryViewModel inventory, MapViewModel map, Settings settings)
        {
            this.client = client;
            this.map = map;
            Inventory = inventory;
            speedInmetersPerMillisecond = settings.TravelSpeed / 3600.0;

            Position = new Position3DViewModel(client.CurrentLatitude, client.CurrentLongitude, client.CurrentAltitude);
            LoadProfile = new AsyncRelayCommand(async () =>
            {
                var profile = (await client.Player.GetPlayer()).PlayerData;
                Username = profile.Username;
                Team = Enum.GetName(typeof(TeamColor), profile.Team);
                Stardust = profile.Currencies.Where(c => c.Name == "STARDUST").Sum(c => c.Amount);
                Pokecoins = profile.Currencies.Where(c => c.Name == "POKECOIN").Sum(c => c.Amount);
            });

            Login = new AsyncRelayCommand(async () =>
            {
                // We do not use Task.WhenAll here because this is the order in which the android app executes these requests.
                MessengerInstance.Send<Message>(new Message("Login"));
                await client.Login.DoLogin();
                MessengerInstance.Send<Message>(new Message("LoadProfile"));
                await LoadProfile.ExecuteAsync();
                MessengerInstance.Send<Message>(new Message("GetMapObjects"));
                await map.GetMapObjects.ExecuteAsync();
                MessengerInstance.Send<Message>(new Message("LoadInventory"));
                await Inventory.Load.ExecuteAsync();
                IsLoggedIn = true;
            });
            Move = new AsyncRelayCommand<Position2DViewModel>(MoveToAsync);
        }
    }
}