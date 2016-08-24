#region using directives

using System;
using System.IO;

#endregion

namespace PokemonGo.RocketBot.Window.Resources
{
    internal class ProgressBar
    {
        public static int total = 100;
        private static int leftOffset;

        public static void start(string startText, int startAmt)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(startText);

            leftOffset = startText.Length + 1;
            fill(startAmt);
        }

        public static void fill(int amt, ConsoleColor barColor = ConsoleColor.Red)
        {
            try
            {
                // Window width has be be larger than what Console.CursorLeft is set to
                // or System.ArgumentOutOfRangeException is thrown.
                if (Console.WindowWidth < 50 + leftOffset)
                {
                    Console.WindowWidth = 51 + leftOffset;
                }

                Console.ForegroundColor = barColor;
                Console.CursorLeft = 0 + leftOffset;
                Console.Write("[");
                Console.CursorLeft = 47 + leftOffset;
                Console.Write("]");
                Console.CursorLeft = 1 + leftOffset;
                var segment = 45.5f/total;

                var pos = 1 + leftOffset;
                for (var i = 0; i < segment*amt; i++)
                {
                    Console.BackgroundColor = barColor;
                    Console.CursorLeft = pos++;
                    Console.Write(" ");
                }

                for (var i = pos; i <= 46 + leftOffset - 2; i++)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.CursorLeft = pos++;
                    Console.Write(" ");
                }

                Console.CursorLeft = 50 + leftOffset;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(amt + "%");

                if (amt == total)
                    Console.Write(Environment.NewLine);
            }
            catch (IOException)
            {
            }
        }
    }
}