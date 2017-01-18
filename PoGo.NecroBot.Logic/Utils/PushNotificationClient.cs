using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.Model.Settings;
using PoGo.NecroBot.Logic.State;

namespace PoGo.NecroBot.Logic.Utils
{
    //somehow, we can add PushBulletSharp in project, that is a simple client to just for send note message,
    public class PushNotificationClient
    {
        private static void HandleEvent(ErrorEvent errorEvent, ISession session)
        {
            //SendPushNotificationV2("Error occured", errorEvent.Message);
        }

        private static StreamContent AddContent(Stream stream, string filename)
        {
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"file\"",
                FileName = "\"" + filename + "\""
            };
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            return fileContent;
        }

        private static StringContent addContent(string name, string content)
        {
            var fileContent = new StringContent(content);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"" + name + "\""
            };
            return fileContent;
        }

        public static async Task SendMailNotification(NotificationConfig cfg, string title, string body)
        {
            await Task.Run(() =>
            {
                var fromAddress = new MailAddress(cfg.GmailUsername, "Necrobot Notifier");
                //var toAddress = new MailAddress(cfg.Recipients);

                string fromPassword = cfg.GmailPassword;

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using (var message = new MailMessage()
                {
                    Subject = title,
                    Body = body
                })
                {
                    message.From = fromAddress;
                    foreach (var item in cfg.Recipients.Split(';'))
                    {
                        message.To.Add(item);
                    }
                    smtp.Send(message);
                }
            });
        }

        public static async Task SendNotification(ISession session, string title, string body, bool push = false)
        {
            var cfg = session.LogicSettings.NotificationConfig;
            try
            {
                if (cfg.EnableEmailNotification)
                {
                    await SendMailNotification(cfg, title, body);
                }

                if (push)
                {
                    if (cfg.EnablePushBulletNotification)
                    {
                        await SendPushNotificationV2(cfg.PushBulletApiKey, title, body);
                    }

                    // TODO function is deprecated / obsolete
                    await session.Telegram.SendMessage($"{title}\r\n{body}");
                }
            }
            catch (Exception)
            {
                session.EventDispatcher.Send(new WarnEvent()
                {
                    Message = session.Translation.GetTranslation(TranslationString.FailedSendNotification)
                });
            }
        }

        public static async Task<bool> SendPushNotificationV2(string apiKey, string title, string body)
        {
            bool isSusccess = false;

            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(apiKey, "")
            };

            // string name = Path.GetFileName(pathFile);
            using (var wc = new HttpClient(handler))
            {
                using (var multiPartCont = new MultipartFormDataContent())
                {
                    multiPartCont.Add(addContent("type", "note"));
                    multiPartCont.Add(addContent("title", title));
                    multiPartCont.Add(addContent("body", body));
                    //multiPartCont.Add(AddContent(new FileStream(pathFile, FileMode.Open), name));

                    try
                    {
                        var resp = wc.PostAsync("https://api.pushbullet.com/v2/pushes", multiPartCont);
                        var result = await resp.Result.Content.ReadAsStringAsync();
                        isSusccess = true;
                    }
                    catch (Exception ex)
                    {
                        isSusccess = false;
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return isSusccess;
        }
    }
}