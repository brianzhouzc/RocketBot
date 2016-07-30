using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI
{
    public interface ICoordinate
    {
        double Latitude { get; }
        double Longitude { get; }
    }
    public interface MetricSpace<T>
    {
        double distance(T t1, T t2);
    }
    public class CoordinateMetric : MetricSpace<ICoordinate>
    {
        public double distance(ICoordinate c1, ICoordinate c2)
        {
            double R = 6371;
            Func<double, double> toRad = x => x * (Math.PI / 180);
            double dLat = toRad(c2.Latitude - c1.Latitude);
            double dLong = toRad(c2.Latitude - c2.Longitude);
            double lat1 = toRad(c1.Latitude);
            double lat2 = toRad(c2.Latitude);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Sin(dLong / 2) * Math.Sin(dLong / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
    }
}
