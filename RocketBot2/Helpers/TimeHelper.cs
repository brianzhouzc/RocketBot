using System;

namespace RocketBot2.Helpers
{
    public class TimeHelper
    {
        public static DateTime FromUnixTimeUtc(long time)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(time);
        }
    }
}