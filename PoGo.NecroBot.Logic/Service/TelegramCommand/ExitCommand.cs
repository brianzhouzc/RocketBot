using PoGo.NecroBot.Logic.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class ExitCommand : ICommand
    {
        public string Command => "/exit";
        public string Description        =>  "Exit bot";
        public bool StopProcess => true;

        public async Task<bool> OnCommand(ISession session,string cmd, Action<string> Callback)
        {
            if(cmd.ToLower() == Command)
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
