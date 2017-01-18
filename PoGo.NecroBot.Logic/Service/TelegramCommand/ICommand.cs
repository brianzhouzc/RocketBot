using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.State;
using Telegram.Bot.Types;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    internal interface ICommand
    {
        string Command { get; }
        string Arguments { get; }
        bool StopProcess { get; }
        TranslationString DescriptionI18NKey { get; }
        TranslationString MsgHeadI18NKey { get; }

        Task<bool> OnCommand(ISession session, string cmd, Message telegramMessage);

        string GetDescription(ISession session);
        string GetMsgHead(ISession session, object[] args = null);
    }
}