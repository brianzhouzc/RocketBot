#region using directives

using System.Threading;
using System.Threading.Tasks;

#endregion

namespace PokemonGo.RocketBot.Logic.State
{
    public class VersionCheckState : IState
    {
        // reserve for auto updater
        public async Task<IState> Execute(ISession session, CancellationToken cancellationToken)
        {
            return new LoginState();
        }
    }
}