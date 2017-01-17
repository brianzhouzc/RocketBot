using System;
using System.Collections.Generic;
using GeoCoordinatePortable;
using Newtonsoft.Json.Linq;

namespace PoGo.NecroBot.Logic.Model.Mapzen
{
    public class Trip
    {
        public string language { get; set; }
        public Summary summary { get; set; }
        public int status { get; set; }
        public string status_message { get; set; }
        public string units { get; set; }
        public List<Legs> legs { get; set; }
        public List<Location> locations { get; set; }
    }

    public class Summary
    {
        public double max_lon { get; set; }
        public double max_lat { get; set; }
        public int time { get; set; }
        public double length { get; set; }
        public double min_lon { get; set; }
        public double min_lat { get; set; }
    }

    public class Location
    {
        public double lon { get; set; }
        public double lat { get; set; }
    }

    public class Legs
    {
        public string shape { get; set; }
        public Summary summary { get; set; }
    }

    public class MapzenWalk
    {
        public List<GeoCoordinate> Waypoints { get; set; }
        public double Distance { get; set; }

        public MapzenWalk(string mapzenResponse, GeoCoordinate sourceLocation, GeoCoordinate destLocation)
        {
            Waypoints = new List<GeoCoordinate>();

            // Add the source
            Waypoints.Add(sourceLocation);

            JObject jsonObj = JObject.Parse(mapzenResponse);

            var trip = jsonObj["trip"];
            int status = (int) trip["status"];
            if (status == 0)
            {
                JObject summary = (JObject) trip["summary"];
                Distance = (double) summary["length"] * 1000;

                JArray legs = (JArray) trip["legs"];
                foreach (var leg in legs)
                {
                    string shape = (string) leg["shape"];

                    // Decode the polyline.
                    Waypoints.AddRange(DecodePoly(shape));
                }
            }

            // Add the destination
            Waypoints.Add(destLocation);
        }

        public List<GeoCoordinate> DecodePoly(string points, int precision = 6)
        {
            var poly = new List<GeoCoordinate>();
            if (string.IsNullOrEmpty(points))
                throw new ArgumentNullException("polyline");

            char[] polylineChars = points.ToCharArray();
            int index = 0;
            double lat = 0;
            double lng = 0;
            double latitudeChange = 0;
            double longitudeChange = 0;
            int next5bits;
            int sum;
            int shifter;
            double factor = Math.Pow(10, precision);

            while (index < polylineChars.Length)
            {
                // calculate next latitude
                next5bits = 0;
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int) polylineChars[index++] - 63;
                    sum |= (next5bits & 0x1f) << shifter;
                    shifter += 5;
                } while (next5bits >= 0x20);

                latitudeChange = (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                //calculate next longitude
                next5bits = 0;
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int) polylineChars[index++] - 63;
                    sum |= (next5bits & 0x1f) << shifter;
                    shifter += 5;
                } while (next5bits >= 0x20);

                longitudeChange = (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                lat += latitudeChange;
                lng += longitudeChange;

                poly.Add(new GeoCoordinate()
                {
                    Latitude = lat / factor,
                    Longitude = lng / factor
                });
            }

            return poly;
        }

        public static MapzenWalk Get(string mapzenResponse, GeoCoordinate sourceLocation, GeoCoordinate destLocation)
        {
            return new MapzenWalk(mapzenResponse, sourceLocation, destLocation);
        }
    }
}