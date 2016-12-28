#region using directives

using System.Threading;
using System.Threading.Tasks;
using NecroBot2.Logic.Tasks;

#endregion

namespace NecroBot2.Logic.State
{
    public class InfoState : IState
    {
        public async Task<IState> Execute(ISession session, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await DisplayPokemonStatsTask.Execute(session);
            return new FarmState();
        }
    }
}