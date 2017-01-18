using System;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.State;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class StatusCommand : CommandMessage
    {
        public override string Command => "/status";
        public override string Description => "Shows you the Status of the Bot.";
        public override bool StopProcess => true;

        public StatusCommand(TelegramUtils telegramUtils) : base(telegramUtils)
        {
        }

        #pragma warning disable 1998 // added to get rid of compiler warning. Remove this if async code is used below.
        public override async Task<bool> OnCommand(ISession session, string cmd, Action<string> Callback)
        #pragma warning restore 1998
        {
            if (cmd.ToLower() == Command)
            {
                var answerTextmessage = "";
                answerTextmessage += Console.Title;

                if (session.LogicSettings.UseCatchLimit)
                {
                    answerTextmessage += String.Format("\nCATCH LIMIT: {0}/{1}",
                        session.Stats.GetNumPokemonsInLast24Hours(),
                        session.LogicSettings.CatchPokemonLimit);
                }

                if (session.LogicSettings.UsePokeStopLimit)
                {
                    answerTextmessage += String.Format("\nPOKESTOP LIMIT: {0}/{1}",
                        session.Stats.GetNumPokestopsInLast24Hours(),
                        session.LogicSettings.PokeStopLimit);
                }

                Callback(answerTextmessage);
                return true;
            }
            return false;
        }
    }
}