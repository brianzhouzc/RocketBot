using System;
using System.Device.Location;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.State;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace PoGo.NecroBot.Logic.Service
{
    public class TelegramUtils
    {
        private readonly TelegramBotClient bot;
        private readonly ISession session;

        public TelegramUtils(TelegramBotClient bot, ISession session)
        {
            this.bot = bot;
            this.session = session;
        }

        public async Task SendLocation(GeoCoordinate geo, Message telegramMessage)
        {
            await SendLocation(geo, telegramMessage.MessageId);
        }

        public async Task SendLocation(GeoCoordinate geo, long chatId)
        {
            if (chatId == 0)
            {
                session.EventDispatcher.Send(new WarnEvent { Message = String.Format("Could not send location to 'Telegram', because given Chat id was '{0}'", 0) });
            }
            await bot.SendLocationAsync(chatId, (float) geo.Latitude, (float) geo.Longitude);
        }

        public async Task SendMessage(string message, Message telegramMessage)
        {
            await SendMessage(message, telegramMessage.MessageId);
        }

        public async Task SendMessage(string message, long chatId)
        {
            if (chatId == 0)
            {
                session.EventDispatcher.Send(new WarnEvent { Message = String.Format("Could not send message to 'Telegram', because given Chat id was '{0}'", 0) });
            }
            else if (string.IsNullOrEmpty(message))
            {
                return;
            }

            await bot.SendTextMessageAsync(chatId, message, replyMarkup: new ReplyKeyboardHide());
        }
    }
}