using GalaSoft.MvvmLight.Command;
using POGOProtos.Map.Fort;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Extensions;
using System;
using System.Linq;
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

        public AsyncRelayCommand Search { get; }

        DispatcherTimer isActiveTimer;
        long cooldownCompleteTimestampMs;
        readonly PlayerViewModel player;

        public PokestopViewModel(FortData fort, Client client, PlayerViewModel player)
            : base(fort, client)
        {
            this.player = player;
            InitializeTimer(fort.CooldownCompleteTimestampMs);

            Search = new AsyncRelayCommand(async () =>
            {
                var searchResult = await client.Fort.SearchFort(Id, Position.Latitude, Position.Longitude);
                InitializeTimer(searchResult.CooldownCompleteTimestampMs);
                if (searchResult.Result == POGOProtos.Networking.Responses.FortSearchResponse.Types.Result.Success)
                {
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
                    }

                    if (searchResult.PokemonDataEgg != null)
                    {
                        player.Inventory.Eggs.Add(new EggViewModel(searchResult.PokemonDataEgg));
                    }
                }
            });
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
            if (cooldownCompleteTimestampMs >= DateTime.UtcNow.ToUnixTime())
            {
                isActiveTimer.Tick -= IsActiveTimer_Tick;
                IsActive = true;
            }
        }
    }
}