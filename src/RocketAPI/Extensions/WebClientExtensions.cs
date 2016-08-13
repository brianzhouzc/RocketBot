using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PokemonGo.RocketAPI.Extensions
{
    public static class WebClientExtensions
    {
        public static string DownloadString(this WebClient webClient, Uri uri)
        {
            webClient.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
            webClient.Encoding = Encoding.UTF8;
            byte[] rawData = null;
            string error;
            try
            {
                error = "loading";
                rawData = webClient.DownloadData(uri);
            }
            catch (NullReferenceException)
            {
                error = null;
            }
            catch (ArgumentNullException)
            {
                error = null;
            }
            catch (WebException)
            {
                error = null;
            }
            catch (SocketException)
            {
                error = null;
            }

            if (error == null || rawData == null)
                return null;

            var encoding = GetEncodingFrom(webClient.ResponseHeaders, Encoding.UTF8);
            return encoding.GetString(rawData);
        }


        public static Encoding GetEncodingFrom(
            NameValueCollection responseHeaders,
            Encoding defaultEncoding = null)
        {
            if (responseHeaders == null)
                throw new ArgumentNullException("responseHeaders");

            //Note that key lookup is case-insensitive
            var contentType = responseHeaders["Content-Type"];
            if (contentType == null)
                return defaultEncoding;

            var contentTypeParts = contentType.Split(';');
            if (contentTypeParts.Length <= 1)
                return defaultEncoding;

            var charsetPart =
                contentTypeParts.Skip(1).FirstOrDefault(
                    p => p.TrimStart().StartsWith("charset", StringComparison.InvariantCultureIgnoreCase));
            if (charsetPart == null)
                return defaultEncoding;

            var charsetPartParts = charsetPart.Split('=');
            if (charsetPartParts.Length != 2)
                return defaultEncoding;

            var charsetName = charsetPartParts[1].Trim();
            if (charsetName == "")
                return defaultEncoding;

            try
            {
                return Encoding.GetEncoding(charsetName);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

    }
}