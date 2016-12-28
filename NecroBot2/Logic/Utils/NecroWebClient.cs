using System;
using System.Net;

namespace NecroBot2.Logic.Utils
{
    public class NecroWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri uri)
        {
            var w = base.GetWebRequest(uri);
            w.Timeout = 5000;
            return w;
        }
    }
}