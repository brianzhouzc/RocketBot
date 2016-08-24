using System;
using System.Drawing;

namespace PokemonGo.RocketBot.WPF.Helpers
{
    public class ResourceHelper
    {
        public static Image GetImage(string name)
        {
            return (Image) Properties.Resources.ResourceManager.GetObject(name);
        }

        public static Image GetImage(string name, int maxHeight, int maxWidth)
        {
            var image = GetImage(name);
            var ratioX = (double) maxWidth/image.Width;
            var ratioY = (double) maxHeight/image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int) (image.Width*ratio);
            var newHeight = (int) (image.Height*ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        public static Image GetPokemonImage(int pokemonId)
        {
            return GetImage("Pokemon_" + pokemonId);
        }
    }
}