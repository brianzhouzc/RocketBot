using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Extensions;

namespace PokemonGo.RocketAPI.Window
{
    public class LocationManager
    {
        private Client client;
        private double metersPerMillisecond;

        public LocationManager(Client client, double speed)
        {
            this.client = client;
            this.metersPerMillisecond = speed / 3600;
        }

        public double getDistance(double lat, double lng)
        {
            LatLong currentLoc = new LatLong(client.CurrentLatitude, client.CurrentLongitude);
            return currentLoc.distanceFrom(new LatLong(lat, lng));
        }

        public async Task update(double lat, double lng)
        {
            double waitTime = getDistance(lat, lng) / this.metersPerMillisecond;
            await Task.Delay((int)Math.Ceiling(waitTime));
            await client.UpdatePlayerLocation(lat, lng);
        }
    }

}