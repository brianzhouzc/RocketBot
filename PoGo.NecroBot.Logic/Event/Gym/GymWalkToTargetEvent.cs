using POGOProtos.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POGOProtos.Networking.Responses;

namespace PoGo.NecroBot.Logic.Event.Gym
{
    public class GymWalkToTargetEvent : IEvent
    {
        public string Name { get; internal set; }
        public double  Distance{ get; internal set; }
        public double Longitude { get; internal set; }
        public double Latitude { get; internal set; }
    }
}
