using System;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.State;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class ExitCommand : CommandMessage
    {
        public override string Command => "/exit";
        public override string Description => "Exit bot";
        public override bool StopProcess => true;

        public ExitCommand(TelegramUtils telegramUtils) : base(telegramUtils)
        {
        }

        public override async Task<bool> OnCommand(ISession session, string cmd, Action<string> Callback)
        {
            if (cmd.ToLower() == Command)
            {
                Callback("Closing Bot... BYE!");
                await Task.Delay(5000);
                Environment.Exit(0);
                return true;
            }
            return false;
        }
    }
}