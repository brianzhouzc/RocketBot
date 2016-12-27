using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POGOProtos.Enums;

namespace PoGo.NecroBot.Logic.Event.Gym
{
    public class GymDetailInfoEvent : IEvent
    {
        public string Name { get; internal set; }
        public long Point { get; internal set; }
        public TeamColor Team { get; internal set; }
    }
}
