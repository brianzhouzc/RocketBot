using PoGo.NecroBot.Logic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.State
{
    public class BlockableScope : IDisposable
    {
        private ISession session;

        private BotActions action;
        public BlockableScope(ISession session, BotActions action)
        {
            this.session = session;
            this.action = action;
        } 
        public async Task<bool> WaitToRun(int timeout = 60000)
        {
            return await session.WaitUntilActionAccept(action, timeout);
        }
        public void Dispose()
        {
            this.session.Actions.RemoveAll(x => x == this.action);
        }
    }
}
