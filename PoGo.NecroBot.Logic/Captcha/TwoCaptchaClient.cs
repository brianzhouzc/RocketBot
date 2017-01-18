using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Logging;

namespace PoGo.NecroBot.Logic.Captcha
{
    public class TwoCaptchaClient
    {
        public string APIKey { get; private set; }

        public TwoCaptchaClient(string apiKey)
        {
            APIKey = apiKey;
        }

        /// <summary>
        /// Sends a solve request and waits for a response
        /// </summary>
        /// <param name="googleKey">The "sitekey" value from site your captcha is located on</param>
        /// <param name="pageUrl">The page the captcha is located on</param>
        /// <param name="proxy">The proxy used, format: "username:password@ip:port</param>
        /// <param name="proxyType">The type of proxy used</param>
        /// <param name="result">If solving was successful this contains the answer</param>
        /// <returns>Returns true if solving was successful, otherwise false</returns>
        public async Task<string> SolveRecaptchaV2(string googleKey, string pageUrl, string proxy, ProxyType proxyType)
        {
            string requestUrl = "http://2captcha.com/in.php?key=" + APIKey + "&method=userrecaptcha&googlekey=" + googleKey + "&pageurl=" + pageUrl + "&proxy=" + proxy + "&proxytype=";

            switch (proxyType)
            {
                case ProxyType.HTTP:
                    requestUrl += "HTTP";
                    break;
                case ProxyType.HTTPS:
                    requestUrl += "HTTPS";
                    break;
                case ProxyType.SOCKS4:
                    requestUrl += "SOCKS4";
                    break;
                case ProxyType.SOCKS5:
                    requestUrl += "SOCKS5";
                    break;
            }

            try
            {
                WebRequest req = WebRequest.Create(requestUrl);

                using (WebResponse resp = req.GetResponse())
                using (StreamReader read = new StreamReader(resp.GetResponseStream()))
                {
                    string response = read.ReadToEnd();

                    if (response.Length < 3)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        if (response.Substring(0, 3) == "OK|")
                        {
                            string captchaID = response.Remove(0, 3);
                            Logger.Write($"Captcha has been sent to 2Captcha, Your captcha ID :  {captchaID}");

                            for (int i = 0; i < 29; i++)
                            {
                                WebRequest getAnswer = WebRequest.Create("http://2captcha.com/res.php?key=" + APIKey + "&action=get&id=" + captchaID);

                                using (WebResponse answerResp = getAnswer.GetResponse())
                                using (StreamReader answerStream = new StreamReader(answerResp.GetResponseStream()))
                                {
                                    string answerResponse = answerStream.ReadToEnd();

                                    if (answerResponse.Length < 3)
                                    {
                                        return string.Empty;
                                    }
                                    else
                                    {
                                        if (answerResponse.Substring(0, 3) == "OK|")
                                        {
                                            return answerResponse.Remove(0, 3);
                                        }
                                        else if (answerResponse != "CAPCHA_NOT_READY")
                                        {
                                            return string.Empty;
                                        }
                                    }
                                    Logger.Write($"Waiting response captcha from 2Captcha workers...");
                                }

                                await Task.Delay(3000);
                            }
                            return string.Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write($"2Captcha Error :  {ex.Message}");
            }
            return string.Empty;
        }
    }

    public enum ProxyType
    {
        HTTP,
        HTTPS,
        SOCKS4,
        SOCKS5
    }
}