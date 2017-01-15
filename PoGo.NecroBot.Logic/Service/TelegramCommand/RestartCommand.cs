using PoGo.NecroBot.Logic.State;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class RestartCommand : ICommand
    {
        public string Command => "/restart";
        public string Description        =>  "Restart bot";
        public bool StopProcess => true;

        public async Task<bool> OnCommand(ISession session,string cmd, Action<string> Callback)
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
