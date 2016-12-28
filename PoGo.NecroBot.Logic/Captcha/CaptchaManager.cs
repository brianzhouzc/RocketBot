using Google.Protobuf;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using PoGo.NecroBot.Logic.Captcha.Anti_Captcha;
using PoGo.NecroBot.Logic.Logging;
using PoGo.NecroBot.Logic.State;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static POGOProtos.Networking.Envelopes.Signature.Types;

namespace PoGo.NecroBot.Logic.Captcha
{
    public class CaptchaManager
    {
        const string POKEMON_GO_GOOGLE_KEY = "6LeeTScTAAAAADqvhqVMhPpr_vB9D364Ia-1dSgK";

        public static async Task<bool> SolveCaptcha(ISession session, string captchaUrl)
        {
            string captchaResponse = "";
            var cfg = session.LogicSettings.CaptchaConfig;
            bool resolved = false;
            bool needGetNewCaptcha = false;
            int retry = cfg.AutoCaptchaRetries;

            while (retry-- > 0 && !resolved)
            {
                //Use captcha solution to resolve captcha
                if (cfg.EnableCaptchaSolutions)
                {
                    if (needGetNewCaptcha)
                    {
                        captchaUrl = await GetNewCaptchaURL(session);
                    }

                    Logger.Write("Auto resolving captcha by using captcha solution service, please wait..........");
                    CaptchaSolutionClient client = new CaptchaSolutionClient(cfg.CaptchaSolutionAPIKey, cfg.CaptchaSolutionsSecretKey, cfg.AutoCaptchaTimeout);
                    captchaResponse = await client.ResolveCaptcha(POKEMON_GO_GOOGLE_KEY, captchaUrl);
                    needGetNewCaptcha = true;
                    if (!string.IsNullOrEmpty(captchaResponse))
                    {
                        resolved = await Resolve(session, captchaResponse);
                    }

                }

                //Anty captcha
                if (!resolved && session.LogicSettings.CaptchaConfig.EnableAntiCaptcha && !string.IsNullOrEmpty(session.LogicSettings.CaptchaConfig.AntiCaptchaAPIKey))
                {
                    if (needGetNewCaptcha)
                    {
                        captchaUrl = await GetNewCaptchaURL(session);
                    }
                    if (string.IsNullOrEmpty(captchaUrl)) return true;

                    Logger.Write("Auto resolving captcha by using anti captcha service");
                    captchaResponse = await GetCaptchaResposeByAntiCaptcha(session, captchaUrl);
                    needGetNewCaptcha = true;
                    if (!string.IsNullOrEmpty(captchaResponse))
                    {
                        resolved = await Resolve(session, captchaResponse);
                    }

                }

                //use 2 captcha
                if (!resolved && session.LogicSettings.CaptchaConfig.Enable2Captcha && !string.IsNullOrEmpty(session.LogicSettings.CaptchaConfig.TwoCaptchaAPIKey))
                {
                    if (needGetNewCaptcha)
                    {
                        captchaUrl = await GetNewCaptchaURL(session);

                    }
                    if (string.IsNullOrEmpty(captchaUrl)) return true;

                    Logger.Write("Auto resolving captcha by using 2Captcha service");
                    captchaResponse = await GetCaptchaResposeBy2Captcha(session, captchaUrl);
                    needGetNewCaptcha = true;
                    if (!string.IsNullOrEmpty(captchaResponse))
                    {
                        resolved = await Resolve(session, captchaResponse);
                    }
                }
            }
            
            //captchaRespose = "";
            if (!resolved)
            {
                if(needGetNewCaptcha)
                {
                    captchaUrl = await GetNewCaptchaURL(session);
                }

                if (session.LogicSettings.CaptchaConfig.PlaySoundOnCaptcha)
                {
                    SystemSounds.Asterisk.Play();
                }
                captchaResponse = await GetCaptchaResposeManually(session, captchaUrl);
                resolved = await Resolve(session, captchaResponse);
            }
           
            return resolved;
        }

