using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class RecycleCommand : ICommand
    {
        public string Command  => "/recycle";
        public string Description => "lets oder bot to recycle items";
        public bool StopProcess => true;

        public async Task<bool> OnCommand(ISession session,string commandText, Action<string> Callback)
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
