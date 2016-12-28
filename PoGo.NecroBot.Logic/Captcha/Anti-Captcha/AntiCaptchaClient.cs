using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Captcha.Anti_Captcha
{
    public class AntiCaptchaClient
    {
        private const string Host = "api.anti-captcha.com";
        private const string ClientKey = "xxxxxxx";
        private const string ProxyHost = "xx.xx.xx.xx";
        private const int ProxyPort = 8282;
        private const string ProxyLogin = "";
        private const string ProxyPassword = "";

        private const string UserAgent =
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";


        public static async Task<string> SolveCaptcha(string captchaURL, string apiKey, string googleSiteKey, string proxyHost, int proxyPort, string proxyAccount = "", string proxyPassword = "")
        {

            var task1 = AnticaptchaApiWrapper.CreateNoCaptchaTask(
                Host,
                apiKey,
                captchaURL,     //target website address
                googleSiteKey,         //target website Recaptcha key
                AnticaptchaApiWrapper.ProxyType.http,
                proxyHost,                                     //ipv4 or ipv6 proxy address
                proxyPort,                                               //proxy port
                "",                                            //proxy login              
                "",                                         //proxy password
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36"
                );

            return await ProcessTask(task1);
        }

        private static async Task<string> ProcessTask(AnticaptchaTask task)
        {
            AnticaptchaResult response;

            do
            {
                response = AnticaptchaApiWrapper.GetTaskResult(Host, ClientKey, task);

                if (response.GetStatus().Equals(AnticaptchaResult.Status.ready))
                {
                    break;
                }

                Console.WriteLine("Not done yet, waiting...");
                await Task.Delay(1000);
            } while (response != null && response.GetStatus().Equals(AnticaptchaResult.Status.processing));

            if (response == null || response.GetSolution() == null)
            {
                Console.WriteLine("Unknown error occurred...");
                Console.WriteLine("Response dump:");
                Console.WriteLine(response);
            }
            else
            {
                Console.WriteLine("The answer is '" + response.GetSolution() + "'");
            }

            return response.GetSolution();
        }
    }

}

