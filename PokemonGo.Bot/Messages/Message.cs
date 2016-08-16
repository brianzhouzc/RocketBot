using System;
using System.Windows.Media;

namespace PokemonGo.Bot.Messages
{
    public class Message
    {
        public Color Color { get; }
        public DateTime CreationTime { get; }
        public string Text { get; }

        public Message(string text)
            : this(Colors.Gray, text)
        {
        }

        public Message(Color color, string text)
        {
            CreationTime = DateTime.Now;
            Color = color;
            Text = text;
        }
    }
}