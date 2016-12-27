using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Captcha.Anti_Captcha
{
    public class AnticaptchaApiWrapper
    {
        public enum ProxyType
        {
            http
        }

        public static Dictionary<string, bool> HostsChecked = new Dictionary<string, bool>();

        public static bool CheckHost(string host)
        {
            if (!HostsChecked.ContainsKey(host))
            {
                HostsChecked[host] = Ping(host);
            }

            return HostsChecked[host];
        }

        private static dynamic JsonPostRequest(string host, string methodName, string postData)
        {
            return HttpHelper.Post(new Uri("http://" + host + "/" + methodName), postData);
        }

        public static bool Ping(string host)
        {
            try
            {
                new Ping().Send(host, 1000);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static AnticaptchaTask CreateNoCaptchaTaskProxyless(string host, string clientKey, string websiteUrl,
            string websiteKey, string userAgent, string websiteSToken = "")
        {
            return CreateNoCaptchaTask(
                "NoCaptchaTaskProxyless",
                host,
                clientKey,
                websiteUrl,
                websiteKey,
                null,
                null,
                null,
                null,
                null,
                userAgent,
                websiteSToken
                );
        }

        public static AnticaptchaTask CreateNoCaptchaTask(string host, string clientKey, string websiteUrl,
            string websiteKey, ProxyType proxyType, string proxyAddress, int proxyPort, string proxyLogin,
            string proxyPassword, string userAgent, string websiteSToken = "")
        {
            return CreateNoCaptchaTask(
                "NoCaptchaTask",
                host,
                clientKey,
                websiteUrl,
                websiteKey,
                proxyType,
                proxyAddress,
                proxyPort,
                proxyLogin,
                proxyPassword,
                userAgent,
                websiteSToken
                );
        }

        private static AnticaptchaTask CreateNoCaptchaTask(
            string type,
            string host,
            string clientKey,
            string websiteUrl,
            string websiteKey,
            ProxyType? proxyType,
            string proxyAddress,
            int? proxyPort,
            string proxyLogin,
            string proxyPassword,
            string userAgent,
            string websiteSToken = ""
            )
        {
            if (proxyType != null && (string.IsNullOrEmpty(proxyAddress) || !CheckHost(proxyAddress)))
            {
                throw new Exception("Proxy address is incorrect!");
            }

            if (proxyType != null && (proxyPort < 1 || proxyPort > 65535))
            {
                throw new Exception("Proxy port is incorrect!");
            }

            if (string.IsNullOrEmpty(userAgent))
            {
                throw new Exception("User-Agent is incorrect!");
            }

            if (string.IsNullOrEmpty(websiteUrl) || !websiteUrl.Contains(".") || !websiteUrl.Contains("/") ||
                !websiteUrl.Contains("http"))
            {
                throw new Exception("Website URL is incorrect!");
            }

            if (string.IsNullOrEmpty(websiteKey))
            {
                throw new Exception("Recaptcha Website Key is incorrect!");
            }

            var jObj = new JObject();

            jObj["softId"] = 2;
            jObj["clientKey"] = clientKey;
            jObj["task"] = new JObject();
            jObj["task"]["type"] = type;
            jObj["task"]["websiteURL"] = websiteUrl;
            jObj["task"]["websiteKey"] = websiteKey;
            jObj["task"]["websiteSToken"] = websiteSToken;
            jObj["task"]["userAgent"] = userAgent;

            if (proxyType != null)
            {
                jObj["task"]["proxyType"] = proxyType.ToString();
                jObj["task"]["proxyAddress"] = proxyAddress;
                jObj["task"]["proxyPort"] = proxyPort;
                jObj["task"]["proxyLogin"] = proxyLogin;
                jObj["task"]["proxyPassword"] = proxyPassword;
            }

            try
            {
                var resultJson = JsonPostRequest(host, "createTask",
                    JsonConvert.SerializeObject(jObj, Formatting.Indented));

                int? taskId = null;
                int? errorId = null;
                string errorCode = null;
                string errorDescription = null;

                try
                {
                    taskId = int.Parse(resultJson.taskId.ToString());
                }
                catch
                {
                    // ignored
                }

                try
                {
                    errorId = int.Parse(resultJson.errorId.ToString());
                }
                catch
                {
                    // ignored
                }

                try
                {
                    errorCode = resultJson.errorCode.ToString();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    errorDescription = resultJson.errorDescription.ToString();
                }
                catch
                {
                    // ignored
                }

                return new AnticaptchaTask(
                    taskId,
                    errorId,
                    errorCode,
                    errorDescription
                    );
            }
            catch
            {
                // ignored
            }

            return null;
        }

        private static string ImagePathToBase64String(string path)
        {
            try
            {
                using (var image = Image.FromFile(path))
                {
                    using (var m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        var imageBytes = m.ToArray();

                        // Convert byte[] to Base64 String
                        var base64String = Convert.ToBase64String(imageBytes);

                        return base64String;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Creates "image to text" task and sends it to anti-captcha.com
        /// </summary>
        /// <param name="host"></param>
        /// <param name="clientKey"></param>
        /// <param name="pathToImageOrBase64Body">You can set just a path to your image in the filesystem or a base64-encoded image</param>
        /// <param name="phrase"></param>
        /// <param name="_case"></param>
        /// <param name="numeric"></param>
        /// <param name="math"></param>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <returns>AnticaptchaTask with taskId or error information</returns>
        public static AnticaptchaTask CreateImageToTextTask(string host, string clientKey,
            string pathToImageOrBase64Body,
            bool? phrase = null, bool? _case = null, int? numeric = null,
            bool? math = null, int? minLength = null, int? maxLength = null)
        {
            try
            {
                if (File.Exists(pathToImageOrBase64Body))
                {
                    pathToImageOrBase64Body = ImagePathToBase64String(pathToImageOrBase64Body);
                }
            }
            catch
            {
                // ignored
            }

            var jObj = new JObject();

            jObj["softId"] = 2;
            jObj["clientKey"] = clientKey;
            jObj["task"] = new JObject();
            jObj["task"]["type"] = "ImageToTextTask";
            jObj["task"]["body"] = pathToImageOrBase64Body.Replace("\r", "").Replace("\n", "").Trim();

            if (phrase != null)
            {
                jObj["task"]["phrase"] = phrase;
            }

            if (_case != null)
            {
                jObj["task"]["case"] = _case;
            }

            if (numeric != null)
            {
                jObj["task"]["numeric"] = numeric;
            }

            if (math != null)
            {
                jObj["task"]["math"] = math;
            }

            if (minLength != null)
            {
                jObj["task"]["minLength"] = minLength;
            }

            if (maxLength != null)
            {
                jObj["task"]["maxLength"] = maxLength;
            }

            try
            {
                var resultJson = JsonPostRequest(
                    host,
                    "createTask",
                    JsonConvert.SerializeObject(jObj, Formatting.Indented)
                    );

                int? taskId = null;
                int? errorId = null;
                string errorCode = null;
                string errorDescription = null;

                try
                {
                    taskId = int.Parse(resultJson.taskId.ToString());
                }
                catch
                {
                    // ignored
                }

                try
                {
                    errorId = int.Parse(resultJson.errorId.ToString());
                }
                catch
                {
                    // ignored
                }

                try
                {
                    errorCode = resultJson.errorCode.ToString();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    errorDescription = resultJson.errorDescription.ToString();
                }
                catch
                {
                    // ignored
                }

                return new AnticaptchaTask(
                    taskId,
                    errorId,
                    errorCode,
                    errorDescription
                    );
            }
            catch
            {
                // ignored
            }

            return null;
        }

        public static AnticaptchaResult GetTaskResult(string host, string clientKey, AnticaptchaTask task)
        {
            var jObj = new JObject();

            jObj["clientKey"] = clientKey;
            jObj["taskId"] = task.GetTaskId();

            try
            {
                dynamic resultJson = JsonPostRequest(host, "getTaskResult",
                    JsonConvert.SerializeObject(jObj, Formatting.Indented));

                var status = AnticaptchaResult.Status.unknown;

                try
                {
                    status = resultJson.status.ToString().Equals("ready")
                        ? AnticaptchaResult.Status.ready
                        : AnticaptchaResult.Status.processing;
                }
                catch
                {
                    // ignored
                }

                string solution;
                int? errorId = null;
                string errorCode = null;
                string errorDescription = null;
                double? cost = null;
                string ip = null;
                int? createTime = null;
                int? endTime = null;
                int? solveCount = null;

                try
                {
                    solution = resultJson.solution.gRecaptchaResponse.ToString();
                }
                catch
                {
                    try
                    {
                        solution = resultJson.solution.text.ToString();
                    }
                    catch
                    {
                        solution = null;
                    }
                }

                try
                {
                    errorId = resultJson.errorId;
                }
                catch
                {
                    // ignored
                }

                try
                {
                    errorCode = resultJson.errorCode;
                }
                catch
                {
                    // ignored
                }

                try
                {
                    errorDescription = resultJson.errorDescription;
                }
                catch
                {
                    // ignored
                }

                try
                {
                    cost = double.Parse(resultJson.cost.ToString().Replace(',', '.'), CultureInfo.InvariantCulture);
                }
                catch
                {
                    // ignored
                }

                try
                {
                    createTime = resultJson.createTime;
                }
                catch
                {
                    // ignored
                }

                try
                {
                    endTime = resultJson.endTime;
                }
                catch
                {
                    // ignored
                }

                try
                {
                    solveCount = resultJson.solveCount;
                }
                catch
                {
                    // ignored
                }

                try
                {
                    ip = resultJson.ip;
                }
                catch
                {
                    // ignored
                }

                return new AnticaptchaResult(
                    status,
                    solution,
                    errorId,
                    errorCode,
                    errorDescription,
                    cost,
                    ip,
                    createTime,
                    endTime,
                    solveCount
                    );
            }
            catch
            {
                // ignored
            }

            return null;
        }
    }
}
