using PokemonGo.Bot.ViewModels;
using PokemonGo.RocketAPI.GeneratedCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.Bot.utils
{
    public static class RouteOptimizer
    {
        public static List<FortData> Optimize(IEnumerable<FortData> pokeStops, PositionViewModel position)
        {
            var optimizedRoute = new List<FortData>(pokeStops);

            // NN
            var NN = FindNN(optimizedRoute, position.Latitude, position.Longitude);
            optimizedRoute.Remove(NN);
            optimizedRoute.Insert(0, NN);
            for (int i = 1; i < optimizedRoute.Count; i++)
            {
                NN = FindNN(optimizedRoute.Skip(i), NN.Latitude, NN.Longitude);
                optimizedRoute.Remove(NN);
                optimizedRoute.Insert(i, NN);
            }

            // 2-Opt
            bool isOptimized;
            do
            {
                optimizedRoute = Optimize2Opt(optimizedRoute, out isOptimized);
            }
            while (isOptimized);

            return optimizedRoute;
        }

        //private static void Visualize(List<FortData> pokeStops, GMapOverlay routeOverlay)
        //{
        //    MainForm.synchronizationContext.Post(new SendOrPostCallback(o =>
        //    {
        //        List<FortData> p = new List<FortData>((List<FortData>)o);
        //        routeOverlay.Markers.Clear();
        //        List<PointLatLng> routePoint = new List<PointLatLng>();
        //        foreach (var pokeStop in p)
        //        {
        //            var pokeStopLoc = new PointLatLng(pokeStop.Latitude, pokeStop.Longitude);

        //            routePoint.Add(pokeStopLoc);
        //        }
        //        routeOverlay.Routes.Clear();
        //        routeOverlay.Routes.Add(new GMapRoute(routePoint, "Walking Path"));
        //    }), pokeStops);
        //}

        static List<FortData> Optimize2Opt(List<FortData> pokeStops, out bool isOptimized)
        {
            var n = pokeStops.Count;
            var bestGain = 0f;
            var bestI = -1;
            var bestJ = -1;

            for (int ai = 0; ai < n; ai++)
            {
                for (int ci = 0; ci < n; ci++)
                {
                    var bi = (ai + 1) % n;
                    var di = (ci + 1) % n;

                    var a = pokeStops[ai];
                    var b = pokeStops[bi];
                    var c = pokeStops[ci];
                    var d = pokeStops[di];

                    var ab = GetDistance(a, b);
                    var cd = GetDistance(c, d);
                    var ac = GetDistance(a, c);
                    var bd = GetDistance(b, d);

                    if (ci != ai && ci != bi)
                    {
                        var gain = (ab + cd) - (ac + bd);
                        if (gain > bestGain)
                        {
                            bestGain = gain;
                            bestI = bi;
                            bestJ = ci;
                        }
                    }
                }
            }

            if (bestI != -1)
            {
                List<FortData> optimizedRoute;
                if (bestI > bestJ)
                {
                    optimizedRoute = new List<FortData>();
                    optimizedRoute.Add(pokeStops[0]);
                    optimizedRoute.AddRange(pokeStops.Skip(bestI));
                    optimizedRoute.Reverse(1, n - bestI);
                    optimizedRoute.AddRange(pokeStops.GetRange(bestJ + 1, bestI - bestJ - 1));
                    optimizedRoute.AddRange(pokeStops.GetRange(1, bestJ));
                    optimizedRoute.Reverse(n - bestJ, bestJ);
                }
                else if (bestI == 0)
                {
                    optimizedRoute = new List<FortData>(pokeStops);
                    optimizedRoute.Reverse(bestJ + 1, n - bestJ - 1);
                }
                else
                {
                    optimizedRoute = new List<FortData>(pokeStops);
                    optimizedRoute.Reverse(bestI, bestJ - bestI + 1);
                }

                isOptimized = true;
                return optimizedRoute;
            }
            isOptimized = false;
            return pokeStops;
        }

        static FortData FindNN(IEnumerable<FortData> pokeStops, double cLatitude, double cLongitude)
        {
            return pokeStops.OrderBy(p => GetDistance(cLatitude, cLongitude, p.Latitude, p.Longitude)).First();
        }

        static float GetDistance(FortData a, FortData b)
        {
            return GetDistance(a.Latitude, a.Longitude, b.Latitude, b.Longitude);
        }

        static float GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            var R = 6371e3;
            Func<double, float> toRad = x => (float)(x * (Math.PI / 180));
            lat1 = toRad(lat1);
            lat2 = toRad(lat2);
            var dLng = toRad(lng2 - lng1);

            return (float)(Math.Acos(Math.Sin(lat1) * Math.Sin(lat2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(dLng)) * R);
        }
    }
}