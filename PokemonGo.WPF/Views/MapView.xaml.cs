using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PokemonGo.WPF.Views
{
    /// <summary>
    /// Interaction logic for MapView.xaml
    /// </summary>
    public partial class MapView : UserControl
    {
        public MapView()
        {
            InitializeComponent();
        }

        private void Map_Loaded(object sender, RoutedEventArgs e)
        {
            var map = (Map)sender;
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            timer.Tick += (_, __) =>
            {
                var newCenter = new Location(map.Center);
                newCenter.Latitude += 0.00000000001;
                var oldCenter = map.Center;
                map.SetView(newCenter, map.ZoomLevel);
                map.SetView(oldCenter, map.ZoomLevel);
            };
            timer.Start();
        }
    }
}
