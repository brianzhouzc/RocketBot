#region using directives

using System;
using PoGo.NecroBot.Logic.Logging;
using PoGo.NecroBot.Logic.State;
using RocketBot2.Forms;
using System.Drawing;
using System.Text;
using System.Collections.Generic;

#endregion

namespace RocketBot2
{
    /// <summary>
    ///     The ConsoleLogger is a simple logger which writes all logs to the Console.
    /// </summary>
    internal class ConsoleLogger : ILogger
    {
        // Log write event definition.
        private delegate void LogWriteHandler(object sender, LogWriteEventArgs e);

        private readonly LogLevel _maxLogLevel;
		//private ISession _session;

        /// <summary>
        ///     To create a ConsoleLogger, we must define a maximum log level.
        ///     All levels above won't be logged.
        /// </summary>
        /// <param name="maxLogLevel"></param>
        internal ConsoleLogger(LogLevel maxLogLevel)
        {
            _maxLogLevel = maxLogLevel;
        }

        public void TurnOffLogBuffering()
        {
            // No need for buffering
        }

        public void SetSession(ISession session)
        {
            // No need for session
        }

        /// <summary>
        ///     Log a specific message by LogLevel. Won't log if the LogLevel is greater than the maxLogLevel set.
        /// </summary>
        /// <param name="message">The message to log. The current time will be prepended.</param>
        /// <param name="level">Optional. Default <see cref="LogLevel.Info" />.</param>
        /// <param name="color">Optional. Default is auotmatic</param>
        public void Write(string message, LogLevel level = LogLevel.Info, ConsoleColor color = ConsoleColor.Black)
        {
            // Remember to change to a font that supports your language, otherwise it'll still show as ???.
            Console.OutputEncoding = Encoding.UTF8;
            if (level > _maxLogLevel)
                return;

            var finalMessage = Logger.GetFinalMessage(message.Replace("NecroBot", "RocketBot"), level, color);
            Console.WriteLine(finalMessage);

            // Fire log write event.
            OnLogWrite?.Invoke(this, new LogWriteEventArgs { Message = finalMessage, Level = level, Color = color });

            // ReSharper disable once SwitchStatementMissingSomeCases
            Color _color = new Color();
            Dictionary<LogLevel, Color> colors = new Dictionary<LogLevel, Color>()
            {
                { LogLevel.Error, Color.Red },
                { LogLevel.Caught, Color.Green },
                { LogLevel.Info, Color.DarkCyan } ,
                { LogLevel.Warning, Color.FromArgb(255, 128, 128, 0) } ,
                { LogLevel.Pokestop, Color.Cyan }  ,
                { LogLevel.Farming, Color.Magenta },
                { LogLevel.Sniper, Color.White },
                { LogLevel.Recycling, Color.DarkMagenta },
                { LogLevel.Flee, Color.FromArgb(255, 128, 128, 0) },
                { LogLevel.Transfer, Color.DarkGreen },
                { LogLevel.Evolve, Color.DarkGreen },
                { LogLevel.Berry, Color.FromArgb(255, 128, 128, 0) },
                { LogLevel.Egg, Color.FromArgb(255, 128, 128, 0) },
                { LogLevel.Debug, Color.Gray },
                { LogLevel.Update, Color.White },
                { LogLevel.New, Color.Green },
                { LogLevel.SoftBan, Color.Red },
                { LogLevel.LevelUp, Color.Magenta },
                { LogLevel.BotStats, Color.Green },
                { LogLevel.Gym, Color.Magenta },
                { LogLevel.GymDisk, Color.Cyan },
                { LogLevel.Service, Color.White }
            };

            _color = colors[level];

            if (string.IsNullOrEmpty(color.ToString()) || color.ToString() != "Black")
            {
                _color = FromColor(color);
            }

            if (string.IsNullOrEmpty(_color.ToString())) _color = Color.White;

            MainForm.ColoredConsoleWrite(_color, finalMessage);
        }

        public static Color FromColor(ConsoleColor c)
        {
            switch (c)
            {
                case ConsoleColor.DarkYellow:
                    return Color.FromArgb(255, 128, 128, 0);
                default:
                    return Color.FromName(c.ToString());
            }
        }

        public void LineSelect(int lineChar = 0, int linesUp = 1)
        {
            Console.SetCursorPosition(lineChar, Console.CursorTop - linesUp);
        }

        private class LogWriteEventArgs
        {
            private string message;
            public string Message
            {
                get { return message; }
                set { message = value; }
            }
            private LogLevel level;
            public LogLevel Level
            {
                get { return level; }
                set { level = value; }
            }
            private ConsoleColor color;
            public ConsoleColor Color
            {
                get { return color; }
                set { color = value; }
            }
        }
        private event LogWriteHandler OnLogWrite;
    }
}
