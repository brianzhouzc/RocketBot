using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Tasks;
using System;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class LogsCommand : CommandMessage
    {
        public override string Command  => "/recycle";
        public override string Description => "lets oder bot to recycle items";
        public override bool StopProcess => true;

        public LogsCommand(TelegramUtils telegramUtils) : base(telegramUtils)
        {
        }

        public override async Task<bool> OnCommand(ISession session,string commandText, Action<string> Callback)
        {
            var cmd = commandText.Split(' ');

            if (cmd[0].ToLower() == Command)
            {
                await RecycleItemsTask.Execute(session, session.CancellationTokenSource.Token);
                Callback("RECYCLE ITEM DONE!");
                return true;

            }
            return false;
        }
    }
}