        private static async Task<string> GetNewCaptchaURL(ISession session)
        {
            var res = await session.Client.Player.CheckChallenge();
            if(res.ShowChallenge )
            {
                return res.ChallengeUrl;
            }
            return string.Empty;
        }

        private static async Task<bool> Resolve(ISession session, string captchaRespose)
        {
            if (string.IsNullOrEmpty(captchaRespose)) return false;
            try
            {
                var verifyChallengeResponse = await session.Client.Player.VerifyChallenge(captchaRespose);
                if (!verifyChallengeResponse.Success)
                {
                    Logging.Logger.Write($"(CAPTCHA) Failed to resolve captcha, try resolved captcha by official app. ");
                    return false;
                }
                Logging.Logger.Write($"(CAPTCHA) Great!!! Captcha has been by passed", color: ConsoleColor.Green);
                return verifyChallengeResponse.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        private static async Task<string> GetCaptchaResposeByAntiCaptcha(ISession session, string captchaUrl)
        {
            bool solved = false;
            string result = null;

            {
                result = await AntiCaptchaClient.SolveCaptcha(captchaUrl,
                    POKEMON_GO_GOOGLE_KEY,
                    session.LogicSettings.CaptchaConfig.AntiCaptchaAPIKey,
                    session.LogicSettings.CaptchaConfig.ProxyHost,
                    session.LogicSettings.CaptchaConfig.ProxyPort);
                solved = !string.IsNullOrEmpty(result);
            }
            if (solved)
            {
                Logger.Write("Captcha has been resolved automatically by 2Captcha ");
            }
            return result;
        }


        private static async Task<string> GetCaptchaResposeBy2Captcha(ISession session, string captchaUrl)
        {
            bool solved = false;
            int retries = session.LogicSettings.CaptchaConfig.AutoCaptchaRetries;
            string result = null;

            while (retries-- >0  && !solved)
            {
                TwoCaptchaClient client = new TwoCaptchaClient(session.LogicSettings.CaptchaConfig.TwoCaptchaAPIKey);

                result = await client.SolveRecaptchaV2(POKEMON_GO_GOOGLE_KEY, captchaUrl, string.Empty, ProxyType.HTTP);
                solved = !string.IsNullOrEmpty(result);
            }
            if(solved)
            {
                Logger.Write("Captcha has been resolved automatically by 2Captcha ");
            }
            return result;
        }

        public static async Task<string> GetCaptchaResposeManually(ISession session, string url)
        {
            if (!session.LogicSettings.CaptchaConfig.AllowManualCaptchaResolve) return null;

            if(!File.Exists("chromedriver.exe"))
            {
                Logging.Logger.Write($"You enable manual captcha resolve but didn't setup properly, please download webdriver.exe put in same folder.", Logging.LogLevel.Error);
                return null;
            }
            IWebDriver webDriver = null;
            try
                
            {

                webDriver = new ChromeDriver(System.Environment.CurrentDirectory, new ChromeOptions() {
                    
                    });

                webDriver.Navigate().GoToUrl(url);
                Logging.Logger.Write($"Captcha is being show in separate thread window, please check your chrome browser and resolve it before {session.LogicSettings.CaptchaConfig.ManualCaptchaTimeout} seconds");

                var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(session.LogicSettings.CaptchaConfig.ManualCaptchaTimeout));
                //wait.Until(ExpectedConditions.ElementIsVisible(By.Id("recaptcha-verify-button")));
                wait.Until(d =>
                {
                    var ele = d.FindElement(By.Id("g-recaptcha-response"));

                    if (ele == null) return false;
                    return ele.GetAttribute("value").Length > 0;
                });

                string token = wait.Until<string>(driver =>
                {
                    var ele = driver.FindElement(By.Id("g-recaptcha-response"));
                    string t = ele.GetAttribute("value");
                    return t;
                });

                return token;
            }
            catch (Exception ex)
            {
                Logging.Logger.Write($"You didn't resolve captcha in the given time: {ex.Message} ", Logging.LogLevel.Error);
            }
            finally
            {
                if (webDriver != null) webDriver.Close();
            }

            return null;
        }
    }
}
