using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Captcha
{
    public class CaptchaSolutionClient
    {
        private const string API_ENDPOINT = "http://api.captchasolutions.com/solve";

        public class APIObjectResponse
        {
            public string captchasolutions { get; set; }
        }

        public string APIKey { get; set; }
        public string APISecret { get; set; }

        public int Timeout { get; set; }
        public CaptchaSolutionClient(string key, string secret, int timeout = 120)
        {
            this.APIKey = key;
            this.APISecret = secret;
            this.Timeout = timeout; 
        }
        public async Task<string> ResolveCaptcha(string googleSiteKey, string captchaUrl)
        {
            if(string.IsNullOrEmpty(APIKey) || string.IsNullOrEmpty(APISecret))
            {
                Logging.Logger.Write($"(CAPTCHA) - CaptchaSolutions API key or API Secret  not setup properly.", Logging.LogLevel.Error);

                return string.Empty;
            }

            string contentstring = $"p=nocaptcha&googlekey={googleSiteKey}&pageurl={captchaUrl}&key={this.APIKey}&secret={this.APISecret}&out=json";

            HttpContent content = new StringContent(contentstring);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(this.Timeout);

                try
                {
                    var responseContent = await client.PostAsync(API_ENDPOINT, content);
                    if (responseContent.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        Logging.Logger.Write($"(CAPTCHA) - Could not connect to solution captcha, please check your API config", Logging.LogLevel.Error);
                        return string.Empty;
                    }
                    var responseJSON = await responseContent.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<APIObjectResponse>(responseJSON);
                    return response.captchasolutions;

                }
                catch (Exception ex)
                {
                    Logging.Logger.Write($"(CAPTCHA) - Error occurred when solve captcha with Captcha Solutions");
                }
            }
            return string.Empty;
        }
    }
}
