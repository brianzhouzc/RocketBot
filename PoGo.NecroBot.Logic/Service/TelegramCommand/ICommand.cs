using PoGo.NecroBot.Logic.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    interface ICommand
    {
        string Command { get; }
        string Description { get; }
        bool   StopProcess { get;  }
        Task<bool> OnCommand(ISession session, string cmd, Action<string> cb);
    }
}
