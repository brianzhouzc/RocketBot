using System;
using System.IO;
using System.Net;
using GeoCoordinatePortable;
using PoGo.NecroBot.Logic.Model.Yours;
using PoGo.NecroBot.Logic.State;

namespace PoGo.NecroBot.Logic.Service
{
    class YoursDirectionsService
    {
        private readonly ISession _session;

        public YoursDirectionsService(ISession session)
        {
            _session = session;
        }

        public YoursWalk GetDirections(GeoCoordinate sourceLocation, GeoCoordinate destLocation)
        {
            WebRequest request = WebRequest.Create(GetUrl(sourceLocation, destLocation));
            request.Credentials = CredentialCache.DefaultCredentials;

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(dataStream))
                    {
                        string responseFromServer = reader.ReadToEnd();
                        return YoursWalk.Get(responseFromServer);
                    }
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        private string GetUrl(GeoCoordinate sourceLocation, GeoCoordinate destLocation)
        {
            string url = $"http://www.yournavigation.org/api/dev/route.php?format=geojson&flat={sourceLocation.Latitude}&flon={sourceLocation.Longitude}&tlat={destLocation.Latitude}&tlon={destLocation.Longitude}&fast=1&layer=mapnik";

            if (!string.IsNullOrEmpty(_session.LogicSettings.YoursWalkHeuristic))
                url += $"&v={_session.LogicSettings.YoursWalkHeuristic}";
            else
                url += $"&v=bicycle";

            return url;
        }
    }
}