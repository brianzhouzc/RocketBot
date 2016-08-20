using System;
using System.Collections.Generic;
using System.Linq;
using POGOProtos.Map.Fort;
using GMap.NET.WindowsForms;
using PokemonGo.RocketBot.Logic.Logging;
using System.Diagnostics;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using PokemonGo.RocketBot.Logic.Event;
using System.Threading.Tasks;

namespace PokemonGo.RocketBot.Logic.Utils
{
    public delegate void RouteOptimizeDelegate(List<FortData> optimizedRoute);

    public static class RouteOptimizeUtil
    {
        public static List<FortData> Optimize(FortData[] pokeStops, double lat, double lng,
            bool randomizestartpoint = false)
        {
            var optimizedRoute = new List<FortData>(pokeStops);
            // NN
            var nn = !randomizestartpoint
                ? FindNn(optimizedRoute, lat, lng)
                : optimizedRoute[new Random().Next(optimizedRoute.Count)];

            optimizedRoute.Remove(nn);
            optimizedRoute.Insert(0, nn);
            for (var i = 1; i < pokeStops.Length; i++)
            {
                nn = FindNn(optimizedRoute.Skip(i), nn.Latitude, nn.Longitude);
                optimizedRoute.Remove(nn);
                optimizedRoute.Insert(i, nn);
            }


            // 2-Opt
            bool isOptimized;
            do
            {
                optimizedRoute = Optimize2Opt(optimizedRoute, out isOptimized);
            } while (isOptimized);
            //OnRouteOptimizeEvent(optimizedRoute);

            return optimizedRoute;
        }

        public static List<FortData> OptimizeHumanizeRoute(FortData[] pokeStops, double lat, double lng,
            bool randomizestartpoint = false)
        {
            var apikey = "AIzaSyD-9h_V9GjpczxcOOBb5payKuWrrAjk_jg";
            //THIS IS THE LIST WITHOUT ANY MODIFY
            var originalroute = new List<FortData>(Optimize(pokeStops, lat, lng, randomizestartpoint));

            //Split list into lists with length smaller or equal to 24 
            var splitroutecontainer = SplitList(originalroute, 24);

            List<FortData> finaList = new List<FortData>();
            for (var scc = 0; scc < splitroutecontainer.Count; scc++)
            {
                var splitedroutes = splitroutecontainer[scc];
                //Getting the starting point. The start point is the same as the end point if this isn't the first list
                var start = scc == 0 ? splitedroutes[0] : splitroutecontainer[scc - 1][splitroutecontainer[scc - 1].Count - 1];
                var end = splitedroutes[splitedroutes.Count - 1];
                var requesturl =
                    $"https://maps.googleapis.com/maps/api/directions/json?origin={start.Latitude},{start.Longitude}&destination={end.Latitude},{end.Longitude}&waypoints=optimize:true|";

                //Create list to store waypoints
                List<FortData> waypoints = new List<FortData>(splitedroutes);
                //Excluded start only if this is the first list
                if (scc == 0)
                    waypoints.Remove(waypoints[0]);
                //Excluded end
                waypoints.Remove(waypoints[waypoints.Count - 1]);

                foreach (var waypoint in waypoints)
                {
                    //Building request url
                    requesturl += $"{waypoint.Latitude},{waypoint.Longitude}|";
                }
                requesturl += $"&mode=walking&units=metric&key={apikey}";

                //Get resond from Google
                var parseObject = JObject.Parse(GetRespond(requesturl));
                //A list storing the order of waypoints
                var orderlist = parseObject["routes"][0]["waypoint_order"].ToList();

                //Initialize a list to store the sorted waypoints
                List<FortData> buildinglist = new List<FortData>(waypoints);
                if (scc == 0)
                {
                    buildinglist.Insert(0, start);
                }
                for (var d = 0; d < orderlist.Count; d++)
                {
                    if (scc == 0)
                    {
                        buildinglist[d + 1] = waypoints[(int)orderlist[d]];
                    }
                    else
                    {
                        buildinglist[d] = waypoints[(int)orderlist[d]];
                    }
                }
                buildinglist.Add(end);

                finaList.AddRange(buildinglist);
            }
            OnRouteOptimizeEvent(finaList);
            return finaList;

            /*var start = route[0];
            //var endpoint = route[route.Count - 1];
            var end = route[24];
            var requesturl =
                $"https://maps.googleapis.com/maps/api/directions/json?origin={start.Latitude},{start.Longitude}&destination={end.Latitude},{end.Longitude}&waypoints=optimize:true|";
            for (var i = 1; i < 24; i++)
            {
                var point = route[i];
                requesturl += $"{point.Latitude},{point.Longitude}|";
            }
            requesturl += $"&mode=walking&units=metric&key={apikey}";
            Logger.Write(requesturl);
            try
            {
                var web = WebRequest.Create(requesturl);
                web.Credentials = CredentialCache.DefaultCredentials;

                string strResponse;
                using (var response = web.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        Debug.Assert(stream != null, "stream != null");
                        using (var reader = new StreamReader(stream))
                            strResponse = reader.ReadToEnd();
                    }
                }
                var parseObject = JObject.Parse(strResponse);
                var l = parseObject["routes"][0]["waypoint_order"].ToString();
                Logger.Write(strResponse);
            }
            catch (WebException e)
            {

            }
            catch (NullReferenceException e)
            {

            }

            return null; */
        }

