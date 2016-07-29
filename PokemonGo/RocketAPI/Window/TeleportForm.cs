using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.Window
{
    public partial class TeleportForm : Form
    {
        public TeleportForm()
        {
            InitializeComponent();
        }

        private async void teleportButton_Click(object sender, EventArgs e)
        {
            await MainForm.Teleport(MainForm.getClient(), Convert.ToDouble(latitudeText.Text), Convert.ToDouble(longitudeText.Text));
        }

        private void TeleportForm_Load(object sender, EventArgs e)
        {
            Client client = MainForm.getClient();
            latitudeText.Text = Convert.ToString(client.getCurrentLat());
            longitudeText.Text = Convert.ToString(client.getCurrentLong());

        }
    }
}
