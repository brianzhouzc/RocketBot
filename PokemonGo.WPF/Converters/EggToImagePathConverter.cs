using PokemonGo.Bot.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PokemonGo.WPF.Converters
{
    public class EggToImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var egg = value as EggViewModel;
            if (egg != null)
            {
                if(egg.IsInUnlimitedIncubator)
                    return $"pack://siteoforigin:,,,/Images/Eggs/EggIncubatorUnlimited.png";
                if (egg.IsInNormalIncubator)
                    return $"pack://siteoforigin:,,,/Images/Eggs/EggIncubator.png";
                return $"pack://siteoforigin:,,,/Images/Eggs/Egg.png";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}