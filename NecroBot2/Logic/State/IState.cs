#region using directives

using System.Threading;
using System.Threading.Tasks;

#endregion

namespace NecroBot2.Logic.State
{
    public interface IState
    {
        Task<IState> Execute(ISession session, CancellationToken cancellationToken);
    }
}