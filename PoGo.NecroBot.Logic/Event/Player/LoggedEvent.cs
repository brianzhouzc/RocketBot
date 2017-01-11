using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POGOProtos.Networking.Responses;

namespace PoGo.NecroBot.Logic.Event.Player
{
    public class LoggedEvent : IEvent
    {
        public GetPlayerResponse Profile { get; internal set; }
    }
}
