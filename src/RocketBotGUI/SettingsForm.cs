using System;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using POGOProtos.Enums;

namespace PokemonGo.RocketAPI.Window
{
    internal partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            AdressBox.KeyDown += AdressBox_KeyDown;
            cbSelectAllCatch.CheckedChanged += CbSelectAllCatch_CheckedChanged;
            cbSelectAllTransfer.CheckedChanged += CbSelectAllTransfer_CheckedChanged;
            cbSelectAllEvolve.CheckedChanged += CbSelectAllEvolve_CheckedChanged;

            foreach (PokemonId id in Enum.GetValues(typeof(PokemonId))) {
                if (id == PokemonId.Missingno) continue;
                clbCatch.Items.Add(id);
                clbTransfer.Items.Add(id);
                clbEvolve.Items.Add(id);
            }
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            authTypeCb.Text = Settings.Instance.AuthType.ToString();
            if (authTypeCb.Text == "google")
            {
                UserLoginBox.Text = Settings.Instance.GoogleUsername;
                UserPasswordBox.Text = Settings.Instance.GooglePassword;
            }
            else
            {
                UserLoginBox.Text = Settings.Instance.PtcUsername;
                UserPasswordBox.Text = Settings.Instance.PtcPassword;
            }
            latitudeText.Text = Settings.Instance.DefaultLatitude.ToString();
            longitudeText.Text = Settings.Instance.DefaultLongitude.ToString();
            razzmodeCb.Text = Settings.Instance.RazzBerryMode;
            razzSettingText.Text = Settings.Instance.RazzBerrySetting.ToString();
            transferTypeCb.Text = Settings.Instance.TransferType;
            transferCpThresText.Text = Settings.Instance.TransferCpThreshold.ToString();
            transferIVThresText.Text = Settings.Instance.TransferIvThreshold.ToString();
            evolveAllChk.Checked = Settings.Instance.EvolveAllGivenPokemons;
            CatchPokemonBox.Checked = Settings.Instance.CatchPokemon;
            TravelSpeedBox.Text = Settings.Instance.TravelSpeed.ToString();
            // ImageSizeBox.Text = Settings.Instance.ImageSize.ToString();
            // Initialize map:
            //use google provider
            gMapControl1.MapProvider = GoogleMapProvider.Instance;
            //get tiles from server only
            gMapControl1.Manager.Mode = AccessMode.ServerOnly;
            //not use proxy
            GMapProvider.WebProxy = null;
            //center map on moscow
            var lat = ConfigurationManager.AppSettings["DefaultLatitude"];
            var longit = ConfigurationManager.AppSettings["DefaultLongitude"];
            lat.Replace(',', '.');
            longit.Replace(',', '.');
            gMapControl1.Position = new PointLatLng(double.Parse(lat.Replace(",", "."), CultureInfo.InvariantCulture),
                double.Parse(longit.Replace(",", "."), CultureInfo.InvariantCulture));


            //zoom min/max; default both = 2
            gMapControl1.DragButton = MouseButtons.Left;

            gMapControl1.CenterPen = new Pen(Color.Red, 2);
            gMapControl1.MinZoom = trackBar.Maximum = 1;
            gMapControl1.MaxZoom = trackBar.Maximum = 20;
            trackBar.Value = 10;

            //set zoom
            gMapControl1.Zoom = trackBar.Value;

            //disable map focus
            gMapControl1.DisableFocusOnMouseEnter = true;

            foreach (var pokemonIdSetting in Settings.Instance.ExcludedPokemonCatch) {
                for (int i = 0; i < clbCatch.Items.Count; i++) {
                    PokemonId pokemonId = (PokemonId)clbCatch.Items[i];
                    if (pokemonIdSetting == pokemonId) {
                        clbCatch.SetItemChecked(i, true);
                    }
                }
            }

            foreach (var pokemonIdSetting in Settings.Instance.ExcludedPokemonTransfer) {
                for (int i = 0; i < clbTransfer.Items.Count; i++) {
                    PokemonId pokemonId = (PokemonId)clbTransfer.Items[i];
                    if (pokemonIdSetting == pokemonId) {
                        clbTransfer.SetItemChecked(i, true);
                    }
                }
            }

