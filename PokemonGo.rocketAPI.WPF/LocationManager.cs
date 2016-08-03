using System;
using System.Threading.Tasks;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Extensions;

namespace PokemonGo.rocketAPI.WPF
{
    public class LocationManager
    {
        private Client client;
        private double metersPerMillisecond;

        public LocationManager(Client client, double speed)
        {
            this.client = client;
            metersPerMillisecond = speed / 3600;
        }

        public double getDistance(double lat, double lng)
        {
            LatLong currentLoc = new LatLong(client.CurrentLatitude, client.CurrentLongitude);
            return currentLoc.distanceFrom(new LatLong(lat, lng));
        }

        public async Task update(double lat, double lng)
        {
            double waitTime = getDistance(lat, lng) / metersPerMillisecond;
            await Task.Delay((int)Math.Ceiling(waitTime));
            await client.UpdatePlayerLocation(lat, lng);
        }
    }

}