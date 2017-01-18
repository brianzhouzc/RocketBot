using System;
using System.Linq;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.State;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class HelpCommand : CommandMessage
    {
        public override string Command => "/help";
        public override bool StopProcess => true;
        public override TranslationString DescriptionI18NKey => TranslationString.TelegramCommandHelpDescription;
        public override TranslationString MsgHeadI18NKey => TranslationString.TelegramCommandHelpMsgHead;

        public HelpCommand(TelegramUtils telegramUtils) : base(telegramUtils)
        {
        }

        #pragma warning disable 1998 // added to get rid of compiler warning. Remove this if async code is used below.
        public override async Task<bool> OnCommand(ISession session, string cmd, Action<string> callback)
        #pragma warning restore 1998
        {
            if (cmd.ToLower() == Command)
            {
                string message = GetMsgHead(session, session.Profile.PlayerData.Username) + "\r\n\r\n";
                var iCommandInstances = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .Where(x => (typeof(ICommand).IsAssignableFrom(x)) && !x.IsInterface && !x.IsAbstract)
                    .Select(x => (ICommand) Activator.CreateInstance(x, TelegramUtils));

                foreach (var instance in iCommandInstances)
                {
                    var arguments = "";
                    if (!string.IsNullOrEmpty(instance.Arguments))
                    {
                        arguments = ' ' + instance.Arguments;
                    }
                    message += $"{instance.Command}{arguments} - {instance.GetDescription(session)}\r\n";
                }

                callback(message);
                return true;
            }
            return false;
        }
    }
}