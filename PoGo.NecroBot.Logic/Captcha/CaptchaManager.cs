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
            string captchaRespose = "";
            var cfg = session.LogicSettings.CaptchaConfig;

            if (cfg.EnableCaptchaSolutions)
            {
                Logger.Write("Auto resolving captcha by using captcha solution service, please wait..........");
                CaptchaSolutionClient client = new CaptchaSolutionClient(cfg.CaptchaSolutionAPIKey, cfg.CaptchaSolutionsSecretKey, cfg.AutoCaptchaTimeout);
                captchaRespose = await client.ResolveCaptcha(POKEMON_GO_GOOGLE_KEY, captchaUrl);
                //captchaRespose = await GetCaptchaResposeBy2Captcha(session, captchaUrl);
            }

            if (session.LogicSettings.CaptchaConfig.EnableAntiCaptcha && !string.IsNullOrEmpty(session.LogicSettings.CaptchaConfig.AntiCaptchaAPIKey))
            {
                Logger.Write("Auto resolving captcha by using anti captcha service");
                captchaRespose = await GetCaptchaResposeByAntiCaptcha(session, captchaUrl);
            }

            if (session.LogicSettings.CaptchaConfig.Enable2Captcha && !string.IsNullOrEmpty(session.LogicSettings.CaptchaConfig.TwoCaptchaAPIKey))
            {
                Logger.Write("Auto resolving captcha by using 2Captcha service");
                captchaRespose = await GetCaptchaResposeBy2Captcha (session, captchaUrl);
            }

            
            //captchaRespose = "";
            if (string.IsNullOrEmpty(captchaRespose))
            {

                if (session.LogicSettings.CaptchaConfig.PlaySoundOnCaptcha)
                {
                    SystemSounds.Asterisk.Play();
                }
                captchaRespose = await GetCaptchaResposeManually(session, captchaUrl);

            }

            bool resolved = false;
            try
            {
                resolved = await Resolve(session, captchaRespose);
            }
            catch(Exception ex )
            {
                Console.WriteLine(ex.Message);
            }
            return resolved;
        }

        private static async Task<bool> Resolve(ISession session, string captchaRespose)
        {
            if (string.IsNullOrEmpty(captchaRespose)) return false;
            /*
       var deviceInfo = new DeviceInfo()
       {
           //AndroidBoardName = session.Settings.AndroidBoardName,
           //AndroidBootloader = session.Settings.AndroidBootloader,
           DeviceBrand = session.Settings.DeviceBrand,
           DeviceId = session.Settings.DeviceId,
           DeviceModel = session.Settings.DeviceModel,
           DeviceModelBoot = session.Settings.DeviceModelBoot,
           //DeviceModelIdentifier = session.Settings.DeviceModelIdentifier,
           FirmwareBrand = session.Settings.FirmwareBrand,
           //FirmwareFingerprint = session.Settings.FirmwareFingerprint,
           // FirmwareTags = session.Settings.FirmwareTags,
           FirmwareType = session.Settings.FirmwareType,
           HardwareManufacturer = session.Settings.HardwareManufacturer,
           HardwareModel = session.Settings.HardwareManufacturer
       };
       ILoginProvider loginProvider = null;
       if (session.Settings.AuthType == PokemonGo.RocketAPI.Enums.AuthType.Ptc)
       {
           loginProvider = new PtcLoginProvider(session.Settings.PtcUsername, session.Settings.PtcPassword);
       }
       else
       {
           loginProvider = new GoogleLoginProvider(session.Settings.GoogleUsername, session.Settings.GooglePassword);
       }

       var newSession = await GetSession(loginProvider, session.Client.CurrentLatitude, session.Client.CurrentLongitude, true, deviceInfo);

       await Task.Delay(5000);
       //session.Client.SetCaptchaToken(token);
       //var verified = session.Client.Player.VerifyChallenge(token.Trim()).Result;
       var verified = newSession.RpcClient.SendRemoteProcedureCallAsync(new POGOProtos.Networking.Requests.Request()
       {
           RequestType = POGOProtos.Networking.Requests.RequestType.VerifyChallenge,
           RequestMessage = (new VerifyChallengeMessage()
           {
               Token =  captchaRespose
           }).ToByteString()
       }).Result;


       var vres = VerifyChallengeResponse.Parser.ParseFrom(verified);
*/
            var verifyChallengeResponse = await session.Client.Player.VerifyChallenge(captchaRespose);
            if (!verifyChallengeResponse.Success)
            {
                Logging.Logger.Write($"(CAPTCHA) Failed to resolve captcha, try resolved captcha by official app. ");
                return false;
            }
            Logging.Logger.Write($"(CAPTCHA) Great!!! Captcha has been by passed", color:ConsoleColor.Green);
            return verifyChallengeResponse.Success;

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
