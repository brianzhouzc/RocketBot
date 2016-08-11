using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
