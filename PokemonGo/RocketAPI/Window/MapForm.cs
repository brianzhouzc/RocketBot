using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;

namespace PokemonGo.RocketAPI.Window
{
    public partial class MapForm : Form
    {
        private PokemonGo.RocketAPI.Client Client;

        public MapForm(ref PokemonGo.RocketAPI.Client client)
        {
            InitializeComponent();

            // Set the client
            Client = client;

            // Initialize map:
            // Use google provider
            gMapControl1.MapProvider = GoogleMapProvider.Instance;
            
            // Get tiles from server only
            gMapControl1.Manager.Mode = AccessMode.ServerOnly;
            
            // Do not use proxy
            GMapProvider.WebProxy = null;

            // Zoom min/max
            gMapControl1.CenterPen = new Pen(Color.Red, 2);
            gMapControl1.MinZoom = trackBar.Maximum = 1;
            gMapControl1.MaxZoom = trackBar.Maximum = 20;

            // Set zoom
            trackBar.Value = 17;
            gMapControl1.Zoom = trackBar.Value;
        }

        private void trackBar_Scroll(object sender, EventArgs e)
        {
            gMapControl1.Zoom = trackBar.Value;
        }

        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            gMapControl1.Position = new PointLatLng(Client.getCurrentLat(), Client.getCurrentLong());
        }
    }
}
