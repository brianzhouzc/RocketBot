using PoGo.NecroBot.Logic.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class LogsCommand : ICommand
    {
        public string Command  => "/logs";
        public string Description => "<n> send last n line in logs file, default 10 if not provide";
        public bool StopProcess => true;

        public async Task<bool> OnCommand(ISession session,string commandText, Action<string> Callback)
        {
            var cmd = commandText.Split(' ');

            if (cmd[0].ToLower() == Command)
            {
               // var fi = new FileInfo(Assembly.GetExecutingAssembly().Location);
                var logPath ="logs";

                DirectoryInfo di = new DirectoryInfo(logPath);
                var last = di.GetFiles().OrderByDescending(p => p.LastWriteTime).First();
                var alllines = File.ReadAllLines(last.FullName);
                int numberOfLines = 10;
                if(cmd.Length>1)
                {
                    numberOfLines = Convert.ToInt32(cmd[1]);
                }
                var last10lines = alllines.Skip(Math.Max(0,alllines.Length - numberOfLines));
                var message = "";
                foreach (var item in last10lines)
                {
                    message += item + "\r\n";
                }
                Callback(message);
                return true;

            }
            return false;
        }
    }
}
