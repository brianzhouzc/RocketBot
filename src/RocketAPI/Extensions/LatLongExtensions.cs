using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.Extensions
{
    public static class LatLongExtensions
    {
        public static double distanceFrom(this ILatLong c1, ILatLong c2)
        {
            double R = 6371e3;
            Func<double, float> toRad = x => (float)(x * (Math.PI / 180));
            float lat1 = toRad(c1.Latitude);
            float lat2 = toRad(c2.Latitude);
            float dLat = toRad(c2.Latitude - c1.Latitude);
            float dLng = toRad(c2.Longitude - c1.Longitude);
            double h = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(h), Math.Sqrt(1 - h));
            return R * c;
        }
    }
}

