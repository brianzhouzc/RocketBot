using System.Collections.Generic;
using POGOProtos.Map.Fort;

namespace PoGo.NecroBot.Logic.Event.Gym
{
    public class GymListEvent : IEvent
    {
        public List<FortData> Gyms { get; set; }
    }
}