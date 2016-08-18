using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using PokemonGo.RocketBot.Logic;
using PokemonGo.RocketBot.Window.Helpers;
using POGOProtos.Enums;
using POGOProtos.Inventory.Item;

namespace PokemonGo.RocketBot.Window.Forms
{
    internal partial class SettingsForm : Form
    {
        private DeviceHelper _deviceHelper;
        private List<DeviceInfo> _deviceInfos;
        //private bool _doNotPopulate;

        private List<ItemId> itemSettings = new List<ItemId>
        {
            ItemId.ItemPokeBall,
            ItemId.ItemGreatBall,
            ItemId.ItemUltraBall,
            ItemId.ItemRazzBerry,
            ItemId.ItemPotion,
            ItemId.ItemSuperPotion,
            ItemId.ItemHyperPotion,
            ItemId.ItemMaxPotion,
            ItemId.ItemRevive,
            ItemId.ItemMaxRevive
        };

        public SettingsForm()
        {
            InitializeComponent();
            AdressBox.KeyDown += AdressBox_KeyDown;
            cbSelectAllCatch.CheckedChanged += CbSelectAllCatch_CheckedChanged;
            cbSelectAllTransfer.CheckedChanged += CbSelectAllTransfer_CheckedChanged;
            cbSelectAllEvolve.CheckedChanged += CbSelectAllEvolve_CheckedChanged;

            foreach (PokemonId id in Enum.GetValues(typeof(PokemonId)))
            {
                if (id == PokemonId.Missingno) continue;
                clbCatch.Items.Add(id);
                clbTransfer.Items.Add(id);
                clbEvolve.Items.Add(id);
            }

            /**foreach (ItemId itemId in itemSettings)
            {
                ItemData item = new ItemData();
                item.ItemId = itemId;
                flpItems.Controls.Add(new ItemSetting(item));
            } **/
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            GlobalSettings.Load("");
            var settings = new GlobalSettings();

            authTypeCb.Text = settings.Auth.AuthType.ToString();
            if (authTypeCb.Text.ToLower().Equals("google"))
            {
                UserLoginBox.Text = settings.Auth.GoogleUsername;
                UserPasswordBox.Text = settings.Auth.GooglePassword;
            }
            else
            {
                UserLoginBox.Text = settings.Auth.PtcUsername;
                UserPasswordBox.Text = settings.Auth.PtcPassword;
            }
            latitudeText.Text = settings.DefaultLatitude.ToString();
            longitudeText.Text = settings.DefaultLongitude.ToString();
            //razzmodeCb.Text = Settings.Instance.RazzBerryMode;
            //razzSettingText.Text = Settings.Instance.RazzBerrySetting.ToString();

            //useIncubatorsCb.Text = Settings.Instance.UseIncubatorsMode.ToString();
            //transferTypeCb.Text = Settings.Instance.TransferType;
            //transferCpThresText.Text = Settings.Instance.TransferCpThreshold.ToString();
            //transferIVThresText.Text = Settings.Instance.TransferIvThreshold.ToString();
            //evolveAllChk.Checked = Settings.Instance.EvolveAllGivenPokemons;

            CatchPokemonBox.Checked = settings.CatchPokemon;
            TravelSpeedBox.Text = settings.WalkingSpeedInKilometerPerHour.ToString();
            // ImageSizeBox.Text = Settings.Instance.ImageSize.ToString();
            // Initialize map:
            //use google provider
            gMapControl1.MapProvider = GoogleMapProvider.Instance;
            //get tiles from server only
            gMapControl1.Manager.Mode = AccessMode.ServerOnly;
            //not use proxy
            GMapProvider.WebProxy = null;
            //center map on moscow
            gMapControl1.Position = new PointLatLng(settings.DefaultLatitude, settings.DefaultLongitude);

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

            foreach (var pokemonIdSetting in settings.PokemonsToIgnore)
            {
                for (var i = 0; i < clbCatch.Items.Count; i++)
                {
                    var pokemonId = (PokemonId)clbCatch.Items[i];
                    if (pokemonIdSetting == pokemonId)
                    {
                        clbCatch.SetItemChecked(i, true);
                    }
                }
            }

            foreach (var pokemonIdSetting in settings.PokemonsNotToTransfer)
            {
                for (var i = 0; i < clbTransfer.Items.Count; i++)
                {
                    var pokemonId = (PokemonId)clbTransfer.Items[i];
                    if (pokemonIdSetting == pokemonId)
                    {
                        clbTransfer.SetItemChecked(i, true);
                    }
                }
            }

            /**  foreach (var pokemonIdSetting in Settings.Instance.ExcludedPokemonEvolve)
            {
                for (var i = 0; i < clbEvolve.Items.Count; i++)
                {
                    var pokemonId = (PokemonId)clbEvolve.Items[i];
                    if (pokemonIdSetting == pokemonId)
                    {
                        clbEvolve.SetItemChecked(i, true);
                    }
                }
            } **/

            /** var itemCounts = Settings.Instance.ItemCounts;
            foreach (ItemSetting itemSetting in flpItems.Controls)
            {
                foreach (var itemCount in itemCounts)
                {
                    if (itemSetting.ItemData.ItemId == itemCount.ItemId)
                    {
                        itemSetting.ItemData = itemCount;
                    }
                }
            } **/

            // Device settings
            _deviceHelper = new DeviceHelper();
            _deviceInfos = _deviceHelper.DeviceBucket;

            if (settings.Auth.DeviceId == "41aeed0990284b37")
            {
                PopulateDevice();
            }
            else
            {
                DeviceIdTb.Text = settings.Auth.DeviceId;
                AndroidBoardNameTb.Text = settings.Auth.AndroidBoardName;
                AndroidBootloaderTb.Text = settings.Auth.AndroidBootloader;
                DeviceBrandTb.Text = settings.Auth.DeviceBrand;
                DeviceModelTb.Text = settings.Auth.DeviceModel;
                DeviceModelIdentifierTb.Text = settings.Auth.DeviceModelIdentifier;
                DeviceModelBootTb.Text = settings.Auth.DeviceModelBoot;
                HardwareManufacturerTb.Text = settings.Auth.HardwareManufacturer;
                HardwareModelTb.Text = settings.Auth.HardwareModel;
                FirmwareBrandTb.Text = settings.Auth.FirmwareBrand;
                FirmwareTagsTb.Text = settings.Auth.FirmwareTags;
                FirmwareTypeTb.Text = settings.Auth.FirmwareType;
                FirmwareFingerprintTb.Text = settings.Auth.FirmwareFingerprint;
                deviceTypeCb.SelectedIndex = settings.Auth.DeviceBrand.ToLower() == "apple" ? 0 : 1;
            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            /** Settings.Instance.SetSetting(authTypeCb.Text, "AuthType");
            if (authTypeCb.Text.ToLower().Equals("google"))
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
            Settings.Instance.SetSetting(useIncubatorsCb.Text, "useIncubatorsMode");
            Settings.Instance.SetSetting(TravelSpeedBox.Text, "TravelSpeed");
            //Settings.Instance.SetSetting(ImageSizeBox.Text, "ImageSize");
            Settings.Instance.SetSetting(evolveAllChk.Checked ? "true" : "false", "EvolveAllGivenPokemons");
            Settings.Instance.SetSetting(CatchPokemonBox.Checked ? "true" : "false", "CatchPokemon");
            Settings.Instance.ExcludedPokemonCatch = clbCatch.CheckedItems.Cast<PokemonId>().ToList();
            Settings.Instance.ExcludedPokemonTransfer = clbTransfer.CheckedItems.Cast<PokemonId>().ToList();
            Settings.Instance.ExcludedPokemonEvolve = clbEvolve.CheckedItems.Cast<PokemonId>().ToList();
            Settings.Instance.ItemCounts = flpItems.Controls.Cast<ItemSetting>().Select(i => i.ItemData).ToList();
            Settings.Instance.Reload();

            //Device settings
            Settings.Instance.DeviceId = DeviceIdTb.Text;
            Settings.Instance.AndroidBoardName = AndroidBoardNameTb.Text;
            Settings.Instance.AndroidBootloader = AndroidBootloaderTb.Text;
            Settings.Instance.DeviceBrand = DeviceBrandTb.Text;
            Settings.Instance.DeviceModel = DeviceModelTb.Text;
            Settings.Instance.DeviceModelIdentifier = DeviceModelIdentifierTb.Text;
            Settings.Instance.DeviceModelBoot = DeviceModelBootTb.Text;
            Settings.Instance.HardwareManufacturer = HardwareManufacturerTb.Text;
            Settings.Instance.HardwareModel = HardwareModelTb.Text;
            Settings.Instance.FirmwareBrand = FirmwareBrandTb.Text;
            Settings.Instance.FirmwareTags = FirmwareTagsTb.Text;
            Settings.Instance.FirmwareType = FirmwareTypeTb.Text;
            Settings.Instance.FirmwareFingerprint = FirmwareFingerprintTb.Text;
            if (DeviceIdTb.Text == "8525f6d8251f71b7")
            {
                PopulateDevice();
            }

            MainForm.ResetMap();
            Close(); **/
        }

        private void authTypeCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (authTypeCb.Text.ToLower().Equals("google"))
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

        private void FindAdressButton_Click(object sender, EventArgs e)
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

        private void CbSelectAllEvolve_CheckedChanged(object sender, EventArgs e)
        {
            for (var i = 0; i < clbEvolve.Items.Count; i++)
            {
                clbEvolve.SetItemChecked(i, cbSelectAllEvolve.Checked);
            }
        }

        private void CbSelectAllTransfer_CheckedChanged(object sender, EventArgs e)
        {
            for (var i = 0; i < clbTransfer.Items.Count; i++)
            {
                clbTransfer.SetItemChecked(i, cbSelectAllTransfer.Checked);
            }
        }

        private void CbSelectAllCatch_CheckedChanged(object sender, EventArgs e)
        {
            for (var i = 0; i < clbCatch.Items.Count; i++)
            {
                clbCatch.SetItemChecked(i, cbSelectAllCatch.Checked);
            }
        }

        private void AdressBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                gMapControl1.SetPositionByKeywords(AdressBox.Text);
                gMapControl1.Zoom = 15;
            }
        }

