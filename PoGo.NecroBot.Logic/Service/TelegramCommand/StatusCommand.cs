using PoGo.NecroBot.Logic.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class StatusCommand : ICommand
    {
        public string Command =>  "/status";
        public string Description =>  "Shows you the Status of the Bot.";
        public bool StopProcess => true;

        public async Task<bool> OnCommand(ISession session,string cmd, Action<string> Callback)
        {
            if(cmd.ToLower() == Command)
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
