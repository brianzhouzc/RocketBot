using System;
using System.Globalization;

namespace PokemonGo.Bot.ViewModels
{
    public class Position2DViewModel
    {
        public double Latitude { get; }
        public double Longitude { get; }
        const int R = 6371000;
        const double PiBy180 = Math.PI / 180;

        public Position2DViewModel(double lat, double lon)
        {
            Latitude = lat;
            Longitude = lon;
        }

        public double DistanceTo(Position2DViewModel other)
        {
            var lat1 = ToRadians(Latitude);
            var lat2 = ToRadians(other.Latitude);
            var dLat = ToRadians(other.Latitude - Latitude);
            var dLng = ToRadians(other.Longitude - Longitude);
            var h = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(h), Math.Sqrt(1 - h));
            return R * c;
        }

        protected static double ToRadians(double degrees) => degrees * PiBy180;

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