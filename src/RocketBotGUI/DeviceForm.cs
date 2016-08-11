using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.Window
{
    public partial class DeviceForm : Form
    {
        private DeviceHelper _deviceHelper;
        private List<DeviceInfo> _deviceInfos;
        private bool _doNotPopulate;

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
                DeviceIdTb.Text = Settings.Instance.DeviceId;
                AndroidBoardNameTb.Text = Settings.Instance.AndroidBoardName;
                AndroidBootloaderTb.Text = Settings.Instance.AndroidBootloader;
                DeviceBrandTb.Text = Settings.Instance.DeviceBrand;
                DeviceModelTb.Text = Settings.Instance.DeviceModel;
                DeviceModelIdentifierTb.Text = Settings.Instance.DeviceModelIdentifier;
                DeviceModelBootTb.Text = Settings.Instance.DeviceModelBoot;
                HardwareManufacturerTb.Text = Settings.Instance.HardwareManufacturer;
                HardwareModelTb.Text = Settings.Instance.HardwareModel;
                FirmwareBrandTb.Text = Settings.Instance.FirmwareBrand;
                FirmwareTagsTb.Text = Settings.Instance.FirmwareTags;
                FirmwareTypeTb.Text = Settings.Instance.FirmwareType;
                FirmwareFingerprintTb.Text = Settings.Instance.FirmwareFingerprint;
                _doNotPopulate = true;
                deviceTypeCb.SelectedIndex = Settings.Instance.DeviceBrand.ToLower() == "apple" ? 0 : 1;
                _doNotPopulate = false;
            }
        }

        private void RandomDeviceBtn_Click(object sender, EventArgs e)
        {
            PopulateDevice();
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            if (DeviceIdTb.Text == "8525f6d8251f71b7")
            {
                PopulateDevice();
            }
            else
            {
                SaveToSetting();
            }
            Close();
        }

        private void deviceTypeCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateDevice();
        }

        private void RandomIDBtn_Click(object sender, EventArgs e)
        {
            DeviceIdTb.Text = _deviceHelper.RandomString(16, "0123456789abcdef");
        }

        private void PopulateDevice()
        {
            if (_doNotPopulate)
            {
                return;
            }
            var candidateDevices = deviceTypeCb.SelectedIndex == 0
                ? _deviceInfos.Where(d => d.DeviceBrand.ToLower() == "apple").ToList()
                : _deviceInfos.Where(d => d.DeviceBrand.ToLower() != "apple").ToList();
            var selectIndex = _deviceHelper.GetRandomIndex(candidateDevices.Count);

            DeviceIdTb.Text = _deviceInfos[selectIndex].DeviceId == "8525f6d8251f71b7"
                ? _deviceHelper.RandomString(16, "0123456789abcdef")
                : _deviceInfos[selectIndex].DeviceId;
            AndroidBoardNameTb.Text = _deviceInfos[selectIndex].AndroidBoardName;
            AndroidBootloaderTb.Text = _deviceInfos[selectIndex].AndroidBootloader;
            DeviceBrandTb.Text = _deviceInfos[selectIndex].DeviceBrand;
            DeviceModelTb.Text = _deviceInfos[selectIndex].DeviceModel;
            DeviceModelIdentifierTb.Text = _deviceInfos[selectIndex].DeviceModelIdentifier;
            DeviceModelBootTb.Text = _deviceInfos[selectIndex].DeviceModelBoot;
            HardwareManufacturerTb.Text = _deviceInfos[selectIndex].HardwareManufacturer;
            HardwareModelTb.Text = _deviceInfos[selectIndex].HardwareModel;
            FirmwareBrandTb.Text = _deviceInfos[selectIndex].FirmwareBrand;
            FirmwareTagsTb.Text = _deviceInfos[selectIndex].FirmwareTags;
            FirmwareTypeTb.Text = _deviceInfos[selectIndex].FirmwareType;
            FirmwareFingerprintTb.Text = _deviceInfos[selectIndex].FirmwareFingerprint;
            SaveToSetting();
        }

        private void SaveToSetting()
        {
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
            Settings.Instance.FirmwareTags = FirmwareTypeTb.Text;
            Settings.Instance.FirmwareType = FirmwareFingerprintTb.Text;
            Settings.Instance.Reload();
        }
    }
}