using GMap.NET.WindowsForms;
using PokemonGo.RocketAPI.GeneratedCode;
using PokemonGo.RocketAPI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.Window
{
    public static class RouteOptimizer<T> where T: ILatLong
    {
        public static List<T> Optimize(T[] stops, ILatLong startingPosition, GMapOverlay routeOverlay)
        {
            List<T> optimizedRoute = new List<T>(stops);

            // NN
            T NN = FindNN(optimizedRoute, startingPosition);
            optimizedRoute.Remove(NN);
            optimizedRoute.Insert(0, NN);
            for (int i=1; i< stops.Length; i++)
            {
                NN = FindNN(optimizedRoute.Skip(i), NN);
                optimizedRoute.Remove(NN);
                optimizedRoute.Insert(i, NN);
            }

            // 2-Opt
            optimizedRoute = Optimize2Opt(optimizedRoute);

            return optimizedRoute;
        }

        public static float routeCost(List<T> stops)
        {
            return Enumerable.Range(0, stops.Count - 1).Aggregate<int, float>(0, (sum,i) =>
            {
                return sum + (float)stops[i].distanceFrom(stops[i + 1]);
            });
        }

        private static List<T> reverseSublist(List<T> stops, int startIndex, int endIndex)
        {
            return stops
                .Take(startIndex)
                .Concat(
                    stops
                    .Skip(startIndex)
                    .Take(endIndex - startIndex)
                    .Reverse()
                ).Concat(stops.Skip(endIndex)).ToList();
        }

        private static List<T> Optimize2Opt(List<T> stops)
        {
            List<T> optimizedRoute = stops;

            int n = stops.Count;
            bool foundCheaperRoute;
            float minCost = routeCost(optimizedRoute);
            do
            {
                foundCheaperRoute = false;
                for (int i = 0; i < n - 1; i++)
                {
                    for (int j = i + 1; j < n ; j++)
                    {
                        List<T> newRoute = reverseSublist(optimizedRoute, i, j);
                        float newCost = routeCost(newRoute);
                        if (newCost < minCost)
                        {
                            minCost = newCost;
                            optimizedRoute = newRoute;
                            foundCheaperRoute = true;
                            break;
                        }
                    }
                    if (foundCheaperRoute)
                        break;
                }
            }
            while (foundCheaperRoute);
            return optimizedRoute;
        }

        private static T FindNN(IEnumerable<T> stops, ILatLong fromPosition)
        {
            return stops.OrderBy(p => fromPosition.distanceFrom(p)).First();
        }

    }
}
