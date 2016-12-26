#region using directives

using System.Threading;
using System.Threading.Tasks;

#endregion

namespace PoGo.NecroBot.Logic.Forms_Gui.State
{
    public interface IState
    {
        Task<IState> Execute(ISession session, CancellationToken cancellationToken);
    }
}