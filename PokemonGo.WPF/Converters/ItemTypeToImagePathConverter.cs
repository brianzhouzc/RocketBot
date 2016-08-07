using System;
using System.Globalization;
using System.Windows.Data;

namespace PokemonGo.WPF.Converters
{
    public class ItemTypeToImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var number = (int)value;
            if (number > 0)
                return $"pack://siteoforigin:,,,/Images/Items/{number:0000}.png";
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}