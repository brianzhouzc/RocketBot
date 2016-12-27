#region using directives

using System;
using System.Collections.Generic;
using System.IO;
using PoGo.NecroBot.Logic.State;
using System.Collections.Concurrent;

#endregion

namespace PoGo.NecroBot.Logic.Logging
{
    public static class Logger
    {
        private static List<ILogger> _loggers = new List<ILogger>();
        
        private static ConcurrentQueue<string> LogbufferList = new ConcurrentQueue<string>();
        private static string _lastLogMessage;

        public static void TurnOffLogBuffering()
        {
            foreach (var logger in _loggers)
            {
                logger?.TurnOffLogBuffering();
            }
        }
        
        /// <summary>
        ///   Add a logger.
        /// </summary>
        /// <param name="logger"></param>
        public static void AddLogger(ILogger logger, string subPath = "", bool isGui = false)
        {
            if (!_loggers.Contains(logger))
                _loggers.Add(logger);
        }

        /// <summary>
        ///     Sets Context for the loggers
        /// </summary>
        /// <param name="session">Context</param>
        public static void SetLoggerContext(ISession session)
        {
            LoggingStrings.SetStrings(session);
            foreach (var logger in _loggers)
            {
                logger?.SetSession(session);
            }
        }

        /// <summary>
        ///     Log a specific message to the logger setup by <see cref="SetLogger(ILogger)" /> .
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="level">Optional level to log. Default <see cref="LogLevel.Info" />.</param>
        /// <param name="color">Optional. Default is automatic color.</param>
        public static void Write(string message, LogLevel level = LogLevel.Info, ConsoleColor color = ConsoleColor.Black, bool force = false)
        {
            if (_loggers.Count == 0 || _lastLogMessage == message)
                return;

            _lastLogMessage = message;
            foreach(var logger in _loggers)
                logger?.Write(message, level, color);
        }

        public static void lineSelect(int lineChar = 0, int linesUp = 1)
        {
            foreach(var logger in _loggers)
                logger?.lineSelect(lineChar, linesUp);
        }

