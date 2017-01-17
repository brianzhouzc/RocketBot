using System;
using System.IO;
using System.Net;
using GeoCoordinatePortable;
using PoGo.NecroBot.Logic.Model.Mapzen;
using PoGo.NecroBot.Logic.State;

namespace PoGo.NecroBot.Logic.Service
{
    class MapzenDirectionsService
    {
        private readonly ISession _session;

        public MapzenDirectionsService(ISession session)
        {
            _session = session;
        }

        public MapzenWalk GetDirections(GeoCoordinate sourceLocation, GeoCoordinate destLocation)
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
                        return MapzenWalk.Get(responseFromServer, sourceLocation, destLocation);
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
            string url = "http://valhalla.mapzen.com/route?json={\"locations\":" + "[{\"lat\":" + sourceLocation.Latitude + ",\"lon\":" + sourceLocation.Longitude + "},{\"lat\":" + destLocation.Latitude + ",\"lon\":" + destLocation.Longitude + "}]," + $"\"costing\":\"{_session.LogicSettings.MapzenWalkHeuristic}\",\"directions_options\":" + "{\"narrative\":\"false\"}}";

            if (!string.IsNullOrEmpty(_session.LogicSettings.MapzenTurnByTurnApiKey))
                url += $"&api_key={_session.LogicSettings.MapzenTurnByTurnApiKey}";

            return url;
        }
    }
}