using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.State;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class LogsCommand : CommandMessage
    {
        private const int DeafultLogEntries = 10;

        public override string Command => "/logs";
        public override string Arguments => "[n]";
        public override bool StopProcess => true;
        public override TranslationString DescriptionI18NKey => TranslationString.TelegramCommandLogsDescription;
        public override TranslationString MsgHeadI18NKey => TranslationString.TelegramCommandLogsMsgHead;

        public LogsCommand(TelegramUtils telegramUtils) : base(telegramUtils)
        {
        }

        public override string GetDescription(ISession session) =>
            base.GetDescription(session, DeafultLogEntries.ToString());

        #pragma warning disable 1998 // added to get rid of compiler warning. Remove this if async code is used below.
        public override async Task<bool> OnCommand(ISession session,string commandText, Action<string> callback)
        #pragma warning restore 1998
        {
            var cmd = commandText.Split(' ');

            if (cmd[0].ToLower() == Command)
            {
                // var fi = new FileInfo(Assembly.GetExecutingAssembly().Location);
                const string logPath = "logs";

                var logDirectoryHandle = new DirectoryInfo(logPath);
                var last = logDirectoryHandle.GetFiles().OrderByDescending(p => p.LastWriteTime).First();
                var alllines = File.ReadAllLines(last.FullName);
                var numberOfLines = DeafultLogEntries;
                if (cmd.Length > 1)
                {
                    numberOfLines = Convert.ToInt32(cmd[1]);
                }
                var last10Lines = (alllines.Skip(Math.Max(0, alllines.Length - numberOfLines))) ;
                var enumerable = last10Lines as string[] ?? last10Lines.ToArray();

                var message = GetMsgHead(session, session.Profile.PlayerData.Username, enumerable.Length) + "\r\n\r\n";
                message = enumerable.Aggregate(message, (current, item) => current + (item + "\r\n"));
                callback(message);
                return true;
            }
            return false;
        }
    }
}