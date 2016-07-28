#region

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace PokemonGo.RocketAPI.Helpers
{
    public static class HttpClientHelper
    {
        public static async Task<TResponse> PostFormEncodedAsync<TResponse>(string url,
            params KeyValuePair<string, string>[] keyValuePairs)
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip,
                AllowAutoRedirect = false
            };

            using (var tempHttpClient = new HttpClient(handler))
            {
                var response = await tempHttpClient.PostAsync(url, new FormUrlEncodedContent(keyValuePairs));
                return await response.Content.ReadAsAsync<TResponse>();
            }
        }

        public static async Task<TResponse> Get<TResponse>(string url,
            params KeyValuePair<string, string>[] keyValuePairs)
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip,
                AllowAutoRedirect = false
            };

            if (keyValuePairs.Length > 0)
            {
                if (!url.Last().Equals('?'))
                    url += "?";

                var builder = keyValuePairs.Aggregate(new StringBuilder(),
                      (sb, kvp) => sb.AppendFormat("{0}={1}&", kvp.Key, kvp.Value));

                // remove trailing '&'
                if (builder.Length > 0)
                    builder.Remove(builder.Length - 1, 1);

                url += builder.ToString();
            }

            using (var tempHttpClient = new HttpClient(handler))
            {
                var response = await tempHttpClient.GetAsync(url);
                return await response.Content.ReadAsAsync<TResponse>();
            }

        }
    }


}