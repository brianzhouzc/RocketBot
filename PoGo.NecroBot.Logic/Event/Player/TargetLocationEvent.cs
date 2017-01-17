using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Event.Player
{
    public class TargetLocationEvent : IEvent
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public TargetLocationEvent(double lat, double lng)
        {

            this.Latitude = lat;

            this.Longitude = lng;

        }
    }
}
