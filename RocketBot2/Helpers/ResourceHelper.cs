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
        public static Image GetImage(string name)
        {
            var strSplit = name.Split('_');
            if (strSplit.Length > 1)
            {
                var strStatus = strSplit[0];
                var id = strSplit[1];

                if (strStatus.ToLower().Contains("pokemon"))
                {
                    return GetPokemonImage(Convert.ToInt32(id));  
                }
            }
            return (Image)Properties.Resources.ResourceManager.GetObject(name);
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
            if (pokemonId > 151)
            {
               return LoadPicture($"https://rankedboost.com/wp-content/plugins/ice/riot/poksimages/pokemons2/{pokemonId:000}.png");
            }
            else
            {
               return LoadPicture($"https://rankedboost.com/wp-content/plugins/ice/riot/poksimages/pokemons/{pokemonId:000}.png");
            }
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
#endregion
    }
}