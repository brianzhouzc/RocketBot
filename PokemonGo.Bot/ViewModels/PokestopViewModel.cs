using GalaSoft.MvvmLight.Command;
using POGOProtos.Inventory.Item;
using POGOProtos.Map.Fort;
using POGOProtos.Networking.Responses;
using PokemonGo.Bot.Messages;
using PokemonGo.Bot.MVVMLightUtils;
using PokemonGo.Bot.Utils;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace PokemonGo.Bot.ViewModels
{
    public class PokestopViewModel : FortViewModel, IUpdateable<PokestopViewModel>, IDisposable
    {
        bool isActive;

        public bool IsActive
        {
            get { return isActive; }
            set { if (IsActive != value) { isActive = value; RaisePropertyChanged(); } }
        }

        bool isNear;

        public bool IsNear
        {
            get { return isNear; }
            set { if (IsNear != value) { isNear = value; RaisePropertyChanged(); } }
        }

        bool hasLureModuleActive;
        public bool HasLureModuleActive
        {
            get { return hasLureModuleActive; }
            set { if (HasLureModuleActive != value) { hasLureModuleActive = value; RaisePropertyChanged(); } }
        }


        #region Search

        AsyncRelayCommand search;

        public AsyncRelayCommand Search
        {
            get
            {
                if (search == null)
                    search = new AsyncRelayCommand(ExecuteSearch, CanExecuteSearch);

                return search;
            }
        }

        async Task ExecuteSearch()
        {
            var searchResult = await session.SearchFort(Id, Position.Latitude, Position.Longitude);
            InitializeIsActiveTimer(searchResult.CooldownCompleteTimestampMs);
            if (searchResult.Result == FortSearchResponse.Types.Result.Success ||
                searchResult.Result == FortSearchResponse.Types.Result.InventoryFull)
            {
                var sb = new StringBuilder("Pokestop successfully farmed.");
                sb.AppendFormat(" {0}Xp", searchResult.ExperienceAwarded);
                player.Xp += searchResult.ExperienceAwarded;
                foreach (var itemGroup in searchResult.ItemsAwarded.GroupBy(i => i.ItemId))
                {
                    // since 3 pokeballs are returned as 3x one pokeball, we group here to get a nicer display.
                    var item = new ItemAward
                    {
                        ItemCount = itemGroup.Sum(i => i.ItemCount),
                        ItemId = itemGroup.Key
                    };
                    var itemInInventory = player.Inventory.Items.SingleOrDefault(i => (int)i.ItemType == (int)item.ItemId);
                    if (itemInInventory == null)
                    {
                        itemInInventory = new ItemViewModel(item, session);
                        player.Inventory.Items.Add(itemInInventory);
                    }
                    else
                    {
                        itemInInventory.Count += item.ItemCount;
                    }
                    sb.AppendFormat(" - {0}x {1}", item.ItemCount, Enum.GetName(typeof(Enums.ItemType), itemInInventory.ItemType));
                }

                if (searchResult.PokemonDataEgg != null)
                {
                    sb.AppendFormat("- 1x Egg ({0}km)", searchResult.PokemonDataEgg.EggKmWalkedTarget);
                    player.Inventory.Eggs.Add(new EggViewModel(searchResult.PokemonDataEgg, player.Inventory.EggIncubators, player.KmWalked));
                }
                MessengerInstance.Send(new Message(Colors.Green, sb.ToString()));
            }
            else
            {
                MessengerInstance.Send(new Message(Colors.Red, $"Failed to farm Pokestop. Result is {Enum.GetName(typeof(FortSearchResponse.Types.Result), searchResult.Result)}."));
            }
        }

        bool CanExecuteSearch() => IsNear;

        #endregion Search


        DispatcherTimer isActiveTimer;
        PlayerViewModel player;
        DateTimeOffset cooldownCompletedUtc;
        readonly Settings settings;

        public PokestopViewModel(FortData fort, SessionViewModel session, PlayerViewModel player, Settings settings)
            : base(fort, session)
        {
            this.settings = settings;
            this.player = player;
            HasLureModuleActive = !fort.ActiveFortModifier.IsEmpty;
            InitializeIsActiveTimer(fort.CooldownCompleteTimestampMs);
            CalculateIsNear();
            //player.PropertyChanged += Player_PropertyChanged;
        }

        //void Player_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == nameof(player.Position))
        //    {
        //        CalculateIsNear();
        //    }
        //}

        public void CalculateIsNear()
        {
            IsNear = player.Position.IsInRange(Position, settings.Fort.InteractionRangeMeters);
        }

        void InitializeIsActiveTimer(long cooldownCompleteTimestampMs)
        {
            cooldownCompletedUtc = DateTimeOffset.FromUnixTimeMilliseconds(cooldownCompleteTimestampMs);
            InitializeIsActiveTimer();
        }

        void InitializeIsActiveTimer()
        {
            var completedIn = (cooldownCompletedUtc - DateTime.UtcNow);

            if (completedIn.Ticks > 0)
            {
                IsActive = false;
                isActiveTimer = new DispatcherTimer();
                isActiveTimer.Tick += IsActiveTimer_Tick;
                isActiveTimer.Interval = completedIn;
                isActiveTimer.Start();
            }
            else
            {
                IsActive = true;
            }
        }

        void IsActiveTimer_Tick(object sender, System.EventArgs e)
        {
            var timer = isActiveTimer;
            if (isActiveTimer != null)
            {
                isActiveTimer.Stop();
                isActiveTimer.Tick -= IsActiveTimer_Tick;
                isActiveTimer = null;
            }
            IsActive = true;
        }

        public void UpdateWith(PokestopViewModel other)
        {
            if (!Equals(other))
                throw new ArgumentException($"Expected a pokestop with Id {Id} but got {Id}.", nameof(other));

            IsActive = other.IsActive;
            IsNear = other.IsNear;
            HasLureModuleActive = other.HasLureModuleActive;
            cooldownCompletedUtc = other.cooldownCompletedUtc;
            InitializeIsActiveTimer();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    var timer = isActiveTimer;
                    if (isActiveTimer != null)
                    {
                        isActiveTimer.Stop();
                        isActiveTimer.Tick -= IsActiveTimer_Tick;
                        isActiveTimer = null;
                    }
                    //player.PropertyChanged -= Player_PropertyChanged;
                    player = null;
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}