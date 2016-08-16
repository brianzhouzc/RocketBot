using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using POGOProtos.Enums;
using PokemonGo.Bot.Messages;
using PokemonGo.Bot.Utils;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace PokemonGo.Bot.ViewModels
{
    public class PlayerViewModel : ViewModelBase
    {
        readonly MapViewModel map;
        readonly Client client;

        public AsyncRelayCommand Login { get; }
        public AsyncRelayCommand LoadProfile { get; }

        public InventoryViewModel Inventory { get; }

        long xp;

        public long Xp
        {
            get { return xp; }
            set { if (Xp != value) { xp = value; RaisePropertyChanged(); } }
        }

        long nextLevelXP;

        public long NextLevelXP
        {
            get { return nextLevelXP; }
            set { if (NextLevelXP != value) { nextLevelXP = value; RaisePropertyChanged(); } }
        }

        long prevLevelXP;

        public long PrevLevelXp
        {
            get { return prevLevelXP; }
            set { if (PrevLevelXp != value) { prevLevelXP = value; RaisePropertyChanged(); } }
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

        Position3DViewModel position;

        public Position3DViewModel Position
        {
            get { return position; }
            set { if (Position != value) { position = value; RaisePropertyChanged(); } }
        }

        bool isLoggedIn;

        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
            set { if (IsLoggedIn != value) { isLoggedIn = value; RaisePropertyChanged(); } }
        }

        double xpPerHour;
        public double XpPerHour
        {
            get { return xpPerHour; }
            set { if (XpPerHour != value) { xpPerHour = value; RaisePropertyChanged(); } }
        }

        readonly double minSpeedInmetersPerMillisecond;
        readonly double maxSpeedInmetersPerMillisecond;

        public AsyncRelayCommand<Position2DViewModel> Move { get; }

        Task MoveToAsync(Position2DViewModel newPosition)
            => MoveToAsync(newPosition.To3D(Position.Altitute));

        Queue<KeyValuePair<DateTime, long>> xPValuesInLastHours = new Queue<KeyValuePair<DateTime, long>>();

        Random random = new Random(DateTime.Now.Millisecond);
        readonly Settings settings;

        double GetRandomTravelSpeed() => random.NextDouble() * (maxSpeedInmetersPerMillisecond - minSpeedInmetersPerMillisecond) + minSpeedInmetersPerMillisecond;
        private async Task MoveToAsync(Position3DViewModel newPosition)
        {
            var waitTime = Position.DistanceTo(newPosition) / GetRandomTravelSpeed();
            var startTime = DateTime.Now;
            var targetTime = startTime.AddMilliseconds(waitTime);
            var targetVector = newPosition - Position;
            var startPosition = Position;
            var lastUpdateTime = startTime;
            var now = startTime;
            while (now < targetTime)
            {
                var msTraveled = (now - startTime).TotalMilliseconds;
                var currentPosition = startPosition + (targetVector * (msTraveled / waitTime));
                Position = currentPosition;
                await Task.WhenAll(map.SetPosition.ExecuteAsync(position), Task.Delay(100));

                now = DateTime.Now;
            }
            Position = newPosition;
        }

        public PlayerViewModel(Client client, InventoryViewModel inventory, MapViewModel map, Settings settings)
        {
            this.client = client;
            this.map = map;
            this.settings = settings;
            Inventory = inventory;
            minSpeedInmetersPerMillisecond = settings.MinTravelSpeedInKmH / 3600.0;
            maxSpeedInmetersPerMillisecond = settings.MaxTravelSpeedInKmH / 3600.0;

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
                await settings.DownloadSettingsAsync();
                await settings.DownloadItemTemplatesAsync();
                IsLoggedIn = true;
            });
            Move = new AsyncRelayCommand<Position2DViewModel>(MoveToAsync);

            var xpTimer = new DispatcherTimer();
            xpTimer.Tick += XpTimer_Tick;
            xpTimer.Interval = TimeSpan.FromSeconds(10);
            xpTimer.Start();
        }

        private void XpTimer_Tick(object sender, EventArgs e)
        {
            var currentXp = Xp;
            if (currentXp != 0)
            {
                var now = DateTime.Now;
                var oneHourBefore = now.AddHours(-1);

                xPValuesInLastHours.Enqueue(new KeyValuePair<DateTime, long>(now, currentXp));

                var current = xPValuesInLastHours.Peek();
                while (current.Key <= oneHourBefore)
                {
                    xPValuesInLastHours.Dequeue();
                    current = xPValuesInLastHours.Peek();
                }

                var timeDiff = now - current.Key;
                var xpDiff = currentXp - current.Value;

                XpPerHour = xpDiff / timeDiff.TotalHours;
            }
        }
    }
}