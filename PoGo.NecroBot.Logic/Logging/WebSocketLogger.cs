#region using directives

using System;
using System.Text;
using PoGo.NecroBot.Logic.Event;
using System.Collections.Concurrent;
using System.IO;
using PoGo.NecroBot.Logic.State;

#endregion

namespace PoGo.NecroBot.Logic.Logging
{
    /// <summary>
    ///     The WebSocketLogger is a simple logger which writes all logs to the Console.
    /// </summary>
    public class WebSocketLogger : ILogger
    {
        private readonly LogLevel _maxLogLevel;
        private ISession _session;
        private ConcurrentQueue<LogEvent> _messageQueue = new ConcurrentQueue<LogEvent>();
        private bool isBuffering = true;

        public void TurnOffLogBuffering()
        {
            if (isBuffering)
                isBuffering = false;
        }

        /// <summary>
        ///     To create a WebSocketLogger, we must define a maximum log level.
        ///     All levels above won't be logged.
        /// </summary>
        /// <param name="maxLogLevel"></param>
        public WebSocketLogger(LogLevel maxLogLevel)
        {
            _maxLogLevel = maxLogLevel;
        }

        public void SetSession(ISession session)
        {
            _session = session;
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

            var finalMessage = Logger.GetFinalMessage(message, level, color);

            // Add message to the queue
            _messageQueue.Enqueue(new LogEvent
            {
                Message = finalMessage,
                Color = Logger.GetHexColor(Console.ForegroundColor)
            });

            // We cannot send out log events to the GUI until it has connected via websocket. So buffer all 
            // messages until GUI has connected.
            if (_session != null && _session.EventDispatcher != null)
            {
                if (!isBuffering)
                {
                    LogEvent logEventToSend;
                    while (_messageQueue.TryDequeue(out logEventToSend))
                    {
                        _session?.EventDispatcher?.Send(logEventToSend);
                    }
                }
            }
        }

        public void lineSelect(int lineChar = 0, int linesUp = 1)
        {
            // No line select for file logger.
        }
    }
}