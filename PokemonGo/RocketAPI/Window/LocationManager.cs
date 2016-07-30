using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.RocketAPI;

namespace PokemonGo.RocketAPI.Window
{
    public class LocationManager
    {
        private Client client;
        private double kilometersPerMillisecond;

        public LocationManager(Client client, double speed)
        {
            this.client = client;
            this.kilometersPerMillisecond = speed / 3600000;
        }

        public double getDistance(double lat, double lng)
        {
            Coordinate currentLoc = new Coordinate(client.CurrentLatitude, client.CurrentLongitude);
            return currentLoc.distanceFrom(new Coordinate(lat, lng));
        }

        public async Task update(double lat, double lng)
        {
            double waitTime = getDistance(lat, lng)/this.kilometersPerMillisecond;
            await Task.Delay((int)Math.Ceiling(waitTime));
            await client.UpdatePlayerLocation(lat, lng);
        }
    }

    public class Coordinate : Tuple<double, double>, ICoordinate
    {
        public Coordinate(double item1, double item2) : base(item1, item2)
        {
        }

        public double Latitude
        {
            get
            {
                return this.Item1;
            }
        }
        public double Longitude
        {
            get
            {
                return this.Item2;
            }
        }

        public double distanceFrom(ICoordinate c2)
        {
            return new CoordinateMetric().distance(this, c2);
        }


    }

}
