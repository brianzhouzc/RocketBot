using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketBot.Window.WebSocketHandler
{
    interface IWebSocketResponce
    {
        string RequestID { get;  }
        string Command { get; }
        dynamic Data { get; }
    }
}
