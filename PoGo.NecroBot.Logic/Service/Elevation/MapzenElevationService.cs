using Caching;
using Newtonsoft.Json.Linq;
using PoGo.NecroBot.Logic.Model.Settings;
using System;
using System.IO;
using System.Net;

namespace PoGo.NecroBot.Logic.Service.Elevation
{
    public class MapzenElevationService : BaseElevationService
    {
        public MapzenElevationService(GlobalSettings settings, LRUCache<string, double> cache) : base(settings, cache)
        {
            if (!string.IsNullOrEmpty(settings.MapzenWalkConfig.MapzenElevationApiKey))
                _apiKey = settings.MapzenWalkConfig.MapzenElevationApiKey;
        }

        public override string GetServiceId()
        {
            return "Mapzen Elevation Service";
        }

        public override double GetElevationFromWebService(double lat, double lng)
        {
            if (string.IsNullOrEmpty(_apiKey))
                return 0;

            try
            {
                string url = $"https://elevation.mapzen.com/height?json=" + "{\"shape\":[{\"lat\":" + lat + ",\"lon\":" + lng + "}]}" + $"&api_key={_apiKey}";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
                request.ContentType = "application/json";
                request.ReadWriteTimeout = 2000;
                string responseFromServer = "";

                using (WebResponse response = request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(dataStream))
                    {
                        responseFromServer = reader.ReadToEnd();

                        JObject jsonObj = JObject.Parse(responseFromServer);

                        JArray heights = (JArray)jsonObj["height"];
                        return (double)heights[0];

                        // All error handling is handled inside of the ElevationService.
                    }
                }
            }
            catch(Exception)
            {
                // If we get here for any reason, then just drop down and return 0. Will cause this elevation service to be blacklisted.
            }

            return 0;
        }
    }
}
