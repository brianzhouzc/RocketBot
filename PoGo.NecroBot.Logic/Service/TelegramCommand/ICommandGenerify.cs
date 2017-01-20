using System;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.State;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    internal interface ICommandGenerify<out T> : ICommand
    {
        Task<bool> OnCommand(ISession session, string cmd, Action<T> callback);
    }
}