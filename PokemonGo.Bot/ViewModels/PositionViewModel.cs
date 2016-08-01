using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.Bot.ViewModels
{
    public class PositionViewModel
    {
        public double Latitude { get; }
        public double Longitude { get; }
        public PositionViewModel(double lat, double lon)
        {
            Latitude = lat;
            Longitude = lon;
        }
        public double DistanceTo(PositionViewModel other)
        {
            var R = 6371e3;
            Func<double, float> toRad = x => (float)(x * (Math.PI / 180));
            var lat1 = toRad(Latitude);
            var lat2 = toRad(other.Latitude);
            var dLat = toRad(other.Latitude - Latitude);
            var dLng = toRad(other.Longitude - Longitude);
            var h = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(h), Math.Sqrt(1 - h));
            return R * c;
        }

        public override bool Equals(object obj) => Equals(obj as PositionViewModel);

        public bool Equals(PositionViewModel other)
        {
            return other != null &&
                Latitude == other.Latitude &&
                Longitude == other.Longitude;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 13;
                hash = (hash * 7) + Latitude.GetHashCode();
                hash = (hash * 7) + Longitude.GetHashCode();
                return hash;
            }
        }
    }
}
