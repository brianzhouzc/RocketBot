using GalaSoft.MvvmLight.Command;
using Microsoft.Maps.MapControl.WPF;
using PokemonGo.Bot.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PokemonGo.WPF.Converters
{
    public class EventArgsToPositionConverter : IEventArgsConverter
    {
        public object Convert(object value, object parameter)
        {
            var mouseButtonEventArgs = value as MouseButtonEventArgs;
            var map = mouseButtonEventArgs?.Source as Map;
            if(mouseButtonEventArgs != null && map != null)
            {
                var mousePosition = mouseButtonEventArgs.GetPosition(map);
                Location location;
                if(map.TryViewportPointToLocation(mousePosition, out location))
                {
                    return new PositionToMapLocationConverter().ConvertBack(location, typeof(Position3DViewModel), null, CultureInfo.CurrentCulture);
                }
            }

            return null;
        }
    }
}
