using System;
using System.Globalization;

namespace PokemonGo.WPF.Converters
{
    public class MapLocationToPositionConverter : PositionToMapLocationConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => base.ConvertBack(value, targetType, parameter, culture);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => base.Convert(value, targetType, parameter, culture);
    }
}