        public static string GetFinalMessage(string message, LogLevel level, ConsoleColor color)
        {
            string finalMessage;

            switch (level)
            {
                case LogLevel.Error:
                    Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.Red : color;
                    finalMessage = $"[{DateTime.Now.ToString("HH:mm:ss")}] ({LoggingStrings.Error}) {message}";
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.DarkYellow : color;
                    finalMessage = $"[{DateTime.Now.ToString("HH:mm:ss")}] ({LoggingStrings.Attention}) {message}";
                    break;
                case LogLevel.Info:
                    Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.DarkCyan : color;
                    finalMessage = $"[{DateTime.Now.ToString("HH:mm:ss")}] ({LoggingStrings.Info}) {message}";
                    break;
                case LogLevel.Pokestop:
                    Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.Cyan : color;
                    finalMessage = $"[{DateTime.Now.ToString("HH:mm:ss")}] ({LoggingStrings.Pokestop}) {message}";
                    break;
                case LogLevel.Farming:
                    Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.Magenta : color;
                    finalMessage = $"[{DateTime.Now.ToString("HH:mm:ss")}] ({LoggingStrings.Farming}) {message}";
                    break;
                case LogLevel.Sniper:
                    Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.White : color;
                    finalMessage = $"[{DateTime.Now.ToString("HH:mm:ss")}] ({LoggingStrings.Sniper}) {message}";
                    break;
                case LogLevel.Recycling:
                    Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.DarkMagenta : color;
                    finalMessage = $"[{DateTime.Now.ToString("HH:mm:ss")}] ({LoggingStrings.Recycling}) {message}";
                    break;
                case LogLevel.Caught:
                    Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.Green : color;
                    finalMessage = $"[{DateTime.Now.ToString("HH:mm:ss")}] ({LoggingStrings.Pkmn}) {message}";
                    break;
                case LogLevel.Flee:
                    Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.DarkYellow : color;
                    finalMessage = $"[{DateTime.Now.ToString("HH:mm:ss")}] ({LoggingStrings.Pkmn}) {message}";
                    break;
                case LogLevel.Transfer:
                    Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.DarkGreen : color;
                    finalMessage = $"[{DateTime.Now.ToString("HH:mm:ss")}] ({LoggingStrings.Transferred}) {message}";
                    break;
                case LogLevel.Evolve:
                    Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.DarkGreen : color;
                    finalMessage = $"[{DateTime.Now.ToString("HH:mm:ss")}] ({LoggingStrings.Evolved}) {message}";
                    break;
                case LogLevel.Berry:
                    Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.DarkYellow : color;
                    finalMessage = $"[{DateTime.Now.ToString("HH:mm:ss")}] ({LoggingStrings.Berry}) {message}";
                    break;
                case LogLevel.Egg:
                    Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.DarkYellow : color;
                    finalMessage = $"[{DateTime.Now.ToString("HH:mm:ss")}] ({LoggingStrings.Egg}) {message}";
                    break;
                case LogLevel.Debug:
                    Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.Gray : color;
                    finalMessage = $"[{DateTime.Now.ToString("HH:mm:ss")}] ({LoggingStrings.Debug}) {message}";
                    break;
                case LogLevel.Update:
                    Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.White : color;
                    finalMessage = $"[{DateTime.Now.ToString("HH:mm:ss")}] ({LoggingStrings.Update}) {message}";
                    break;
                case LogLevel.New:
                    Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.Green : color;
                    finalMessage = $"[{DateTime.Now.ToString("HH:mm:ss")}] ({LoggingStrings.New}) {message}";
                    break;
                case LogLevel.SoftBan:
                    Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.Red : color;
                    finalMessage = $"[{DateTime.Now.ToString("HH:mm:ss")}] ({LoggingStrings.SoftBan}) {message}";
                    break;
                case LogLevel.LevelUp:
                    Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.Magenta : color;
                    finalMessage = $"[{DateTime.Now.ToString("HH:mm:ss")}] ({LoggingStrings.Pkmn}) {message}";
                    break;

                case LogLevel.Gym:
                    Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.Magenta : color;
                    finalMessage = $"[{DateTime.Now.ToString("HH:mm:ss")}] ({LoggingStrings.Gym}) {message}";
                    break;
                case LogLevel.Service:
                    Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.White : color;
                    finalMessage = $"[{DateTime.Now.ToString("HH:mm:ss")}] ({LoggingStrings.Service}) {message}";
                    break;
                default:
                    Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.White : color;
                    finalMessage = $"[{DateTime.Now.ToString("HH:mm:ss")}] ({LoggingStrings.Error}) {message}";
                    break;
            }
            return finalMessage;
        }

        public static string GetHexColor(ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Black:
                    return "#000000";

                case ConsoleColor.Blue:
                    return "#0000FF";

                case ConsoleColor.Cyan:
                    return "#00FFFF";

                case ConsoleColor.DarkBlue:
                    return "#000080";

                case ConsoleColor.DarkCyan:
                    return "#008B8B";

                case ConsoleColor.DarkGray:
                    return "#808080";

                case ConsoleColor.DarkGreen:
                    return "#008000";

                case ConsoleColor.DarkMagenta:
                    return "#800080";

                case ConsoleColor.DarkRed:
                    return "#800000";

                case ConsoleColor.DarkYellow:
                    return "#808000";

                case ConsoleColor.Gray:
                    return "#C0C0C0";

                case ConsoleColor.Green:
                    return "#00FF00";

                case ConsoleColor.Magenta:
                    return "#FF00FF";

                case ConsoleColor.Red:
                    return "#FF0000";

                case ConsoleColor.White:
                    return "#FFFFFF";

                case ConsoleColor.Yellow:
                    return "#FFFF00";

                default:
                    // Grey
                    return "#C0C0C0";
            }
        }
    }

    public enum LogLevel
    {
        None = 0,
        Error = 1,
        Warning = 2,
        Pokestop = 3,
        Farming = 4,
        Sniper = 5,
        Recycling = 6,
        Berry = 7,
        Caught = 8,
        Flee = 9,
        Transfer = 10,
        Evolve = 11,
        Egg = 12,
        Update = 13,
        Info = 14,
        New = 15,
        SoftBan = 16,
        LevelUp = 17,
        Gym = 18,
        Service = 19,
        Debug = 20,
    }
}