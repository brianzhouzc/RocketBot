using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PokemonGo.WPF.Converters
{
    public class EggToImagePathConverter : IMultiValueConverter
    {
        static ImageSourceConverter imageSourceConverter = new ImageSourceConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (values.Length != 2)
                throw new ArgumentOutOfRangeException(nameof(values), "You must supply IsInNormalIncubator and IsInUnlimitedIncubator");

            var isInNormalIncubator = values[0] as bool?;
            var isInUnlimitedIncubator = values[1] as bool?;

            if (!isInNormalIncubator.HasValue)
                throw new ArgumentException("The first parameter must be of type bool (IsInNormalIncubator)");
            if (!isInUnlimitedIncubator.HasValue)
                throw new ArgumentException("The second parameter must be of type bool (IsInUnlimitedIncubator)");

            var uri = GetImageUri(isInNormalIncubator, isInUnlimitedIncubator);
            return imageSourceConverter.ConvertFromString(uri);
        }

        static string GetImageUri(bool? isInNormalIncubator, bool? isInUnlimitedIncubator)
        {
            if (isInUnlimitedIncubator.Value)
                return $"pack://siteoforigin:,,,/Images/Eggs/EggIncubatorUnlimited.png";
            if (isInNormalIncubator.Value)
                return $"pack://siteoforigin:,,,/Images/Eggs/EggIncubator.png";
            return $"pack://siteoforigin:,,,/Images/Eggs/Egg.png";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}