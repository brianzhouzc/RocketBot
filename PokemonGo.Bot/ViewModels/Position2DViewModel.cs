using System;
using System.Globalization;

namespace PokemonGo.Bot.ViewModels
{
    public class Position2DViewModel
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public Position2DViewModel(double lat, double lon)
        {
            Latitude = lat;
            Longitude = lon;
        }

        public double DistanceTo(Position2DViewModel other)
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

        public Position3DViewModel To3D(double alt)
            => new Position3DViewModel(Latitude, Longitude, alt);

        public override bool Equals(object obj) => Equals(obj as Position2DViewModel);

        public bool Equals(Position2DViewModel other)
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

        public override string ToString() => string.Format(CultureInfo.InvariantCulture, "{0},{1}", Latitude, Longitude);
    }
}