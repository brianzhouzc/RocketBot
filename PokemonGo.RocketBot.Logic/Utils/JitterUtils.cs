#region using directives

using System;
using System.Threading.Tasks;

#endregion

namespace PokemonGo.RocketBot.Logic.Utils
{
    public static class JitterUtils
    {
        private static readonly Random RandomDevice = new Random();

        public static Task RandomDelay(int min, int max)
        {
            return Task.Delay(RandomDevice.Next(min, max));
        }
    }
}