        private static List<FortData> Optimize2Opt(List<FortData> pokeStops, out bool isOptimized)
        {
            var n = pokeStops.Count;
            float bestGain = 0;
            var bestI = -1;
            var bestJ = -1;

            for (var ai = 0; ai < n; ai++)
            {
                for (var ci = 0; ci < n; ci++)
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
                        var gain = ab + cd - (ac + bd);
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
                    optimizedRoute = new List<FortData> { pokeStops[0] };
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

        private static FortData FindNn(IEnumerable<FortData> pokeStops, double cLatitude, double cLongitude)
        {
            return pokeStops.OrderBy(p => GetDistance(cLatitude, cLongitude, p.Latitude, p.Longitude)).First();
        }

        private static float GetDistance(FortData a, FortData b)
        {
            return GetDistance(a.Latitude, a.Longitude, b.Latitude, b.Longitude);
        }

        private static float GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            const double R = 6371e3;
            Func<double, float> toRad = x => (float)(x * (Math.PI / 180));
            lat1 = toRad(lat1);
            lat2 = toRad(lat2);
            var dLng = toRad(lng2 - lng1);

            return (float)(Math.Acos(Math.Sin(lat1) * Math.Sin(lat2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(dLng)) * R);
        }

        private static void OnRouteOptimizeEvent(List<FortData> optimizedroute)
        {
            RouteOptimizeEvent?.Invoke(optimizedroute);
        }

        public static List<List<T>> SplitList<T>(List<T> locations, int nSize = 30)
        {
            var list = new List<List<T>>();

            for (int i = 0; i < locations.Count; i += nSize)
            {
                list.Add(locations.GetRange(i, Math.Min(nSize, locations.Count - i)));
            }

            return list;
        }

        public static String GetRespond(string request)
        {
            try
            {
                var web = WebRequest.Create(request);
                web.Credentials = CredentialCache.DefaultCredentials;

                string strResponse;
                using (var response = web.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        Debug.Assert(stream != null, "stream != null");
                        using (var reader = new StreamReader(stream))
                            strResponse = reader.ReadToEnd();
                    }
                }
                return strResponse;
            }
            catch (WebException e)
            {
                Logger.Write(e.Message, LogLevel.Error);
            }
            catch (NullReferenceException e)
            {
                Logger.Write(e.Message, LogLevel.Error);

            }
            return null;
        }

        public static event RouteOptimizeDelegate RouteOptimizeEvent;

    }
}