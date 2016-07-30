using GMap.NET.WindowsForms;
using PokemonGo.RocketAPI.GeneratedCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.Window
{
    public static class PokeStopOptimizer
    {
        public static List<FortData> Optimize(FortData[] pokeStops, double cLatitude, double cLongitude, GMapOverlay routeOverlay)
        {
            List<FortData> optimizedRoute = new List<FortData>(pokeStops);

            // NN
            FortData NN = FindNN(optimizedRoute, cLatitude, cLongitude);
            optimizedRoute.Remove(NN);
            optimizedRoute.Insert(0, NN);
            for (int i=1; i<pokeStops.Length; i++)
            {
                NN = FindNN(optimizedRoute.Skip(i), NN.Latitude, NN.Longitude);
                optimizedRoute.Remove(NN);
                optimizedRoute.Insert(i, NN);
            }

            // 2-Opt


            return optimizedRoute;
        }

        private static List<FortData> Optimize2Opt(List<FortData> pokeStops)
        {
            List<FortData> optimizedRoute = new List<FortData>();

            int n = pokeStops.Count;

            for (int ai = 0; ai < n; ai++)
            {
                for (int ci = 0; ci < n; ci++)
                {
                    int bi = (ai + 1) % n;
                    int di = (ci + 1) % n;
                }
            }

            return optimizedRoute;
        }

        private static FortData FindNN(IEnumerable<FortData> pokeStops, double cLatitude, double cLongitude)
        {
            return pokeStops.OrderBy(p => GetDistance(cLatitude, cLongitude, p.Latitude, p.Longitude)).First();
        }

        private static float GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double R = 6371e3;
            Func<double, float> toRad = x => (float)(x * (Math.PI / 180));
            lat1 = toRad(lat1);
            lat2 = toRad(lat2);
            float dLng = toRad(lng2 - lng1);

            return (float)(Math.Acos(Math.Sin(lat1)*Math.Sin(lat2) + Math.Cos(lat1)*Math.Cos(lat2)*Math.Cos(dLng)) * R);
        }
    }
}
