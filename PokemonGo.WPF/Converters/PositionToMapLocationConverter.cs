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
            var position3d = value as Position3DViewModel;
            if (position3d != null)
                return new Location(position3d.Latitude, position3d.Longitude, position3d.Altitute);

            var position2d = value as Position2DViewModel;
            if (position2d != null)
                return new Location(position2d.Latitude, position2d.Longitude);

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var location = value as Location;
            if (location != null)
            {
                if(targetType == typeof(Position3DViewModel))
                    return new Position3DViewModel(location.Latitude, location.Longitude, location.Altitude);
                if (targetType == typeof(Position2DViewModel))
                    return new Position2DViewModel(location.Latitude, location.Longitude);
            }
            return null;
        }
    }
}