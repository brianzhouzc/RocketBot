#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PokemonGo.RocketAPI.Helpers;
using System.IO;


#endregion

namespace PokemonGo.RocketAPI.Login
{
    public static class GoogleLogin
    {
        private const string OauthTokenEndpoint = "https://www.googleapis.com/oauth2/v4/token";
        private const string OauthEndpoint = "https://accounts.google.com/o/oauth2/device/code";
        private const string ClientId = "537398154704-dhmo8l7m9r61jdv7gb838004r73mlcg9.apps.googleusercontent.com";
        private const string ClientSecret = "6_z1_mpkkF8CujDf21I0xXFB";

        /// <summary>
        ///     Gets the access token from Google
        /// </summary>
        /// <param name="deviceCode"></param>
        /// <returns>tokenResponse</returns>
        public static async Task<TokenResponseModel> GetAccessToken(DeviceCodeModel deviceCode)
        {
            //Poll until user submitted code..
            TokenResponseModel tokenResponse;
            do
            {
                await Task.Delay(2000);
                tokenResponse = await PollSubmittedToken(deviceCode.device_code);
            } while (tokenResponse.access_token == null || tokenResponse.refresh_token == null);

            return tokenResponse;
        }

        public static async Task<TokenResponseModel> GetAccessToken(string refreshToken)
        {
            return await HttpClientHelper.PostFormEncodedAsync<TokenResponseModel>(OauthTokenEndpoint,
                new KeyValuePair<string, string>("access_type", "offline"),
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("client_secret", ClientSecret),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("scope", "openid email https://www.googleapis.com/auth/userinfo.email"));
        }

        public static async Task<DeviceCodeModel> GetDeviceCode()
        {
            var deviceCode = await HttpClientHelper.PostFormEncodedAsync<DeviceCodeModel>(OauthEndpoint,
            new KeyValuePair<string, string>("client_id", ClientId),
            new KeyValuePair<string, string>("scope", "openid email https://www.googleapis.com/auth/userinfo.email"));

            try
            {
                //ColoredConsoleWrite("Google Device Code copied to clipboard");
                System.Console.WriteLine($"Goto: http://www.google.com/device & enter {deviceCode.user_code}");
                File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Logs.txt", "[" + DateTime.Now.ToString("HH:mm:ss tt") + $"] Goto: http://www.google.com/device & enter {deviceCode.user_code}");
                Process.Start(@"http://www.google.com/device");
                var thread = new Thread(() => Clipboard.SetText(deviceCode.user_code)); //Copy device code
                thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                thread.Start();
                thread.Join();
            }
            catch (Exception)
            {
                //System.Console.WriteLine("Couldnt copy to clipboard, do it manually");
                //System.Console.WriteLine($"Goto: http://www.google.com/device & enter {deviceCode.user_code}");
            }

            return deviceCode;
        }

        private static async Task<TokenResponseModel> PollSubmittedToken(string deviceCode)
        {
            return await HttpClientHelper.PostFormEncodedAsync<TokenResponseModel>(OauthTokenEndpoint,
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("client_secret", ClientSecret),
                new KeyValuePair<string, string>("code", deviceCode),
                new KeyValuePair<string, string>("grant_type", "http://oauth.net/grant_type/device/1.0"),
                new KeyValuePair<string, string>("scope", "openid email https://www.googleapis.com/auth/userinfo.email"));
        }


        internal class ErrorResponseModel
        {
            public string error { get; set; }
            public string error_description { get; set; }
        }

        public class TokenResponseModel
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string refresh_token { get; set; }
            public string id_token { get; set; }
        }


        public class DeviceCodeModel
        {
            public string verification_url { get; set; }
            public int expires_in { get; set; }
            public int interval { get; set; }
            public string device_code { get; set; }
            public string user_code { get; set; }
        }
    }
}