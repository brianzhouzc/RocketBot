using POGOProtos.Enums;
using System;
using System.Drawing;

namespace RocketBot2.Helpers
{
    public class ResourceHelper
    {
        public static Image GetImage(string name)
        {
            //TODO: 2G Images 151-25x missing
            if ((Image)Properties.Resources.ResourceManager.GetObject(name) != null)
            {
                return (Image)Properties.Resources.ResourceManager.GetObject(name);
            }
            return (Image)Properties.Resources.ResourceManager.GetObject("Pokemon_152");
        }

        public static Image GetImage(string name, int maxHeight, int maxWidth)
        {
                var image = GetImage(name);
                var ratioX = (double)maxWidth /  image.Width;
                var ratioY = (double)maxHeight / image.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var newWidth = (int)(image.Width * ratio);
                var newHeight = (int)(image.Height * ratio);

                var newImage = new Bitmap(newWidth, newHeight);

                using (var graphics = Graphics.FromImage(newImage))
                    graphics.DrawImage(image, 0, 0, newWidth, newHeight);
                return  newImage;
        }
        public static Image GetPokemonImage(int pokemonId)
        {
            //TODO: 2G Images 151-25x missing
            if (pokemonId < 151)
            {
                return GetImage($"Pokemon_{pokemonId}");
            }
            return GetImage("Pokemon_152");
        }
    }
}