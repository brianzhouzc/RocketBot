using PoGo.NecroBot.Logic.State;
using System;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    interface ICommandGenerify<T> : ICommand
    {
        Task<bool> OnCommand(ISession session, string cmd, Action<T> callback);
    }
}
