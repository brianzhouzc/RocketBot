using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using PokemonGo.RocketBot.Logic.Logging;

namespace PokemonGo.RocketBot.Logic.Utils
{
    internal class ErrorHandler
    {
        /// <summary>
        ///     Alerts that a fatal error has occurred, displaying a message and exiting the application
        /// </summary>
        /// <param name="strMessage">Optional message to display - Leave NULL to exclude message</param>
        /// <param name="timeout">The total seconds the messag will display before shutting down</param>
        public static void ThrowFatalError(string strMessage, int timeout, LogLevel level, bool boolRestart = false)
        {
            if (strMessage != null)
                Logger.Write(strMessage, level);

            Console.Write("Ending Application... ");

            for (var i = timeout; i > 0; i--)
            {
                Console.Write("\b" + i);
                Thread.Sleep(1000);
            }

            if (boolRestart)
                Process.Start(Assembly.GetEntryAssembly().Location);

            Environment.Exit(-1);
        }
    }
}