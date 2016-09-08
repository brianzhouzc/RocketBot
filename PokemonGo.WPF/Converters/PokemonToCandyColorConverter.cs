using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PokemonGo.WPF.Converters
{
    class PokemonToCandyColorConverter : IValueConverter
    {
        static PokemonToImagePathConverter pokemonToImagePathConverter = new PokemonToImagePathConverter();
        static IDictionary<int, Tuple<Color, Color>> pokemonToColorCache = new Dictionary<int, Tuple<Color, Color>>(151);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Colors.Red;

            var number = (int)value;
            var colorIndex = System.Convert.ToInt32(parameter);
            if (number > 0)
            {
                Tuple<Color, Color> colors;
                if (!pokemonToColorCache.TryGetValue(number, out colors))
                {
                    var pokemonImagePath = (string)pokemonToImagePathConverter.Convert(value, typeof(string), parameter, culture);
                    var pokemonImage = new BitmapImage(new Uri(pokemonImagePath));
                    var candypalette = new BitmapPalette(pokemonImage, 2);
                    colors = Tuple.Create(candypalette.Colors[0], candypalette.Colors[1]);
                    pokemonToColorCache[number] = colors;
                }

                return colorIndex == 0 ? colors.Item1 : colors.Item2;
            }

            return Colors.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}