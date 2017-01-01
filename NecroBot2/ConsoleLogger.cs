#region using directives

using System;
using System.Text;
using PoGo.NecroBot.Logic.Logging;
using PoGo.NecroBot.Logic.State;
using NecroBot2.Forms;
using System.Drawing;

#endregion

namespace NecroBot2
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
            //Console.OutputEncoding = Encoding.UTF8;
            if (level > _maxLogLevel)
                return;

            var finalMessage = Logger.GetFinalMessage(message, level, color);
            //  Console.WriteLine(finalMessage);

            // Fire log write event.
            OnLogWrite?.Invoke(this, new LogWriteEventArgs { Message = finalMessage, Level = level, Color = color });
           
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (level)
            {
                case LogLevel.Berry:
                    MainForm.ColoredConsoleWrite(Color.DarkGoldenrod, finalMessage);
                    break;
                case LogLevel.Caught:
                    MainForm.ColoredConsoleWrite(Color.Green, finalMessage);
                    break;
                case LogLevel.Debug:
                    MainForm.ColoredConsoleWrite(Color.Gray, finalMessage);
                    break;
                case LogLevel.Egg:
                    MainForm.ColoredConsoleWrite(Color.DarkGoldenrod, finalMessage);
                    break;
                case LogLevel.Error:
                    MainForm.ColoredConsoleWrite(Color.Red, finalMessage);
                    break;
                case LogLevel.Evolve:
                    MainForm.ColoredConsoleWrite(Color.Yellow, finalMessage);
                    break;
                case LogLevel.Farming:
                    MainForm.ColoredConsoleWrite(Color.Magenta, finalMessage);
                    break;
                case LogLevel.Flee:
                    MainForm.ColoredConsoleWrite(Color.Orange, finalMessage);
                    break;
                case LogLevel.Gym:
                    MainForm.ColoredConsoleWrite(Color.LightCyan, finalMessage);
                    break;
                case LogLevel.Info:
                    MainForm.ColoredConsoleWrite(Color.DarkCyan, finalMessage);
                    break;
                case LogLevel.LevelUp:
                    MainForm.ColoredConsoleWrite(Color.Yellow, finalMessage);
                    break;
                case LogLevel.New:
                    MainForm.ColoredConsoleWrite(Color.Green, finalMessage);
                    break;
                case LogLevel.None:
                    MainForm.ColoredConsoleWrite(Color.White, finalMessage);
                    break;
                case LogLevel.Pokestop:
                    MainForm.ColoredConsoleWrite(Color.Cyan, finalMessage);
                    break;
                case LogLevel.Recycling:
                    MainForm.ColoredConsoleWrite(Color.DarkMagenta, finalMessage);
                    break;
                case LogLevel.Service:
                    MainForm.ColoredConsoleWrite(Color.LimeGreen, finalMessage);
                    break;
                case LogLevel.Sniper:
                    MainForm.ColoredConsoleWrite(Color.White, finalMessage);
                    break;
                case LogLevel.SoftBan:
                    MainForm.ColoredConsoleWrite(Color.Red, finalMessage);
                    break;
                case LogLevel.Transfer:
                    MainForm.ColoredConsoleWrite(Color.DarkGreen, finalMessage);
                    break;
                case LogLevel.Update:
                    MainForm.ColoredConsoleWrite(Color.White, finalMessage);
                    break;
                case LogLevel.Warning:
                    MainForm.ColoredConsoleWrite(Color.Goldenrod, finalMessage);
                    break;
                default:
                    MainForm.ColoredConsoleWrite(Color.White, finalMessage);
                    break;
            }
        }

        public void lineSelect(int lineChar = 0, int linesUp = 1)
        {
            //Console.SetCursorPosition(lineChar, Console.CursorTop - linesUp);
        }

        private class LogWriteEventArgs
        {
            public string Message
            {
                get { return Message; }
                set { Message = value; }
            }
            public LogLevel Level
            {
                get { return Level; }
                set { Level = value; }
            }
            public ConsoleColor Color
            {
                get { return Color; }
                set { Color = value; }
            }
        }
        private event LogWriteHandler OnLogWrite;
    }
}