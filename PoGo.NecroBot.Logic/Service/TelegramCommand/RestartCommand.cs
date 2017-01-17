using PoGo.NecroBot.Logic.State;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class RestartCommand : CommandMessage
    {
        public override string Command => "/restart";
        public override string Description        =>  "Restart bot";
        public override bool StopProcess => true;

        public RestartCommand(TelegramUtils telegramUtils) : base(telegramUtils)
        {
        }

        public override async Task<bool> OnCommand(ISession session,string cmd, Action<string> Callback)
        {
            if(cmd.ToLower() == Command)
            {
                Callback("Restarted Bot. Closing old Instance... BYE!");
                await Task.Delay(5000);
                Process.Start(Assembly.GetEntryAssembly().Location);

                Environment.Exit(-1);

                return true;
            }
            return false;
        }
    }
}
