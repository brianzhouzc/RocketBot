using PoGo.NecroBot.Logic.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class HelpCommand : ICommand
    {
        public string Command  => "/help";
        public string Description => "list all support command";
        public bool StopProcess => true;

        public async Task<bool> OnCommand(ISession session,string cmd, Action<string> Callback)
        {
            await Task.Delay(0); // Just added to get rid of compiler warning. Remove this if async code is used below.

            if (cmd.ToLower() == Command)
            {
                var type = typeof(ICommand);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => type.IsAssignableFrom(p) && p != type).OrderBy(p=>p.Name);
                string message = "";
                foreach (var item in types)
                {
                    ICommand telegramCMD =(ICommand) Activator.CreateInstance(item);
                    message += $"{telegramCMD.Command} - {telegramCMD.Description}\r\n";
                }

                Callback(message);
                return true;

            }
            return false;
        }
    }
}
