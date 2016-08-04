using System;

namespace PokemonGo.Bot.Messages
{
    public class Message
    {
        public ConsoleColor Color { get; }
        public DateTime CreationTime { get; }
        public string Text { get; }

        public Message(string text)
            : this(ConsoleColor.Gray, text)
        {
        }

        public Message(ConsoleColor color, string text)
        {
            CreationTime = DateTime.Now;
            Color = color;
            Text = text;
        }
    }
}