using PokemonGo.Bot.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PokemonGo.WPF.Converters
{
    public class PokestopToImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var pokestop = value as PokestopViewModel;
            if (pokestop != null)
            {
                var isActive = pokestop.IsActive ? "active" : "inactive";
                var isNear = false ? "near" : "far";
                return $"pack://siteoforigin:,,,/Images/Map/pokestop_{isActive}_{isNear}.png";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}