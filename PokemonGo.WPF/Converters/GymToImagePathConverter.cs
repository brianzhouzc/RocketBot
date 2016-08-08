using PokemonGo.Bot.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PokemonGo.WPF.Converters
{
    public class GymToImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var gym = value as GymViewModel;
            if(gym != null)
                return $"pack://siteoforigin:,,,/Images/Map/gymLogo_{gym.Team.ToLowerInvariant()}.png";
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}