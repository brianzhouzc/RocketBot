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
            var position = value as Position3DViewModel;
            if (position != null)
                return new Location(position.Latitude, position.Longitude, position.Altitute);
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var location = value as Location;
            if (location != null)
                return new Position3DViewModel(location.Latitude, location.Longitude, location.Altitude);
            return null;
        }
    }
}