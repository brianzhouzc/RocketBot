using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.PoGoUtils;
using PoGo.NecroBot.Logic.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class AccountsCommand : ICommand
    {
        public string Command  =>"/accounts";
        public string Description => "List all account setup for multiple bot";
        public bool StopProcess => true;

        public async Task<bool> OnCommand(ISession session,string cmd, Action<string> Callback)
        {
            string[] messagetext = cmd.Split(' ');

            string message = "";
            if(messagetext[0].ToLower() == Command)
            {
                if (session.LogicSettings.AllowMultipleBot)
                {
                    foreach (var item in session.Accounts)
                    {
                        int day = (int)item.RuntimeTotal / 1440;
                        int hour = (int)(item.RuntimeTotal - (day * 1400)) / 60;
                        int min = (int)(item.RuntimeTotal - (day * 1400) - hour * 60);

                        message = message + $"{item.GoogleUsername}{item.PtcUsername}     {day:00}:{hour:00}:{min:00}:00\r\n";
                    }
                }
                else
                {
                    message = message + "Multiple bot is disabled. please use /profile for current account details";
                }
                Callback(message);
                return true;

            }
            return false;
        }
    }
}
