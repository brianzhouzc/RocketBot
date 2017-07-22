using POGOProtos.Data;
using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
using POGOProtos.Map.Pokemon;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace RocketBot2.Helpers
{
    public class ResourceHelper
    {
        public static Image ItemPicture(ItemData item)
        {
            var image = (Image)Resources.ItemIdDB.ResourceManager.GetObject($"_{(int)item.ItemId}");
            if (image != null) return image;
            return (Image)Properties.Resources.ResourceManager.GetObject("question");
        }

        public static Image GetImage(string name)
        {
            return GetImage(name, null, null);
        }

        public static Image GetImage(string name, PokemonData pokemon = null, MapPokemon _pokemon = null)
        {
            if (pokemon != null) return GetPokemonImage(pokemon);
            if (_pokemon != null) return GetPokemonImage(_pokemon);
            return (Image)Properties.Resources.ResourceManager.GetObject(name);
        }

        public static Image GetImage(string name, PokemonData pokemon = null, MapPokemon _pokemon = null, int maxHeight = 0, int maxWidth = 0)
        {
            var image = GetImage(name, pokemon, _pokemon);
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            if (image != null) return newImage;
            return (Image)Properties.Resources.ResourceManager.GetObject("question");
        }

        public static Image SetImageSize(Image image, int maxHeight, int maxWidth)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            if (image != null) return newImage;
            return (Image)Properties.Resources.ResourceManager.GetObject("question");
        }

        public static Image GetPokemonImage(PokemonData pokemon)
        {
            var additional = "";
            if (pokemon.PokemonDisplay.Costume != Costume.Unset) additional = "_" + pokemon.PokemonDisplay.Costume.ToString().Replace("-Unset", "");
            if (pokemon.PokemonDisplay.Form != POGOProtos.Enums.Form.Unset) additional = "_" + pokemon.PokemonDisplay.Form.ToString().Replace("Unown", "").Replace("-ExclamationPoint", "_ExclamationPoint").Replace("-QuestionMark", "_QuestionMark").Replace("-Unset", "");
            if (pokemon.PokemonDisplay.Shiny) additional = "_shiny";

            var image = (Image)Resources.PokemonDB.ResourceManager.GetObject($"_{(int)pokemon.PokemonId}{additional}");
            if (image != null) return image;
            return (Image)Properties.Resources.ResourceManager.GetObject("question");
            //return LoadPicture($"http://assets.pokemon.com/assets/cms2/img/pokedex/full/{(int)pokemonId:000}.png");*/
        }

        public static Image GetPokemonImage(MapPokemon pokemon)
        {
            var additional = "";
            /*if (pokemon.PokemonDisplay.Costume != Costume.Unset) additional = "_" + pokemon.PokemonDisplay.Costume.ToString().Replace("-Unset", "");
            if (pokemon.PokemonDisplay.Form != POGOProtos.Enums.Form.Unset) additional = "_" + pokemon.PokemonDisplay.Form.ToString().Replace("Unown", "").Replace("-ExclamationPoint", "_ExclamationPoint").Replace("-QuestionMark", "_QuestionMark").Replace("-Unset", "");
            if (pokemon.PokemonDisplay.Shiny) additional = "_shiny";*/

            var image = (Image)Resources.PokemonDB.ResourceManager.GetObject($"_{(int)pokemon.PokemonId}{additional}");
            if (image != null) return image;
            return (Image)Properties.Resources.ResourceManager.GetObject("question");
            //return LoadPicture($"http://assets.pokemon.com/assets/cms2/img/pokedex/full/{(int)pokemon.PokemonId:000}.png");
        }

        public static Image GetPokemonImage(int ToPokemonId, PokemonData pokemon)
        {
            var additional = "";
            if (pokemon.PokemonDisplay.Costume != Costume.Unset) additional = "_" + pokemon.PokemonDisplay.Costume.ToString().Replace("-Unset", "");
            if (pokemon.PokemonDisplay.Form != POGOProtos.Enums.Form.Unset) additional = "_" + pokemon.PokemonDisplay.Form.ToString().Replace("Unown", "").Replace("-ExclamationPoint", "_ExclamationPoint").Replace("-QuestionMark", "_QuestionMark").Replace("-Unset", "");
            if (pokemon.PokemonDisplay.Shiny) additional = "_shiny";

            var image = (Image)Resources.PokemonDB.ResourceManager.GetObject($"_{ToPokemonId}{additional}");
            if (image != null) return image;
            return (Image)Properties.Resources.ResourceManager.GetObject("question");
            //return LoadPicture($"http://assets.pokemon.com/assets/cms2/img/pokedex/full/{(int)pokemonId:000}.png");
        }

        public static Image GetPokemonImageById(int PokemonId)
        {
            var image = (Image)Resources.PokemonDB.ResourceManager.GetObject($"_{PokemonId}");
            if (image != null) return image;
            return (Image)Properties.Resources.ResourceManager.GetObject("question");
            //return LoadPicture($"http://assets.pokemon.com/assets/cms2/img/pokedex/full/{(int)pokemonId:000}.png");
        }

        #region Image Utilities

        /// <summary>
        /// Loads an image from a URL into a Bitmap object.
        /// Currently as written if there is an error during downloading of the image, no exception is thrown.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Bitmap LoadPicture(string url)
        {
            System.Net.HttpWebRequest wreq;
            System.Net.HttpWebResponse wresp;
            Stream mystream;
            Bitmap bmp;

            bmp = null;
            mystream = null;
            wresp = null;
            try
            {
                wreq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
                wreq.AllowWriteStreamBuffering = true;

                wresp = (System.Net.HttpWebResponse)wreq.GetResponse();

                if ((mystream = wresp.GetResponseStream()) != null)
                    bmp = new Bitmap(mystream);
            }
            catch
            {
                // Do nothing... 
            }
            finally
            {
                if (mystream != null)
                    mystream.Close();

                if (wresp != null)
                    wresp.Close();
            }

            return (bmp);
        }

        /// <summary>
        /// Takes in an image, scales it maintaining the proper aspect ratio of the image such it fits in the PictureBox's canvas size and loads the image into picture box.
        /// Has an optional param to center the image in the picture box if it's smaller then canvas size.
        /// </summary>
        /// <param name="image">The Image you want to load, see LoadPicture</param>
        /// <param name="canvas">The canvas you want the picture to load into</param>
        /// <param name="centerImage"></param>
        /// <returns></returns>

        public static Image ResizeImage(Image image, PictureBox canvas, bool centerImage)
        {
            if (image == null || canvas == null)
            {
                return null;
            }

            int canvasWidth = canvas.Size.Width;
            int canvasHeight = canvas.Size.Height;
            int originalWidth = image.Size.Width;
            int originalHeight = image.Size.Height;

            System.Drawing.Image thumbnail =
                new Bitmap(canvasWidth, canvasHeight); // changed parm names
            System.Drawing.Graphics graphic =
                         System.Drawing.Graphics.FromImage(thumbnail);

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;

            /* ------------------ new code --------------- */

            // Figure out the ratio
            double ratioX = (double)canvasWidth / (double)originalWidth;
            double ratioY = (double)canvasHeight / (double)originalHeight;
            double ratio = ratioX < ratioY ? ratioX : ratioY; // use whichever multiplier is smaller

            // now we can get the new height and width
            int newHeight = Convert.ToInt32(originalHeight * ratio);
            int newWidth = Convert.ToInt32(originalWidth * ratio);

            // Now calculate the X,Y position of the upper-left corner 
            // (one of these will always be zero)
            int posX = Convert.ToInt32((canvasWidth - (image.Width * ratio)) / 2);
            int posY = Convert.ToInt32((canvasHeight - (image.Height * ratio)) / 2);

            if (!centerImage)
            {
                posX = 0;
                posY = 0;
            }
            graphic.Clear(Color.White); // white padding
            graphic.DrawImage(image, posX, posY, newWidth, newHeight);

            /* ------------- end new code ---------------- */

            System.Drawing.Imaging.ImageCodecInfo[] info =
                             ImageCodecInfo.GetImageEncoders();
            EncoderParameters encoderParameters;
            encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality,
                             100L);

            Stream s = new System.IO.MemoryStream();
            thumbnail.Save(s, info[1],
                              encoderParameters);

            return Image.FromStream(s);
        }

        public static Image ConvertToBlack(Image image)
        {
            Bitmap bmp = new Bitmap(image);
            int width = bmp.Width;
            int height = bmp.Height;
            int[] arr = new int[225];
            Color p;

            //Grayscale
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    p = bmp.GetPixel(x, y);
                    int a = p.A;
                    int r = p.R;
                    int g = p.G;
                    int b = p.B;
                    int avg = (r + g + b) / 3;
                    //avg = avg < 128 ? 0 : 255;     // Converting gray pixels to either pure black or pure white
                    avg = 0;
                    bmp.SetPixel(x, y, Color.FromArgb(a, avg, avg, avg));
                }
            }
            return bmp;
        }

        public static Image ConvertToBlackAndWhite(Image image, int div = 128, int black = 0, int white = 255)
        {
            Bitmap bmp = new Bitmap(image);
            int width = bmp.Width;
            int height = bmp.Height;
            int[] arr = new int[225];
            Color p;

            //Grayscale
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    p = bmp.GetPixel(x, y);
                    int a = p.A;
                    int r = p.R;
                    int g = p.G;
                    int b = p.B;
                    int avg = (r + g + b) / 3;
                    avg = avg < div ? black : white;     // Converting gray pixels to either pure black or pure white values 128 0 255
                    bmp.SetPixel(x, y, Color.FromArgb(a, avg, avg, avg));
                }
            }
            return bmp;
        }

        public static Image GetSlashedPokemonImage(Image image)
        {
            Image source = GetImage("slashed", null, null, image.Size.Height, image.Size.Width);
            var target = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
            var graphics = Graphics.FromImage(target);
            graphics.CompositingMode = CompositingMode.SourceOver; // this is the default, but just to be clear
            graphics.DrawImage(image, 0, 0);
            graphics.DrawImage(source, 0, 0);
            return target;
        }

        public static Image GetGymSpawnImage(Image image)
        {
            Image source = GetImage("spawn", null, null, image.Size.Height, image.Size.Width);
            var target = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
            var graphics = Graphics.FromImage(target);
            graphics.CompositingMode = CompositingMode.SourceOver; // this is the default, but just to be clear
            graphics.DrawImage(image, 0, 0);
            graphics.DrawImage(source, 0, 0);
            return target;
        }

        public static Image GetGymVisitedImage(Image image)
        {
            Image source = GetImage("GymVisited", null, null, image.Size.Height, image.Size.Width);
            var target = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
            var graphics = Graphics.FromImage(target);
            graphics.CompositingMode = CompositingMode.SourceOver; // this is the default, but just to be clear
            graphics.DrawImage(image, 0, 0);
            graphics.DrawImage(source, 0, 0);
            return target;
        }


        public static Image CombineImages(Image image, Image image2)
        {
            var target = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
            var graphics = Graphics.FromImage(target);
            graphics.CompositingMode = CompositingMode.SourceOver; // this is the default, but just to be clear
            graphics.DrawImage(image, 0, 0);
            graphics.DrawImage(image2, 0, 0);
            return target;
        }
        #endregion
    }
}