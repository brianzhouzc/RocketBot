using Newtonsoft.Json;
using PokemonGo.RocketAPI.Exceptions;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace PokemonGo.RocketAPI.Login
{
    class PtcLogin : ILoginType
    {
        readonly string password;
        readonly string username;
        readonly ISettings _settings;

        public PtcLogin(string username, string password, ISettings settings)
        {
            this.username = username;
            this.password = password;
            this._settings = settings;
        }

        public async Task<string> GetAccessToken()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip,
                AllowAutoRedirect = false,
                UseProxy = Client.Proxy != null,
                Proxy = Client.Proxy
            };

            using (var tempHttpClient = new System.Net.Http.HttpClient(handler))
            {
                //Get session cookie
                var sessionData = await GetSessionCookie(tempHttpClient).ConfigureAwait(false);

                //Login
                var ticketId = await GetLoginTicket(username, password, tempHttpClient, sessionData).ConfigureAwait(false);

                //Get tokenvar
                var authToken = await GetToken(tempHttpClient, ticketId).ConfigureAwait(false);
                if (authToken == null) throw new PtcOfflineException();
                return authToken;
            }
        }

        private static string ExtractTicketFromResponse(HttpResponseMessage loginResp)
        {
            var location = loginResp.Headers.Location;
            if (location == null)
                throw new LoginFailedException();

            var ticketId = HttpUtility.ParseQueryString(location.Query)["ticket"];

            if (ticketId == null)
                throw new PtcOfflineException();

            return ticketId;
        }

        private static IDictionary<string, string> GenerateLoginRequest(SessionData sessionData, string user, string pass)
            => new Dictionary<string, string>
            {
                { "lt", sessionData.Lt },
                { "execution", sessionData.Execution },
                { "_eventId", "submit" },
                { "username", user },
                { "password", pass }
            };

        private static IDictionary<string, string> GenerateTokenVarRequest(string ticketId)
            => new Dictionary<string, string>
            {
                {"client_id", "mobile-app_pokemon-go"},
                {"redirect_uri", "https://www.nianticlabs.com/pokemongo/error"},
                {"client_secret", "w8ScCUXJQc6kXKw8FiOhd8Fixzht18Dq3PEVkUCP5ZPxtgyWsbTvWHFLm2wNY0JR"},
                {"grant_type", "refresh_token"},
                {"code", ticketId}
            };

        private static async Task<string> GetLoginTicket(string username, string password, System.Net.Http.HttpClient tempHttpClient, SessionData sessionData)
        {
            HttpResponseMessage loginResp;
            var loginRequest = GenerateLoginRequest(sessionData, username, password);
            using (var formUrlEncodedContent = new FormUrlEncodedContent(loginRequest))
            {
                loginResp = await tempHttpClient.PostAsync(Resources.PtcLoginUrl, formUrlEncodedContent).ConfigureAwait(false);
            }

            var ticketId = ExtractTicketFromResponse(loginResp);
            return ticketId;
        }

        private static async Task<SessionData> GetSessionCookie(System.Net.Http.HttpClient tempHttpClient)
        {
            var sessionResp = await tempHttpClient.GetAsync(Resources.PtcLoginUrl).ConfigureAwait(false);
            var data = await sessionResp.Content.ReadAsStringAsync().ConfigureAwait(false);
            var sessionData = JsonConvert.DeserializeObject<SessionData>(data);
            return sessionData;
        }

        private static async Task<string> GetToken(System.Net.Http.HttpClient tempHttpClient, string ticketId)
        {
            HttpResponseMessage tokenResp;
            var tokenRequest = GenerateTokenVarRequest(ticketId);
            using (var formUrlEncodedContent = new FormUrlEncodedContent(tokenRequest))
            {
                tokenResp = await tempHttpClient.PostAsync(Resources.PtcLoginOauth, formUrlEncodedContent).ConfigureAwait(false);
            }

            var tokenData = await tokenResp.Content.ReadAsStringAsync().ConfigureAwait(false);
            return HttpUtility.ParseQueryString(tokenData)["access_token"];
        }

        private class SessionData
        {
            public string Lt { get; set; }
            public string Execution { get; set; }
        }
    }
}