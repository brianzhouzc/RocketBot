using System;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Tasks;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class RecycleCommand : CommandMessage
    {
        public override string Command => "/recycle";
        public override bool StopProcess => true;
        public override TranslationString DescriptionI18NKey => TranslationString.TelegramCommandRecycleDescription;
        public override TranslationString MsgHeadI18NKey => TranslationString.TelegramCommandRecycleMsgHead;

        public RecycleCommand(TelegramUtils telegramUtils) : base(telegramUtils)
        {
        }

        public override async Task<bool> OnCommand(ISession session, string commandText, Action<string> callback)
        {
            var cmd = commandText.Split(' ');

            if (cmd[0].ToLower() == Command)
            {
                await RecycleItemsTask.Execute(session, session.CancellationTokenSource.Token);
                callback(GetMsgHead(session, session.Profile.PlayerData.Username) + "\r\n\r\n");
                return true;
            }
            return false;
        }
    }
}