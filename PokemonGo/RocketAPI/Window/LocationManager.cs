using System;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.Extensions;

namespace PokemonGo.RocketAPI.Window
{
    public class LocationManager
    {
        private readonly Client client;
        private readonly double metersPerMillisecond;

        public LocationManager(Client client, double speed)
        {
            this.client = client;
            metersPerMillisecond = speed/3600;
        }

        public double getDistance(double lat, double lng)
        {
            var currentLoc = new LatLong(client.CurrentLatitude, client.CurrentLongitude);
            return currentLoc.distanceFrom(new LatLong(lat, lng));
        }

        public async Task update(double lat, double lng)
        {
            var waitTime = getDistance(lat, lng)/metersPerMillisecond;
            await Task.Delay((int) Math.Ceiling(waitTime));
            await client.UpdatePlayerLocation(lat, lng);
        }
    }
}