using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.State;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class RecycleCommand : CommandMessage
    {
        public override string Command => "/logs";
        public override string Description => "<n> send last n line in logs file, default 10 if not provide";
        public override bool StopProcess => true;

        public RecycleCommand(TelegramUtils telegramUtils) : base(telegramUtils)
        {
        }

        #pragma warning disable 1998 // added to get rid of compiler warning. Remove this if async code is used below.
        public override async Task<bool> OnCommand(ISession session,string commandText, Action<string> Callback)
        #pragma warning restore 1998
        {
            var cmd = commandText.Split(' ');

            if (cmd[0].ToLower() == Command)
            {
                // var fi = new FileInfo(Assembly.GetExecutingAssembly().Location);
                var logPath = "logs";

                DirectoryInfo di = new DirectoryInfo(logPath);
                var last = di.GetFiles().OrderByDescending(p => p.LastWriteTime).First();
                var alllines = File.ReadAllLines(last.FullName);
                int numberOfLines = 10;
                if (cmd.Length > 1)
                {
                    numberOfLines = Convert.ToInt32(cmd[1]);
                }
                var last10Lines = alllines.Skip(Math.Max(0, alllines.Length - numberOfLines));
                var message = "";
                foreach (var item in last10Lines)
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