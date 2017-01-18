using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.State;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class RestartCommand : CommandMessage
    {
        public override string Command => "/restart";
        public override bool StopProcess => true;
        public override TranslationString DescriptionI18NKey => TranslationString.TelegramCommandRestartDescription;
        public override TranslationString MsgHeadI18NKey => TranslationString.TelegramCommandRestartMsgHead;

        public RestartCommand(TelegramUtils telegramUtils) : base(telegramUtils)
        {
        }

        public override async Task<bool> OnCommand(ISession session, string cmd, Action<string> callback)
        {
            if (cmd.ToLower() == Command)
            {
                callback(GetMsgHead(session, session.Profile.PlayerData.Username) + "\r\n\r\n");
                await Task.Delay(5000);
                var assembly = Assembly.GetEntryAssembly().Location;
                if (assembly != null)
                {
                    Process.Start(assembly);
                }

                Environment.Exit(-1);
            }
            return false;
        }
    }
}