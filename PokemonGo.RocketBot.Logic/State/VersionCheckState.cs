#region using directives

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PokemonGo.RocketBot.Logic.Common;
using PokemonGo.RocketBot.Logic.Event;
using PokemonGo.RocketBot.Logic.Logging;
using System.Collections.Generic;
using PokemonGo.RocketBot.Logic.Utils;

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