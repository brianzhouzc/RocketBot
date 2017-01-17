#region using directives

using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.Service.TelegramCommand;
using PoGo.NecroBot.Logic.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

#endregion

namespace PoGo.NecroBot.Logic.Service
{
    public class TelegramService
    {
        private DateTime _lastLoginTime;
        private readonly TelegramUtils telegramUtils;
        private bool _loggedIn;
        private readonly ISession _session;
        private readonly TelegramBotClient _bot;
        private IEnumerable<ICommand> iCommandInstances;
        private long lastChatId = 0;
        private const string LOG_FILE = "config\\telegram.id";

        public TelegramService(string apiKey, ISession session)
        {
            try
            {
                _session = session;
                _bot = new TelegramBotClient(apiKey);
                telegramUtils = new TelegramUtils(_bot, _session);

                iCommandInstances = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .Where(x => (typeof(ICommand).IsAssignableFrom(x)) && !x.IsInterface && !x.IsAbstract)
                    .Select(x => (ICommand)Activator.CreateInstance(x, telegramUtils));

                var me = _bot.GetMeAsync().Result;

                _bot.OnMessage += OnTelegramMessageReceived;
                _bot.StartReceiving();

                _session.EventDispatcher.Send(new NoticeEvent { Message = "Using TelegramAPI with " + me.Username });

                if (File.Exists(LOG_FILE))
                {
                    var s = File.ReadAllText(LOG_FILE);
                    if (!string.IsNullOrEmpty(s))
                    {
                        lastChatId = Convert.ToInt64(s);
                        #pragma warning disable 4014 // disables 'await not used warning' - since c'tor is not async by itself we can not use the async statement for method calls
                        telegramUtils.SendMessage(_session.Translation.GetTranslation(TranslationString.TelegramBotStarted), lastChatId);
                    }
                    else
                    {
                        _session.EventDispatcher.Send(new NoticeEvent() { Message = _session.Translation.GetTranslation(TranslationString.TelegramNeedChatId) });
                    }

                }
            }
            catch (Exception ex)
            {
                _session.EventDispatcher.Send(new ErrorEvent { Message = ex.Message });
                _session.EventDispatcher.Send(new ErrorEvent { Message = "Unkown Telegram Error occured. " });
            }
        }

        private async void OnTelegramMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            if (message == null || message.Type != MessageType.TextMessage)
                return;

            var answerTextmessage = "";

            if (_session.Profile == null || _session.Inventory == null)
            {
                return;
            }
            lastChatId = message.Chat.Id;
            File.WriteAllText(LOG_FILE, lastChatId.ToString());
            var messagetext = message.Text.ToLower().Split(' ');

            if (!_loggedIn && messagetext[0].ToLower().Contains("/login"))
            {
                if (messagetext.Length == 2)
                {
                    if (messagetext[1].ToLower().Contains(_session.LogicSettings.TelegramPassword))
                    {
                        _loggedIn = true;
                        _lastLoginTime = DateTime.Now;
                        answerTextmessage += _session.Translation.GetTranslation(TranslationString.LoggedInTelegram);
                        await telegramUtils.SendMessage(answerTextmessage, message.Chat.Id);
                        return;
                    }
                    answerTextmessage += _session.Translation.GetTranslation(TranslationString.LoginFailedTelegram);
                    await telegramUtils.SendMessage(answerTextmessage, message.Chat.Id);
                    return;
                }
                answerTextmessage += _session.Translation.GetTranslation(TranslationString.NotLoggedInTelegram);
                await telegramUtils.SendMessage(answerTextmessage, message.Chat.Id);
                return;
            }
            if (_loggedIn)
            {
                if (_lastLoginTime.AddMinutes(5).Ticks < DateTime.Now.Ticks)
                {
                    _loggedIn = false;
                    answerTextmessage += _session.Translation.GetTranslation(TranslationString.NotLoggedInTelegram);
                    await telegramUtils.SendMessage(answerTextmessage, message.Chat.Id);
                    return;
                }
                var remainingMins = _lastLoginTime.AddMinutes(5).Subtract(DateTime.Now).Minutes;
                var remainingSecs = _lastLoginTime.AddMinutes(5).Subtract(DateTime.Now).Seconds;
                answerTextmessage += _session.Translation.GetTranslation(TranslationString.LoginRemainingTime,
                    remainingMins, remainingSecs);
                await telegramUtils.SendMessage(answerTextmessage, message.Chat.Id);
                return;
            }

            bool handled = false;
            foreach (var item in iCommandInstances)
            {
                try
                {
                    handled = await item.OnCommand(_session, message.Text, message);
                    if (handled) break;
                }
                catch (Exception)
                {
                }

            }

            if (!handled)
            {
                HelpCommand helpCMD = new HelpCommand(telegramUtils);
                await helpCMD.OnCommand(_session, helpCMD.Command, message);
            }
        }

        [Obsolete("SendMessage in TelegramService.cs is deprecated, please use SendMessage.cs TelegramUtils.")]
        public async Task SendMessage(string message)
        {
            if (string.IsNullOrEmpty(message) || lastChatId == 0) return;
            await _bot.SendTextMessageAsync(lastChatId, message, replyMarkup: new ReplyKeyboardHide());
        }
    }
}