            foreach (var pokemonIdSetting in Settings.Instance.ExcludedPokemonEvolve) {
                for (int i = 0; i < clbEvolve.Items.Count; i++) {
                    PokemonId pokemonId = (PokemonId)clbEvolve.Items[i];
                    if (pokemonIdSetting == pokemonId) {
                        clbEvolve.SetItemChecked(i, true);
                    }
                }
            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            Settings.Instance.SetSetting(authTypeCb.Text, "AuthType");
            if (authTypeCb.Text == "google")
            {
                Settings.Instance.SetSetting(UserLoginBox.Text, "GoogleUsername");
                Settings.Instance.SetSetting(UserPasswordBox.Text, "GooglePassword");
            }
            else
            {
                Settings.Instance.SetSetting(UserLoginBox.Text, "PtcUsername");
                Settings.Instance.SetSetting(UserPasswordBox.Text, "PtcPassword");
            }
            Settings.Instance.SetSetting(latitudeText.Text.Replace(',', '.'), "DefaultLatitude");
            Settings.Instance.SetSetting(longitudeText.Text.Replace(',', '.'), "DefaultLongitude");

            var lat = ConfigurationManager.AppSettings["DefaultLatitude"];
            var longit = ConfigurationManager.AppSettings["DefaultLongitude"];
            lat.Replace(',', '.');
            longit.Replace(',', '.');


            Settings.Instance.SetSetting(razzmodeCb.Text, "RazzBerryMode");
            Settings.Instance.SetSetting(razzSettingText.Text, "RazzBerrySetting");
            Settings.Instance.SetSetting(transferTypeCb.Text, "TransferType");
            Settings.Instance.SetSetting(transferCpThresText.Text, "TransferCPThreshold");
            Settings.Instance.SetSetting(transferIVThresText.Text, "TransferIVThreshold");
            Settings.Instance.SetSetting(TravelSpeedBox.Text, "TravelSpeed");
            //Settings.Instance.SetSetting(ImageSizeBox.Text, "ImageSize");
            Settings.Instance.SetSetting(evolveAllChk.Checked ? "true" : "false", "EvolveAllGivenPokemons");
            Settings.Instance.SetSetting(CatchPokemonBox.Checked ? "true" : "false", "CatchPokemon");
            Settings.Instance.ExcludedPokemonCatch = clbCatch.CheckedItems.Cast<PokemonId>().ToList();
            Settings.Instance.ExcludedPokemonTransfer = clbTransfer.CheckedItems.Cast<PokemonId>().ToList();
            Settings.Instance.ExcludedPokemonEvolve = clbEvolve.CheckedItems.Cast<PokemonId>().ToList();
            Settings.Instance.Reload();

            MainForm.ResetMap();
            Close();
        }

        private void authTypeCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (authTypeCb.Text == "google")
            {
                UserLabel.Text = "Email:";
            }
            else
            {
                UserLabel.Text = "Username:";
            }
        }

        private void gMapControl1_MouseClick(object sender, MouseEventArgs e)
        {
            var localCoordinates = e.Location;
            gMapControl1.Position = gMapControl1.FromLocalToLatLng(localCoordinates.X, localCoordinates.Y);

            if (e.Clicks >= 2)
            {
                gMapControl1.Zoom += 5;
            }

            var X = Math.Round(gMapControl1.Position.Lng, 6);
            var Y = Math.Round(gMapControl1.Position.Lat, 6);
            var longitude = X.ToString();
            var latitude = Y.ToString();
            latitudeText.Text = latitude;
            longitudeText.Text = longitude;
        }

        private void trackBar_Scroll(object sender, EventArgs e)
        {
            gMapControl1.Zoom = trackBar.Value;
        }

        private void FindAdressButton_Click(object sender, EventArgs e)
        {
            gMapControl1.SetPositionByKeywords(AdressBox.Text);
            gMapControl1.Zoom = 15;
        }

        private void transferTypeCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (transferTypeCb.Text == "CP")
            {
                label4.Visible = true;
                transferCpThresText.Visible = true;
            }
            else
            {
                label4.Visible = false;
                transferCpThresText.Visible = false;
            }

            if (transferTypeCb.Text == "IV")
            {
                label6.Visible = true;
                transferIVThresText.Visible = true;
            }
            else
            {
                label6.Visible = false;
                transferIVThresText.Visible = false;
            }
        }

        private void FindAdressButton_Click_1(object sender, EventArgs e)
        {
            gMapControl1.SetPositionByKeywords(AdressBox.Text);
            gMapControl1.Zoom = 15;
            var X = Math.Round(gMapControl1.Position.Lng, 6);
            var Y = Math.Round(gMapControl1.Position.Lat, 6);
            var longitude = X.ToString();
            var latitude = Y.ToString();
            latitudeText.Text = latitude;
            longitudeText.Text = longitude;
        }

        private void TravelSpeedBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            var ch = e.KeyChar;
            if (!char.IsDigit(ch) && ch != 8)
            {
                e.Handled = true;
            }
        }

        private void razzSettingText_KeyPress(object sender, KeyPressEventArgs e)
        {
            var ch = e.KeyChar;
            if (!char.IsDigit(ch) && ch != 8)
            {
                e.Handled = true;
            }
        }

        private void AdressBox_Leave(object sender, EventArgs e)
        {
            if (AdressBox.Text.Length == 0)
            {
                AdressBox.Text = "Enter an address or a coordinate";
                AdressBox.ForeColor = SystemColors.GrayText;
            }
        }

        private void AdressBox_Enter(object sender, EventArgs e)
        {
            if (AdressBox.Text == "Enter an address or a coordinate")
            {
                AdressBox.Text = "";
                AdressBox.ForeColor = SystemColors.WindowText;
            }
        }

        private void CbSelectAllEvolve_CheckedChanged(object sender, EventArgs e) {
            for (int i = 0; i < clbEvolve.Items.Count; i++) {
                clbEvolve.SetItemChecked(i, cbSelectAllEvolve.Checked);
            }
        }

        private void CbSelectAllTransfer_CheckedChanged(object sender, EventArgs e) {
            for (int i = 0; i < clbTransfer.Items.Count; i++) {
                clbTransfer.SetItemChecked(i, cbSelectAllTransfer.Checked);
            }
        }

        private void CbSelectAllCatch_CheckedChanged(object sender, EventArgs e) {
            for (int i = 0; i < clbCatch.Items.Count; i++) {
                clbCatch.SetItemChecked(i, cbSelectAllCatch.Checked);
            }
        }

        private void AdressBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                gMapControl1.SetPositionByKeywords(AdressBox.Text);
                gMapControl1.Zoom = 15;
            }
        }
    }
}