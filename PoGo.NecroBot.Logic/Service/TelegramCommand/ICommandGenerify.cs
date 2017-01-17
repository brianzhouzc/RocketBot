using System;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.State;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    interface ICommandGenerify<T> : ICommand
    {
        Task<bool> OnCommand(ISession session, string cmd, Action<T> callback);
    }
}