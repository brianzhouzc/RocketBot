using System;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.State;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class ExitCommand : CommandMessage
    {
        public override string Command => "/exit";
        public override bool StopProcess => true;
        public override TranslationString DescriptionI18NKey => TranslationString.TelegramCommandExitDescription;
        public override TranslationString MsgHeadI18NKey => TranslationString.TelegramCommandExitMsgHead;

        public ExitCommand(TelegramUtils telegramUtils) : base(telegramUtils)
        {
        }

        public override async Task<bool> OnCommand(ISession session, string cmd, Action<string> callback)
        {
            if (cmd.ToLower() == Command)
            {
                callback(GetMsgHead(session, session.Profile.PlayerData.Username) + "\r\n\r\n");
                await Task.Delay(5000);
                Environment.Exit(0);
            }
            return false;
        }
    }
}