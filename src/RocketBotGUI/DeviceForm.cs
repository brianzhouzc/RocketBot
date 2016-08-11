using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.Window
{
    public partial class DeviceForm : Form
    {
        private List<DeviceInfo> _deviceInfos;
        private DeviceHelper _deviceHelper;
        private bool _doNotPopulate = false;
        public DeviceForm()
        {
            InitializeComponent();
        }

        private void DeviceForm_Load(object sender, EventArgs e)
        {
            _deviceHelper = new DeviceHelper();
            _deviceInfos = _deviceHelper.DeviceBucket;

            if (Settings.Instance.DeviceId == "8525f6d8251f71b7")
            {
                PopulateDevice();
            }
            else
            {
                this.DeviceIdTb.Text = Settings.Instance.DeviceId;
                this.AndroidBoardNameTb.Text = Settings.Instance.AndroidBoardName;
                this.AndroidBootloaderTb.Text = Settings.Instance.AndroidBootloader;
                this.DeviceBrandTb.Text = Settings.Instance.DeviceBrand;
                this.DeviceModelTb.Text = Settings.Instance.DeviceModel;
                this.DeviceModelIdentifierTb.Text = Settings.Instance.DeviceModelIdentifier;
                this.DeviceModelBootTb.Text = Settings.Instance.DeviceModelBoot;
                this.HardwareManufacturerTb.Text = Settings.Instance.HardwareManufacturer;
                this.HardwareModelTb.Text = Settings.Instance.HardwareModel;
                this.FirmwareBrandTb.Text = Settings.Instance.FirmwareBrand;
                this.FirmwareTagsTb.Text = Settings.Instance.FirmwareTags;
                this.FirmwareTypeTb.Text = Settings.Instance.FirmwareType;
                this.FirmwareFingerprintTb.Text = Settings.Instance.FirmwareFingerprint;
                _doNotPopulate = true;
                this.deviceTypeCb.SelectedIndex = Settings.Instance.DeviceBrand.ToLower() == "apple" ? 0 : 1;
                _doNotPopulate = false;
            }
        }

        private void RandomDeviceBtn_Click(object sender, EventArgs e)
        {
            PopulateDevice();
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            if (this.DeviceIdTb.Text == "8525f6d8251f71b7")
            {
                PopulateDevice();
            }
            else
            {
                SaveToSetting();
            }
            this.Close();
        }

        private void deviceTypeCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateDevice();
        }

        private void RandomIDBtn_Click(object sender, EventArgs e)
        {
            this.DeviceIdTb.Text = _deviceHelper.RandomString(16, "0123456789abcdef");
        }

        private void PopulateDevice()
        {
            if (_doNotPopulate)
            {
                return;
            }
            var candidateDevices = deviceTypeCb.SelectedIndex == 0 ? _deviceInfos.Where(d => d.DeviceBrand.ToLower() == "apple").ToList() : _deviceInfos.Where(d => d.DeviceBrand.ToLower() != "apple").ToList();
            int selectIndex = _deviceHelper.GetRandomIndex(candidateDevices.Count);

            this.DeviceIdTb.Text = _deviceInfos[selectIndex].DeviceId == "8525f6d8251f71b7" ? _deviceHelper.RandomString(16, "0123456789abcdef") : _deviceInfos[selectIndex].DeviceId;
            this.AndroidBoardNameTb.Text = _deviceInfos[selectIndex].AndroidBoardName;
            this.AndroidBootloaderTb.Text = _deviceInfos[selectIndex].AndroidBootloader;
            this.DeviceBrandTb.Text = _deviceInfos[selectIndex].DeviceBrand;
            this.DeviceModelTb.Text = _deviceInfos[selectIndex].DeviceModel;
            this.DeviceModelIdentifierTb.Text = _deviceInfos[selectIndex].DeviceModelIdentifier;
            this.DeviceModelBootTb.Text = _deviceInfos[selectIndex].DeviceModelBoot;
            this.HardwareManufacturerTb.Text = _deviceInfos[selectIndex].HardwareManufacturer;
            this.HardwareModelTb.Text = _deviceInfos[selectIndex].HardwareModel;
            this.FirmwareBrandTb.Text = _deviceInfos[selectIndex].FirmwareBrand;
            this.FirmwareTagsTb.Text = _deviceInfos[selectIndex].FirmwareTags;
            this.FirmwareTypeTb.Text = _deviceInfos[selectIndex].FirmwareType;
            this.FirmwareFingerprintTb.Text = _deviceInfos[selectIndex].FirmwareFingerprint;
            SaveToSetting();
        }

        private void SaveToSetting()
        {
            Settings.Instance.DeviceId = this.DeviceIdTb.Text;
            Settings.Instance.AndroidBoardName = this.AndroidBoardNameTb.Text;
            Settings.Instance.AndroidBootloader = this.AndroidBootloaderTb.Text;
            Settings.Instance.DeviceBrand = this.DeviceBrandTb.Text;
            Settings.Instance.DeviceModel = this.DeviceModelTb.Text;
            Settings.Instance.DeviceModelIdentifier = this.DeviceModelIdentifierTb.Text;
            Settings.Instance.DeviceModelBoot = this.DeviceModelBootTb.Text;
            Settings.Instance.HardwareManufacturer = this.HardwareManufacturerTb.Text;
            Settings.Instance.HardwareModel = this.HardwareModelTb.Text;
            Settings.Instance.FirmwareBrand = this.FirmwareBrandTb.Text;
            Settings.Instance.FirmwareTags = this.FirmwareTypeTb.Text;
            Settings.Instance.FirmwareType = this.FirmwareFingerprintTb.Text;
            Settings.Instance.Reload();
        }
    }
}
