using Microsoft.Maps.MapControl.WPF;
using PokemonGo.Bot.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PokemonGo.WPF.Converters
{
    public class PositionToMapLocationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var position = value as PositionViewModel;
            if (position != null)
                return new Location(position.Latitude, position.Longitude);
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var location = value as Location;
            if (location != null)
                return new PositionViewModel(location.Latitude, location.Longitude);
            return null;
        }
    }
}