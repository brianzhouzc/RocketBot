using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using POGOProtos.Data;
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

        float kmWalked;
        public float KmWalked
        {
            get { return kmWalked; }
            set { if (KmWalked != value) { kmWalked = value; RaisePropertyChanged(); } }
        }

        Position3DViewModel position;

        public Position3DViewModel Position
        {
            get { return position; }
            set
            {
                if (Position != value)
                {
                    position = value;
                    session.SetPosition(position);
                    RaisePropertyChanged();
                }
            }
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
        readonly SessionViewModel session;

        double GetRandomTravelSpeed() => random.NextDouble() * (maxSpeedInmetersPerMillisecond - minSpeedInmetersPerMillisecond) + minSpeedInmetersPerMillisecond;
        async Task MoveToAsync(Position3DViewModel newPosition)
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
                await Task.Delay(100);

                now = DateTime.Now;
            }
            Position = newPosition;
        }

        public PlayerViewModel(InventoryViewModel inventory, MapViewModel map, Settings settings, SessionViewModel session)
        {
            this.map = map;
            this.settings = settings;
            this.session = session;
            Inventory = inventory;
            minSpeedInmetersPerMillisecond = settings.MinTravelSpeedInKmH / 3600.0;
            maxSpeedInmetersPerMillisecond = settings.MaxTravelSpeedInKmH / 3600.0;

            Position = new Position3DViewModel(settings.DefaultLatitude, settings.DefaultLongitude, settings.DefaultAltitude);


            Move = new AsyncRelayCommand<Position2DViewModel>(MoveToAsync);

            var xpTimer = new DispatcherTimer();
            xpTimer.Tick += XpTimer_Tick;
            xpTimer.Interval = TimeSpan.FromSeconds(10);
            xpTimer.Start();
        }

        internal void UpdateWith(PlayerData profile)
        {
            Username = profile.Username;
            Team = Enum.GetName(typeof(TeamColor), profile.Team);
            Stardust = profile.Currencies.Where(c => c.Name == "STARDUST").Sum(c => c.Amount);
            Pokecoins = profile.Currencies.Where(c => c.Name == "POKECOIN").Sum(c => c.Amount);
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