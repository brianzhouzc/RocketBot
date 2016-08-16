using GalaSoft.MvvmLight.Command;
using POGOProtos.Map.Fort;
using POGOProtos.Networking.Responses;
using PokemonGo.Bot.Enums;
using PokemonGo.Bot.Messages;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Extensions;
using System;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Threading;

namespace PokemonGo.Bot.ViewModels
{
    public class PokestopViewModel : FortViewModel
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

        public AsyncRelayCommand Search { get; }

        DispatcherTimer isActiveTimer;
        long cooldownCompleteTimestampMs;
        readonly PlayerViewModel player;

        public PokestopViewModel(FortData fort, Client client, PlayerViewModel player)
            : base(fort, client)
        {
            this.player = player;
            InitializeTimer(fort.CooldownCompleteTimestampMs);
            CalculateIsNear();
            player.PropertyChanged += Player_PropertyChanged;

            Search = new AsyncRelayCommand(async () =>
            {
                var searchResult = await client.Fort.SearchFort(Id, Position.Latitude, Position.Longitude);
                InitializeTimer(searchResult.CooldownCompleteTimestampMs);
                if (searchResult.Result == FortSearchResponse.Types.Result.Success ||
                    searchResult.Result == FortSearchResponse.Types.Result.InventoryFull)
                {
                    var sb = new StringBuilder("Pokestop successfully farmed.");
                    sb.AppendFormat(" {0}Xp", searchResult.ExperienceAwarded);
                    player.Xp += searchResult.ExperienceAwarded;
                    foreach (var item in searchResult.ItemsAwarded)
                    {
                        var itemInInventory = player.Inventory.Items.SingleOrDefault(i => (int)i.ItemType == (int)item.ItemId);
                        if (itemInInventory == null)
                        {
                            itemInInventory = new ItemViewModel(item);
                            player.Inventory.Items.Add(itemInInventory);
                        }
                        else
                        {
                            itemInInventory.Count += item.ItemCount;
                        }
                        sb.AppendFormat(" - {0}x {1}", item.ItemCount, Enum.GetName(typeof(ItemType), itemInInventory.ItemType));
                    }

                    if (searchResult.PokemonDataEgg != null)
                    {
                        sb.AppendFormat("- 1x Egg ({0}km)", searchResult.PokemonDataEgg.EggKmWalkedTarget);
                        player.Inventory.Eggs.Add(new EggViewModel(searchResult.PokemonDataEgg));
                    }
                    MessengerInstance.Send(new Message(Colors.Green, sb.ToString()));
                }
                else
                {
                    MessengerInstance.Send(new Message(Colors.Red, $"Failed to farm Pokestop. Result is {Enum.GetName(typeof(FortSearchResponse.Types.Result), searchResult.Result)}."));
                }
            });
        }

        ~PokestopViewModel()
        {
            player.PropertyChanged -= Player_PropertyChanged;
        }

        void Player_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(player.Position))
            {
                CalculateIsNear();
            }
        }

        void CalculateIsNear()
        {
            IsNear = player.Position.DistanceTo(Position) <= 10;
        }

        void InitializeTimer(long cooldownComplete)
        {
            cooldownCompleteTimestampMs = cooldownComplete;

            if (cooldownCompleteTimestampMs > DateTime.UtcNow.ToUnixTime())
            {
                IsActive = false;
                if (isActiveTimer == null)
                {
                    isActiveTimer = new DispatcherTimer();
                    isActiveTimer.Tick += IsActiveTimer_Tick;
                    isActiveTimer.Interval = TimeSpan.FromSeconds(1);
                }
                if (!isActiveTimer.IsEnabled)
                    isActiveTimer.Start();
            }
            else
            {
                IsActive = true;
            }
        }

        void IsActiveTimer_Tick(object sender, System.EventArgs e)
        {
            if (cooldownCompleteTimestampMs <= DateTime.UtcNow.ToUnixTime())
            {
                isActiveTimer.Tick -= IsActiveTimer_Tick;
                IsActive = true;
            }
        }
    }
}