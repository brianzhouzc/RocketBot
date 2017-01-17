using System;
using System.Linq;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.State;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class HelpCommand : CommandMessage
    {
        public override string Command => "/help";
        public override string Description => "list all support command";
        public override bool StopProcess => true;

        public HelpCommand(TelegramUtils telegramUtils) : base(telegramUtils)
        {
        }

        #pragma warning disable 1998 // added to get rid of compiler warning. Remove this if async code is used below.
        public override async Task<bool> OnCommand(ISession session, string cmd, Action<string> Callback)
        #pragma warning restore 1998
        {
            if (cmd.ToLower() == Command)
            {
                var message = "";
                var iCommandInstances = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .Where(x => (typeof(ICommand).IsAssignableFrom(x)) && !x.IsInterface && !x.IsAbstract)
                    .Select(x => (ICommand) Activator.CreateInstance(x, telegramUtils));

                foreach (var instance in iCommandInstances)
                {
                    message += $"{((ICommand) instance).Command} - {((ICommand) instance).Description}\r\n";
                }

                Callback(message);
                return true;
            }
            return false;
        }
    }
}