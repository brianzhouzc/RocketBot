using System.Threading.Tasks;
using PoGo.NecroBot.Logic.State;
using Telegram.Bot.Types;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    interface ICommand
    {
        string Command { get; }
        string Description { get; }
        bool StopProcess { get; }
        Task<bool> OnCommand(ISession session, string cmd, Message telegramMessage);
    }
}