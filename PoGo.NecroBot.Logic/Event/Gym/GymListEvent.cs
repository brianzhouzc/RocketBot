using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POGOProtos.Enums;
using POGOProtos.Map.Fort;

namespace PoGo.NecroBot.Logic.Event.Gym
{
    public class GymListEvent : IEvent
    {
        public List<FortData> Gyms { get; set; }
    }
}
