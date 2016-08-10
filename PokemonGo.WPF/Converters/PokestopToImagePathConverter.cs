using PokemonGo.Bot.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PokemonGo.WPF.Converters
{
    public class PokestopToImagePathConverter : IMultiValueConverter
    {
        private static ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
                throw new ArgumentOutOfRangeException(nameof(values), "You must supply IsActive and IsNear");

            var isActive = values[0] as bool?;
            var isNear = values[1] as bool?;

            if (!isActive.HasValue)
                throw new ArgumentException("The first parameter must be of type bool (IsActive)");
            if (!isNear.HasValue)
                throw new ArgumentException("The second parameter must be of type bool (IsNear)");

            var uri = GetImageUri(isActive, isNear);
            return imageSourceConverter.ConvertFromString(uri);
        }

        private static string GetImageUri(bool? isActive, bool? isNear)
        {
            var isActiveString = isActive.Value ? "active" : "inactive";
            var isNearString = isNear.Value ? "near" : "far";
            return $"pack://siteoforigin:,,,/Images/Map/pokestop_{isActiveString}_{isNearString}.png";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}