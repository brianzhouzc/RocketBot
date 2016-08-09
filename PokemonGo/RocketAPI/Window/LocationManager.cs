using System;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.Extensions;

namespace PokemonGo.RocketAPI.Window
{
    public class LocationManager
    {
        private readonly Client _client;
        private readonly double _metersPerMillisecond;

        public LocationManager(Client client, double speed)
        {
            this._client = client;
            _metersPerMillisecond = speed/3600;
        }

        public double GetDistance(double lat, double lng)
        {
            var currentLoc = new LatLong(_client.CurrentLatitude, _client.CurrentLongitude);
            return currentLoc.distanceFrom(new LatLong(lat, lng));
        }

        public async Task Update(double lat, double lng)
        {
            var waitTime = GetDistance(lat, lng)/_metersPerMillisecond;
            await Task.Delay((int) Math.Ceiling(waitTime));
            await _client.Player.UpdatePlayerLocation(lat, lng);
        }
    }
}