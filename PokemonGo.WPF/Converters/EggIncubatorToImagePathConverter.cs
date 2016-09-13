using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PokemonGo.WPF.Converters
{
    public class EggIncubatorToImagePathConverter : IMultiValueConverter
    {
        static ImageSourceConverter imageSourceConverter = new ImageSourceConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (values.Length != 2)
                throw new ArgumentOutOfRangeException(nameof(values), "You must supply IsUnlimited and IsInUse");

            var isUnlimited = values[0] as bool?;
            var isInUse = values[1] as bool?;

            if (!isUnlimited.HasValue)
                throw new ArgumentException("The first parameter must be of type bool (IsUnlimited)");
            if (!isInUse.HasValue)
                throw new ArgumentException("The second parameter must be of type bool (IsInUse)");

            var uri = GetImageUri(isUnlimited, isInUse);
            return imageSourceConverter.ConvertFromString(uri);
        }

        static string GetImageUri(bool? isUnlimited, bool? isInUse)
        {
            if (isUnlimited.Value)
            {
                if(isInUse.Value)
                    return $"pack://siteoforigin:,,,/Images/Eggs/EggIncubatorUnlimited.png";
                return $"pack://siteoforigin:,,,/Images/Items/0901.png";
            }
            if (isInUse.Value)
                return $"pack://siteoforigin:,,,/Images/Eggs/EggIncubator.png";
            return $"pack://siteoforigin:,,,/Images/Items/0902.png";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}