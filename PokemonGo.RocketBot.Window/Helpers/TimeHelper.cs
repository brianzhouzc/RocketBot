using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketBot.Window.Helpers
{
    public class TimeHelper
    {
        public static DateTime FromUnixTimeUtc(long time)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(time);
        }
    }
}
