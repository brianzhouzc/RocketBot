using System;

namespace PokemonGo.RocketAPI
{
    public interface ILatLong
    {
        double Latitude { get; }
        double Longitude { get; }
    }

    public class LatLong : Tuple<double, double>, ILatLong
    {
        public LatLong(double item1, double item2) : base(item1, item2)
        {
        }

        public double Latitude
        {
            get
            {
                return Item1;
            }
        }
        public double Longitude
        {
            get
            {
                return Item2;
            }
        }
    }
}