        private void RandomDeviceBtn_Click(object sender, EventArgs e)
        {
            PopulateDevice();
        }

        private void deviceTypeCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateDevice(deviceTypeCb.SelectedIndex);
        }

        private void RandomIDBtn_Click(object sender, EventArgs e)
        {
            DeviceIdTb.Text = _deviceHelper.RandomString(16, "0123456789abcdef");
        }

        private void PopulateDevice(int tabIndex = -1)
        {
            deviceTypeCb.SelectedIndex = tabIndex == -1 ? _deviceHelper.GetRandomIndex(2) : tabIndex;
            var candidateDevices = deviceTypeCb.SelectedIndex == 0
                ? _deviceInfos.Where(d => d.DeviceBrand.ToLower() == "apple").ToList()
                : _deviceInfos.Where(d => d.DeviceBrand.ToLower() != "apple").ToList();
            var selectIndex = _deviceHelper.GetRandomIndex(candidateDevices.Count);

            DeviceIdTb.Text = candidateDevices[selectIndex].DeviceId == "8525f6d8251f71b7"
                ? _deviceHelper.RandomString(16, "0123456789abcdef")
                : candidateDevices[selectIndex].DeviceId;
            AndroidBoardNameTb.Text = candidateDevices[selectIndex].AndroidBoardName;
            AndroidBootloaderTb.Text = candidateDevices[selectIndex].AndroidBootloader;
            DeviceBrandTb.Text = candidateDevices[selectIndex].DeviceBrand;
            DeviceModelTb.Text = candidateDevices[selectIndex].DeviceModel;
            DeviceModelIdentifierTb.Text = candidateDevices[selectIndex].DeviceModelIdentifier;
            DeviceModelBootTb.Text = candidateDevices[selectIndex].DeviceModelBoot;
            HardwareManufacturerTb.Text = candidateDevices[selectIndex].HardwareManufacturer;
            HardwareModelTb.Text = candidateDevices[selectIndex].HardwareModel;
            FirmwareBrandTb.Text = candidateDevices[selectIndex].FirmwareBrand;
            FirmwareTagsTb.Text = candidateDevices[selectIndex].FirmwareTags;
            FirmwareTypeTb.Text = candidateDevices[selectIndex].FirmwareType;
            FirmwareFingerprintTb.Text = candidateDevices[selectIndex].FirmwareFingerprint;
        }
    }
}