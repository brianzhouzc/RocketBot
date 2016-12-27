using Caching;
using GeoCoordinatePortable;
using PoGo.NecroBot.Logic.Model.Settings;
using PoGo.NecroBot.Logic.State;
using System;

namespace PoGo.NecroBot.Logic.Service.Elevation
{
    public abstract class BaseElevationService : IElevationService
    {
        protected GlobalSettings _settings;
        protected LRUCache<string, double> _cache;
        protected string _apiKey;

        public abstract string GetServiceId();
        public abstract double GetElevationFromWebService(double lat, double lng);

        public BaseElevationService(GlobalSettings settings, LRUCache<string, double> cache)
        {
            _settings = settings;
            _cache = cache;
        }

        public string GetCacheKey(double lat, double lng)
        {
            return Math.Round(lat, 3) + "," + Math.Round(lng, 3);
        }

        public string GetCacheKey(GeoCoordinate position)
        {
            return GetCacheKey(position.Latitude, position.Longitude);
        }

        public double GetElevation(double lat, double lng)
        {
            string cacheKey = GetCacheKey(lat, lng);
            double elevation;
            bool success = _cache.TryGetValue(cacheKey, out elevation);
            if (!success)
            {
                elevation = GetElevationFromWebService(lat, lng);
                if (elevation == 0 || elevation < -100)
                {
                    // Just return the elevation without caching.  Since this is invalid elevation, we want the 
                    // elevation service to blacklist this service and move to the next one.
                    return elevation;
                }
                else
                {
                    _cache.Add(cacheKey, elevation);
                }
            }

            // Always return a slightly random elevation.
            return GetRandomElevation(elevation);
        }

        public void UpdateElevation(ref GeoCoordinate position)
        {
            double elevation = GetElevation(position.Latitude, position.Longitude);
            // Only update the position elevation if we got a non-zero elevation.
            if (elevation != 0)
            {
                position.Altitude = elevation;
            }
        }

        public double GetRandomElevation(double elevation)
        {
            // Adds a random elevation to the retrieved one. This was
            // previously set to 5 meters but since it's happening with
            // just a few seconds in between it is deemed unrealistic. 
            // Telling from real world examples ~1.2 meter fits better.
            return elevation + (new Random().NextDouble() * 1.2);
        }
    }
}
