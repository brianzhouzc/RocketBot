using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Event.UI
{
    public class StatusBarEvent   :IEvent
    {
        public StatusBarEvent (string s)
        {
            this.Message = s;
        }
        public string Message { get; set; }
    }
}
