using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using POGOProtos.Map.Fort;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Extensions;
using System;
using System.Threading;
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

        readonly DispatcherTimer isActiveTimer;
        readonly long lastModifiedTimestampMs;
        private long cooldownCompleteTimestampMs;

        public PokestopViewModel(FortData fort, Client client)
            :base(fort, client)
        {
            lastModifiedTimestampMs = fort.LastModifiedTimestampMs;
            cooldownCompleteTimestampMs = fort.CooldownCompleteTimestampMs;
            if (cooldownCompleteTimestampMs > DateTime.UtcNow.ToUnixTime())
            {
                isActiveTimer = new DispatcherTimer();
                isActiveTimer.Tick += IsActiveTimer_Tick;
                isActiveTimer.Interval = TimeSpan.FromSeconds(1);
            }
            else
                IsActive = true;

            Search = new AsyncRelayCommand(async () =>
            {
                var searchResult = await client.Fort.SearchFort(Id, Position.Latitude, Position.Longitude);
                // TODO set new timer
                if(searchResult.Result == POGOProtos.Networking.Responses.FortSearchResponse.Types.Result.Success)
                {
                    // TODO add new items to inventory and add xp to player
                }
            });
        }

        private void IsActiveTimer_Tick(object sender, System.EventArgs e)
        {
            if (cooldownCompleteTimestampMs >= DateTime.UtcNow.ToUnixTime())
            {
                isActiveTimer.Tick -= IsActiveTimer_Tick;
                IsActive = true;
            }
        }
    }
}