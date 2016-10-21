#region using directives

using System.Threading;
using System.Threading.Tasks;

#endregion

namespace PokemonGo.RocketBot.Logic.State
{
    public class VersionCheckState : IState
    {
        // reserve for auto updater
#pragma warning disable CS1998 // Cette méthode async n'a pas d'opérateur 'await' et elle s'exécutera de façon synchrone
        public async Task<IState> Execute(ISession session, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Cette méthode async n'a pas d'opérateur 'await' et elle s'exécutera de façon synchrone
        {
            return new LoginState();
        }
    }
}