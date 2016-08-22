using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketBot.Logic;
using PokemonGo.RocketBot.Window.Helpers;
using POGOProtos.Enums;
using System.Reflection;

namespace PokemonGo.RocketBot.Window.Forms
{
    internal partial class SettingsForm : Form
    {
        private const int DefaultZoomLevel = 15;

        private static readonly string ConfigFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Config");
        private static readonly string AuthFilePath = Path.Combine(ConfigFolderPath, "auth.json");
        private static readonly string ConfigFilePath = Path.Combine(ConfigFolderPath, "config.json");
        private static readonly string LanguagePath = Path.Combine(ConfigFolderPath, "Translations");
        private readonly DeviceHelper _deviceHelper;
        private readonly List<DeviceInfo> _deviceInfos;
        private readonly GlobalSettings _setting;
        private TabPage _tabAdvSettingTab;

        public SettingsForm(ref GlobalSettings settings)
        {
            InitializeComponent();
            _setting = settings;

            _deviceHelper = new DeviceHelper();
            _deviceInfos = _deviceHelper.DeviceBucket;

            foreach (
                var pokemon in
                    Enum.GetValues(typeof(PokemonId)).Cast<PokemonId>().Where(id => id != PokemonId.Missingno))
            {
                clbIgnore.Items.Add(pokemon);
                clbTransfer.Items.Add(pokemon);
                clbPowerUp.Items.Add(pokemon);
                clbEvolve.Items.Add(pokemon);
            }
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            GetLanguageList();

            #region Advanced Setting Init

            //proxy
            proxyGb.Visible = _setting.EnableAdvancedSettings;
            //advanced tab
            _tabAdvSettingTab = tabAdvSetting;
            enableAdvSettingCb.Checked = _setting.EnableAdvancedSettings;
            if (!enableAdvSettingCb.Checked)
            {
                tabControl.TabPages.Remove(_tabAdvSettingTab);
            }
            else
            {
                _tabAdvSettingTab.Enabled = true;
            }

            #endregion

            #region Login Type and info

            authTypeCb.Text = _setting.Auth.AuthType.ToString();
            UserLoginBox.Text = _setting.Auth.AuthType == AuthType.Google
                ? _setting.Auth.GoogleUsername
                : _setting.Auth.PtcUsername;
            UserPasswordBox.Text = _setting.Auth.AuthType == AuthType.Google
                ? _setting.Auth.GooglePassword
                : _setting.Auth.PtcPassword;

            //google api
            GoogleApiBox.Text = _setting.Auth.GoogleApiKey;

            //proxy
            useProxyCb.Checked = _setting.Auth.UseProxy;
            useProxyAuthCb.Checked = _setting.Auth.UseProxy && _setting.Auth.UseProxyAuthentication;
            ToggleProxyCtrls();

            #endregion

            #region Map Info

            //use google provider
            gMapCtrl.MapProvider = GoogleMapProvider.Instance;
            //get tiles from server only
            gMapCtrl.Manager.Mode = AccessMode.ServerOnly;
            //not use proxy
            GMapProvider.WebProxy = null;
            //center map on moscow
            gMapCtrl.Position = new PointLatLng(_setting.DefaultLatitude, _setting.DefaultLongitude);
            //zoom min/max; default both = 2
            gMapCtrl.DragButton = MouseButtons.Left;
            gMapCtrl.CenterPen = new Pen(Color.Red, 2);
            gMapCtrl.MinZoom = trackBar.Maximum = 2;
            gMapCtrl.MaxZoom = trackBar.Maximum = 18;
            trackBar.Value = DefaultZoomLevel;
            //set zoom
            gMapCtrl.Zoom = trackBar.Value;
            //disable map focus
            gMapCtrl.DisableFocusOnMouseEnter = true;

            tbWalkingSpeed.Text = _setting.WalkingSpeedInKilometerPerHour.ToString(CultureInfo.InvariantCulture);

            #endregion

            #region Device Info

            //by default, select one from Necro's device dictionary
            DeviceIdTb.Text = _setting.Auth.DeviceId;
            AndroidBoardNameTb.Text = _setting.Auth.AndroidBoardName;
            AndroidBootloaderTb.Text = _setting.Auth.AndroidBootloader;
            DeviceBrandTb.Text = _setting.Auth.DeviceBrand;
            DeviceModelTb.Text = _setting.Auth.DeviceModel;
            DeviceModelIdentifierTb.Text = _setting.Auth.DeviceModelIdentifier;
            DeviceModelBootTb.Text = _setting.Auth.DeviceModelBoot;
            HardwareManufacturerTb.Text = _setting.Auth.HardwareManufacturer;
            HardwareModelTb.Text = _setting.Auth.HardwareModel;
            FirmwareBrandTb.Text = _setting.Auth.FirmwareBrand;
            FirmwareTagsTb.Text = _setting.Auth.FirmwareTags;
            FirmwareTypeTb.Text = _setting.Auth.FirmwareType;
            FirmwareFingerprintTb.Text = _setting.Auth.FirmwareFingerprint;
            deviceTypeCb.SelectedIndex = _setting.Auth.DeviceBrand.ToLower() == "apple" ? 0 : 1;

            #endregion

            #region Pokemon Info

            #region Catch

            cbCatchPoke.Checked = _setting.CatchPokemon;
            cbUseEggIncubators.Checked = _setting.UseEggIncubators;
            tBMaxBerriesToUsePerPokemon.Text = _setting.MaxBerriesToUsePerPokemon.ToString();
            tbMaxPokeballsPerPokemon.Text = _setting.MaxPokeballsPerPokemon.ToString();
            cbAutoFavoritePokemon.Checked = _setting.AutoFavoritePokemon;
            tbFavoriteMinIvPercentage.Text = _setting.FavoriteMinIvPercentage.ToString(CultureInfo.InvariantCulture);

            tbUseBerriesMinCp.Text = _setting.UseBerriesMinCp.ToString();
            tbUseBerriesMinIv.Text = _setting.UseBerriesMinIv.ToString(CultureInfo.InvariantCulture);
            tbUseBerriesBelowCatchProbability.Text =
                _setting.UseBerriesBelowCatchProbability.ToString(CultureInfo.InvariantCulture);
            cbUseBerriesOperator.SelectedIndex = _setting.UseBerriesOperator == "and" ? 0 : 1;

            tbUseGreatBallAboveCp.Text = _setting.UseGreatBallAboveCp.ToString();
            tbUseUltraBallAboveCp.Text = _setting.UseUltraBallAboveCp.ToString();
            tbUseMasterBallAboveCp.Text = _setting.UseMasterBallAboveCp.ToString();
            tbUseGreatBallAboveIv.Text = _setting.UseGreatBallAboveIv.ToString(CultureInfo.InvariantCulture);
            tbUseUltraBallAboveIv.Text = _setting.UseUltraBallAboveIv.ToString(CultureInfo.InvariantCulture);
            tbUseGreatBallBelowCatchProbability.Text =
                _setting.UseGreatBallBelowCatchProbability.ToString(CultureInfo.InvariantCulture);
            tbUseUltraBallBelowCatchProbability.Text =
                _setting.UseUltraBallBelowCatchProbability.ToString(CultureInfo.InvariantCulture);
            tbUseMasterBallBelowCatchProbability.Text =
                _setting.UseMasterBallBelowCatchProbability.ToString(CultureInfo.InvariantCulture);

            foreach (var poke in _setting.PokemonsToIgnore)
            {
                clbIgnore.SetItemChecked(clbIgnore.FindStringExact(poke.ToString()), true);
            }

            #endregion

            #region Transfer

            cbPrioritizeIvOverCp.Checked = _setting.PrioritizeIvOverCp;
            tbKeepMinCp.Text = _setting.KeepMinCp.ToString();
            tbKeepMinIV.Text = _setting.KeepMinIvPercentage.ToString(CultureInfo.InvariantCulture);
            tbKeepMinLvl.Text = _setting.KeepMinLvl.ToString();
            cbKeepMinOperator.SelectedIndex = _setting.KeepMinOperator.ToLowerInvariant() == "and" ? 0 : 1;
            cbTransferWeakPokemon.Checked = _setting.TransferWeakPokemon;
            cbTransferDuplicatePokemon.Checked = _setting.TransferDuplicatePokemon;
            cbTransferDuplicatePokemonOnCapture.Checked = _setting.TransferDuplicatePokemonOnCapture;

            tbKeepMinDuplicatePokemon.Text = _setting.KeepMinDuplicatePokemon.ToString();
            cbUseKeepMinLvl.Checked = _setting.UseKeepMinLvl;
            foreach (var poke in _setting.PokemonsNotToTransfer)
            {
                clbTransfer.SetItemChecked(clbTransfer.FindStringExact(poke.ToString()), true);
            }

            #endregion

            #region Powerup

            //focuse to use filter list
            _setting.UseLevelUpList = true;

            cbAutoPowerUp.Checked = _setting.AutomaticallyLevelUpPokemon;
            cbPowerUpFav.Checked = _setting.OnlyUpgradeFavorites;
            cbPowerUpType.SelectedIndex = _setting.LevelUpByCPorIv == "iv" ? 0 : 1;
            cbPowerUpCondiction.SelectedIndex = _setting.UpgradePokemonMinimumStatsOperator == "and" ? 0 : 1;
            cbPowerUpMinStarDust.Text = _setting.GetMinStarDustForLevelUp.ToString();
            tbPowerUpMinIV.Text = _setting.UpgradePokemonIvMinimum.ToString(CultureInfo.InvariantCulture);
            tbPowerUpMinCP.Text = _setting.UpgradePokemonCpMinimum.ToString(CultureInfo.InvariantCulture);
            foreach (var poke in _setting.PokemonsToLevelUp)
            {
                clbPowerUp.SetItemChecked(clbPowerUp.FindStringExact(poke.ToString()), true);
            }
            if (_setting.LevelUpByCPorIv == "iv")
            {
                label31.Visible = true;
                tbPowerUpMinIV.Visible = true;
                label30.Visible = false;
                tbPowerUpMinCP.Visible = false;
            }
            else
            {
                label31.Visible = false;
                tbPowerUpMinIV.Visible = false;
                label30.Visible = true;
                tbPowerUpMinCP.Visible = true;
            }

            #endregion

            #region Evo

            cbEvoAllAboveIV.Checked = _setting.EvolveAllPokemonAboveIv;
            tbEvoAboveIV.Text = _setting.EvolveAboveIvValue.ToString(CultureInfo.InvariantCulture);
            cbEvolveAllPokemonWithEnoughCandy.Checked = _setting.EvolveAllPokemonWithEnoughCandy;
            cbKeepPokemonsThatCanEvolve.Checked = _setting.KeepPokemonsThatCanEvolve;
            tbEvolveKeptPokemonsAtStorageUsagePercentage.Text =
                _setting.EvolveKeptPokemonsAtStorageUsagePercentage.ToString(CultureInfo.InvariantCulture);
            cbUseLuckyEggsWhileEvolving.Checked = _setting.UseLuckyEggsWhileEvolving;
            tbUseLuckyEggsMinPokemonAmount.Text = _setting.UseLuckyEggsMinPokemonAmount.ToString();
            foreach (var poke in _setting.PokemonsToEvolve)
            {
                clbEvolve.SetItemChecked(clbEvolve.FindStringExact(poke.ToString()), true);
            }

            #endregion

            #endregion

            #region Item Info

            cbUseLuckyEggConstantly.Checked = _setting.UseLuckyEggConstantly;
            cbUseIncenseConstantly.Checked = _setting.UseIncenseConstantly;
            tbTotalAmountOfPokeballsToKeep.Text = _setting.TotalAmountOfPokeballsToKeep.ToString();
            tbTotalAmountOfPotionsToKeep.Text = _setting.TotalAmountOfPotionsToKeep.ToString();
            tbTotalAmountOfRevivesToKeep.Text = _setting.TotalAmountOfRevivesToKeep.ToString();
            tbTotalAmountOfBerriesToKeep.Text = _setting.TotalAmountOfBerriesToKeep.ToString();
            cbVerboseRecycling.Checked = _setting.VerboseRecycling;
            tbRecycleInventoryAtUsagePercentage.Text =
                _setting.RecycleInventoryAtUsagePercentage.ToString(CultureInfo.InvariantCulture);

            #endregion

            #region Advance Settings

            cbDisableHumanWalking.Checked = _setting.DisableHumanWalking;
            cbUseWalkingSpeedVariant.Checked = _setting.UseWalkingSpeedVariant;
            tbWalkingSpeedVariantInKilometerPerHour.Text =
                _setting.WalkingSpeedVariant.ToString(CultureInfo.InvariantCulture);
            cbShowWalkingSpeed.Checked = _setting.ShowVariantWalking;
            tbMaxSpawnLocationOffset.Text = _setting.MaxSpawnLocationOffset.ToString();
            tbMaxTravelDistanceInMeters.Text = _setting.MaxTravelDistanceInMeters.ToString();

            tbDelayBetweenPlayerActions.Text = _setting.DelayBetweenPlayerActions.ToString();
            tbDelayBetweenPokemonCatch.Text = _setting.DelayBetweenPokemonCatch.ToString();
            cbDelayBetweenRecycleActions.Checked = _setting.DelayBetweenRecycleActions;

            cbRandomizeRecycle.Checked = _setting.RandomizeRecycle;
            tbRandomRecycleValue.Text = _setting.RandomRecycleValue.ToString();

            cbEnableHumanizedThrows.Checked = _setting.EnableHumanizedThrows;
            tbNiceThrowChance.Text = _setting.NiceThrowChance.ToString();
            tbGreatThrowChance.Text = _setting.GreatThrowChance.ToString();
            tbExcellentThrowChance.Text = _setting.ExcellentThrowChance.ToString();
            tbCurveThrowChance.Text = _setting.CurveThrowChance.ToString();
            tbForceGreatThrowOverIv.Text = _setting.ForceGreatThrowOverIv.ToString(CultureInfo.InvariantCulture);
            tbForceExcellentThrowOverIv.Text = _setting.ForceExcellentThrowOverIv.ToString(CultureInfo.InvariantCulture);
            tbForceGreatThrowOverCp.Text = _setting.ForceGreatThrowOverCp.ToString();
            tbForceExcellentThrowOverCp.Text = _setting.ForceExcellentThrowOverCp.ToString();

            #endregion
        }

        #region private methods

        private static int ConvertStringToInt(string input)
        {
            var output = 0;
            int.TryParse(input, out output);
            return output;
        }

        private static float ConvertStringToFloat(string input)
        {
            float output = 0;
            float.TryParse(input, out output);
            return output;
        }

        private static double ConvertStringToDouble(string input)
        {
            double output = 0;
            double.TryParse(input, out output);
            return output;
        }

        private static List<PokemonId> ConvertClbToList(CheckedListBox input)
        {
            return input.CheckedItems.Cast<PokemonId>().ToList();
        }

        /// <summary>
        ///     Get languale list from Translations folder and populate it to combo box
        /// </summary>
        private void GetLanguageList()
        {
            var languages = new List<string> { "en" };
            var langFiles = Directory.GetFiles(LanguagePath, "*.json", SearchOption.TopDirectoryOnly);
            languages.AddRange(langFiles.Select(
                langFileName => Path.GetFileNameWithoutExtension(langFileName)?.Replace("translation.", ""))
                .Where(langCode => langCode != "en"));
            cbLanguage.DataSource = languages;
        }

        /// <summary>
        ///     Update location lat and lon to textboxes
        /// </summary>
        private void UpdateLocationInfo()
        {
            //not rounding it, need to have correct position to prevent map drifting
            tbLatitude.Text = gMapCtrl.Position.Lat.ToString(CultureInfo.InvariantCulture);
            tbLongitude.Text = gMapCtrl.Position.Lng.ToString(CultureInfo.InvariantCulture);
            //update trackbar
            trackBar.Value = (int)Math.Round(gMapCtrl.Zoom);
        }

        /// <summary>
        ///     Update map location base on giving lng and lat
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        private void UpdateMapLocation(double lat, double lng)
        {
            gMapCtrl.Position = new PointLatLng(lat, lng);
        }

        private void ToggleProxyCtrls()
        {
            if (useProxyCb.Checked)
            {
                proxyHostTb.Enabled = true;
                proxyHostTb.Text = _setting.Auth.UseProxyHost;
                proxyPortTb.Enabled = true;
                proxyPortTb.Text = _setting.Auth.UseProxyPort;
                useProxyAuthCb.Enabled = true;
            }
            else
            {
                proxyHostTb.Enabled = false;
                proxyHostTb.Text = _setting.Auth.UseProxyHost = "";
                proxyPortTb.Enabled = false;
                proxyPortTb.Text = _setting.Auth.UseProxyPort = "";
                useProxyAuthCb.Enabled = false;
            }
            if (useProxyAuthCb.Checked)
            {
                proxyUserTb.Enabled = true;
                proxyUserTb.Text = _setting.Auth.UseProxyUsername;
                proxyPwTb.Enabled = true;
                proxyPwTb.Text = _setting.Auth.UseProxyPassword;
            }
            else
            {
                proxyUserTb.Enabled = false;
                proxyUserTb.Text = _setting.Auth.UseProxyUsername = "";
                proxyPwTb.Enabled = false;
                proxyPwTb.Text = _setting.Auth.UseProxyPassword = "";
            }
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

        private static void ListSelectAllHandler(CheckedListBox targetList, bool setToValue)
        {
            for (var index = 0; index < targetList.Items.Count; index++)
            {
                targetList.SetItemChecked(index, setToValue);
            }
        }

        #endregion

        #region Events

        private void saveBtn_Click(object sender, EventArgs e)
        {
            #region Auth Settings

            _setting.Auth.AuthType = authTypeCb.Text == @"Google" ? AuthType.Google : AuthType.Ptc;
            if (_setting.Auth.AuthType == AuthType.Google)
            {
                _setting.Auth.GoogleUsername = UserLoginBox.Text;
                _setting.Auth.GooglePassword = UserPasswordBox.Text;
                _setting.Auth.PtcUsername = "";
                _setting.Auth.PtcPassword = "";
            }
            else
            {
                _setting.Auth.GoogleUsername = "";
                _setting.Auth.GooglePassword = "";
                _setting.Auth.PtcUsername = UserLoginBox.Text;
                _setting.Auth.PtcPassword = UserPasswordBox.Text;
            }

            _setting.Auth.GoogleApiKey = GoogleApiBox.Text;

            _setting.Auth.UseProxy = useProxyCb.Checked;
            _setting.Auth.UseProxyHost = proxyHostTb.Text;
            _setting.Auth.UseProxyPort = proxyPortTb.Text;
            _setting.Auth.UseProxyAuthentication = useProxyAuthCb.Checked;
            _setting.Auth.UseProxyUsername = proxyUserTb.Text;
            _setting.Auth.UseProxyPassword = proxyPwTb.Text;

            _setting.Auth.DeviceId = DeviceIdTb.Text;
            _setting.Auth.AndroidBoardName = AndroidBoardNameTb.Text;
            _setting.Auth.AndroidBootloader = AndroidBootloaderTb.Text;
            _setting.Auth.DeviceBrand = DeviceBrandTb.Text;
            _setting.Auth.DeviceModel = DeviceModelTb.Text;
            _setting.Auth.DeviceModelIdentifier = DeviceModelIdentifierTb.Text;
            _setting.Auth.DeviceModelBoot = DeviceModelBootTb.Text;
            _setting.Auth.HardwareManufacturer = HardwareManufacturerTb.Text;
            _setting.Auth.HardwareModel = HardwareModelTb.Text;
            _setting.Auth.FirmwareBrand = FirmwareBrandTb.Text;
            _setting.Auth.FirmwareTags = FirmwareTagsTb.Text;
            _setting.Auth.FirmwareType = FirmwareTypeTb.Text;
            _setting.Auth.FirmwareFingerprint = FirmwareFingerprintTb.Text;

            _setting.Auth.Save(AuthFilePath);

            #endregion

            #region Bot Settings

            _setting.TranslationLanguageCode = cbLanguage.Text;

            #region Location

            _setting.DefaultLatitude = ConvertStringToDouble(tbLatitude.Text);
            _setting.DefaultLongitude = ConvertStringToDouble(tbLongitude.Text);
            _setting.WalkingSpeedInKilometerPerHour = ConvertStringToInt(tbWalkingSpeed.Text);

            #endregion

            #region Pokemon

            #region Catch

            _setting.CatchPokemon = cbCatchPoke.Checked;
            _setting.UseEggIncubators = cbUseEggIncubators.Checked;
            _setting.MaxBerriesToUsePerPokemon = ConvertStringToInt(tBMaxBerriesToUsePerPokemon.Text);
            _setting.MaxPokeballsPerPokemon = ConvertStringToInt(tbMaxPokeballsPerPokemon.Text);
            _setting.PokemonsToIgnore = ConvertClbToList(clbIgnore);
            _setting.AutoFavoritePokemon = cbAutoFavoritePokemon.Checked;
            _setting.FavoriteMinIvPercentage = ConvertStringToFloat(tbFavoriteMinIvPercentage.Text);

            _setting.UseBerriesMinCp = ConvertStringToInt(tbUseBerriesMinCp.Text);
            _setting.UseBerriesMinIv = ConvertStringToFloat(tbUseBerriesMinIv.Text);
            _setting.UseBerriesBelowCatchProbability = ConvertStringToDouble(tbUseBerriesBelowCatchProbability.Text);
            _setting.UseBerriesOperator = cbUseBerriesOperator.SelectedIndex == 0 ? "and" : "or";

            _setting.UseGreatBallAboveCp = ConvertStringToInt(tbUseGreatBallAboveCp.Text);
            _setting.UseUltraBallAboveCp = ConvertStringToInt(tbUseUltraBallAboveCp.Text);
            _setting.UseMasterBallAboveCp = ConvertStringToInt(tbUseMasterBallAboveCp.Text);
            _setting.UseGreatBallAboveIv = ConvertStringToDouble(tbUseGreatBallAboveIv.Text);
            _setting.UseUltraBallAboveIv = ConvertStringToDouble(tbUseUltraBallAboveIv.Text);
            _setting.UseGreatBallBelowCatchProbability = ConvertStringToDouble(tbUseGreatBallBelowCatchProbability.Text);
            _setting.UseUltraBallBelowCatchProbability = ConvertStringToDouble(tbUseUltraBallBelowCatchProbability.Text);
            _setting.UseMasterBallBelowCatchProbability =
                ConvertStringToDouble(tbUseMasterBallBelowCatchProbability.Text);

            #endregion

            #region Transfer

            _setting.PrioritizeIvOverCp = cbPrioritizeIvOverCp.Checked;
            _setting.KeepMinCp = ConvertStringToInt(tbKeepMinCp.Text);
            _setting.KeepMinIvPercentage = ConvertStringToFloat(tbKeepMinIV.Text);
            _setting.KeepMinLvl = ConvertStringToInt(tbKeepMinLvl.Text);
            _setting.KeepMinOperator = cbKeepMinOperator.SelectedIndex == 0 ? "and" : "or";
            _setting.TransferWeakPokemon = cbTransferWeakPokemon.Checked;
            _setting.TransferDuplicatePokemon = cbTransferDuplicatePokemon.Checked;
            _setting.TransferDuplicatePokemonOnCapture = cbTransferDuplicatePokemonOnCapture.Checked;

            _setting.KeepMinDuplicatePokemon = ConvertStringToInt(tbKeepMinDuplicatePokemon.Text);
            _setting.UseKeepMinLvl = cbUseKeepMinLvl.Checked;
            _setting.PokemonsNotToTransfer = ConvertClbToList(clbTransfer);

            #endregion

            #region PowerUp

            _setting.UseLevelUpList = true;

            _setting.AutomaticallyLevelUpPokemon = cbAutoPowerUp.Checked;
            _setting.OnlyUpgradeFavorites = cbPowerUpFav.Checked;
            _setting.LevelUpByCPorIv = cbPowerUpType.SelectedIndex == 0 ? "iv" : "cp";
            _setting.UpgradePokemonMinimumStatsOperator = cbPowerUpCondiction.SelectedIndex == 0 ? "and" : "or";
            _setting.GetMinStarDustForLevelUp = ConvertStringToInt(cbPowerUpMinStarDust.Text);
            _setting.UpgradePokemonIvMinimum = ConvertStringToFloat(tbPowerUpMinIV.Text);
            _setting.UpgradePokemonCpMinimum = ConvertStringToFloat(tbPowerUpMinCP.Text);
            _setting.PokemonsToLevelUp = ConvertClbToList(clbPowerUp);

            #endregion

            #region Evo

            _setting.EvolveAllPokemonAboveIv = cbEvoAllAboveIV.Checked;
            _setting.EvolveAboveIvValue = ConvertStringToFloat(tbEvoAboveIV.Text);
            _setting.EvolveAllPokemonWithEnoughCandy = cbEvolveAllPokemonWithEnoughCandy.Checked;
            _setting.KeepPokemonsThatCanEvolve = cbKeepPokemonsThatCanEvolve.Checked;
            _setting.UseLuckyEggsWhileEvolving = cbUseLuckyEggsWhileEvolving.Checked;
            _setting.EvolveKeptPokemonsAtStorageUsagePercentage =
                ConvertStringToDouble(tbEvolveKeptPokemonsAtStorageUsagePercentage.Text);
            _setting.UseLuckyEggsMinPokemonAmount = ConvertStringToInt(tbUseLuckyEggsMinPokemonAmount.Text);
            _setting.PokemonsToEvolve = ConvertClbToList(clbEvolve);

            #endregion

            #endregion

            #region Item

            _setting.UseLuckyEggConstantly = cbUseLuckyEggConstantly.Checked;
            _setting.UseIncenseConstantly = cbUseIncenseConstantly.Checked;
            _setting.TotalAmountOfPokeballsToKeep = ConvertStringToInt(tbTotalAmountOfPokeballsToKeep.Text);
            _setting.TotalAmountOfPotionsToKeep = ConvertStringToInt(tbTotalAmountOfPotionsToKeep.Text);
            _setting.TotalAmountOfRevivesToKeep = ConvertStringToInt(tbTotalAmountOfRevivesToKeep.Text);
            _setting.TotalAmountOfBerriesToKeep = ConvertStringToInt(tbTotalAmountOfBerriesToKeep.Text);
            _setting.VerboseRecycling = cbVerboseRecycling.Checked;
            _setting.RecycleInventoryAtUsagePercentage = ConvertStringToDouble(tbRecycleInventoryAtUsagePercentage.Text);

            #endregion

            #region Advanced Settings

            _setting.DisableHumanWalking = cbDisableHumanWalking.Checked;
            _setting.UseWalkingSpeedVariant = cbUseWalkingSpeedVariant.Checked;
            _setting.WalkingSpeedVariant = ConvertStringToDouble(tbWalkingSpeedVariantInKilometerPerHour.Text);
            _setting.ShowVariantWalking = cbShowWalkingSpeed.Checked;
            _setting.MaxSpawnLocationOffset = ConvertStringToInt(tbMaxSpawnLocationOffset.Text);
            _setting.MaxTravelDistanceInMeters = ConvertStringToInt(tbMaxTravelDistanceInMeters.Text);

            _setting.DelayBetweenPlayerActions = ConvertStringToInt(tbDelayBetweenPlayerActions.Text);
            _setting.DelayBetweenPokemonCatch = ConvertStringToInt(tbDelayBetweenPokemonCatch.Text);
            _setting.DelayBetweenRecycleActions = cbDelayBetweenRecycleActions.Checked;

            _setting.RandomizeRecycle = cbRandomizeRecycle.Checked;
            _setting.RandomRecycleValue = ConvertStringToInt(tbRandomRecycleValue.Text);

            _setting.EnableHumanizedThrows = cbEnableHumanizedThrows.Checked;
            _setting.NiceThrowChance = ConvertStringToInt(tbNiceThrowChance.Text);
            _setting.GreatThrowChance = ConvertStringToInt(tbGreatThrowChance.Text);
            _setting.ExcellentThrowChance = ConvertStringToInt(tbExcellentThrowChance.Text);
            _setting.CurveThrowChance = ConvertStringToInt(tbCurveThrowChance.Text);
            _setting.ForceGreatThrowOverIv = ConvertStringToDouble(tbForceGreatThrowOverIv.Text);
            _setting.ForceExcellentThrowOverIv = ConvertStringToDouble(tbForceExcellentThrowOverIv.Text);
            _setting.ForceGreatThrowOverCp = ConvertStringToInt(tbForceGreatThrowOverCp.Text);
            _setting.ForceExcellentThrowOverCp = ConvertStringToInt(tbForceExcellentThrowOverCp.Text);

            #endregion

            _setting.Save(ConfigFilePath);

            #endregion

            Close();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void trackBar_Scroll(object sender, EventArgs e)
        {
            gMapCtrl.Zoom = trackBar.Value;
        }

        private void gMapCtrl_MouseUp(object sender, MouseEventArgs e)
        {
            UpdateLocationInfo();
        }

        private void gMapCtrl_OnMapZoomChanged()
        {
            UpdateLocationInfo();
        }

        private void ResetLocationBtn_Click(object sender, EventArgs e)
        {
            gMapCtrl.Zoom = trackBar.Value = DefaultZoomLevel;
            UpdateMapLocation(_setting.DefaultLatitude, _setting.DefaultLongitude);
        }

        private void gMapCtrl_MouseClick(object sender, MouseEventArgs e)
        {
            var localCoordinates = e.Location;
            gMapCtrl.Position = gMapCtrl.FromLocalToLatLng(localCoordinates.X, localCoordinates.Y);
        }

        private void latitudeText_Leave(object sender, EventArgs e)
        {
            UpdateMapLocation(ConvertStringToDouble(tbLatitude.Text), ConvertStringToDouble(tbLongitude.Text));
        }

        private void longitudeText_Leave(object sender, EventArgs e)
        {
            UpdateMapLocation(ConvertStringToDouble(tbLatitude.Text), ConvertStringToDouble(tbLongitude.Text));
        }

        private void AdressBox_Enter(object sender, EventArgs e)
        {
            if (AdressBox.Text != @"Enter an address or a coordinate")
            {
                return;
            }
            AdressBox.Text = "";
        }

        private void AdressBox_Leave(object sender, EventArgs e)
        {
            if (AdressBox.Text != string.Empty)
            {
                return;
            }
            AdressBox.Text = @"Enter an address or a coordinate";
        }

        private void AdressBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter)
            {
                return;
            }
            gMapCtrl.SetPositionByKeywords(AdressBox.Text);
            gMapCtrl.Zoom = DefaultZoomLevel;
            UpdateLocationInfo();
        }

        private void FindAdressBtn_Click(object sender, EventArgs e)
        {
            gMapCtrl.SetPositionByKeywords(AdressBox.Text);
            gMapCtrl.Zoom = DefaultZoomLevel;
            UpdateLocationInfo();
        }

        private void RandomIDBtn_Click(object sender, EventArgs e)
        {
            DeviceIdTb.Text = _deviceHelper.RandomString(16, "0123456789abcdef");
        }

        private void RandomDeviceBtn_Click(object sender, EventArgs e)
        {
            PopulateDevice();
        }

        private void useProxyCb_CheckedChanged(object sender, EventArgs e)
        {
            ToggleProxyCtrls();
        }

        private void useProxyAuthCb_CheckedChanged(object sender, EventArgs e)
        {
            ToggleProxyCtrls();
        }

        private void deviceTypeCb_SelectionChangeCommitted(object sender, EventArgs e)
        {
            PopulateDevice(deviceTypeCb.SelectedIndex);
        }

        private void cbPowerUpAll_CheckedChanged(object sender, EventArgs e)
        {
            ListSelectAllHandler(clbPowerUp, cbPowerUpAll.Checked);
        }

        private void cbPowerUpType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cbPowerUpType.Text.ToLowerInvariant() == "iv")
            {
                label31.Visible = true;
                tbPowerUpMinIV.Visible = true;
                label30.Visible = false;
                tbPowerUpMinCP.Visible = false;
            }
            else
            {
                label31.Visible = false;
                tbPowerUpMinIV.Visible = false;
                label30.Visible = true;
                tbPowerUpMinCP.Visible = true;
            }
        }

        private void cbSelectAllEvolve_CheckedChanged(object sender, EventArgs e)
        {
            ListSelectAllHandler(clbEvolve, cbEvolveAll.Checked);
        }

        private void cbSelectAllCatch_CheckedChanged(object sender, EventArgs e)
        {
            ListSelectAllHandler(clbIgnore, cbIgnoreAll.Checked);
        }

        private void cbSelectAllTransfer_CheckedChanged(object sender, EventArgs e)
        {
            ListSelectAllHandler(clbTransfer, cbNotTransferAll.Checked);
        }

        private void enableAdvSettingCb_Click(object sender, EventArgs e)
        {
            proxyGb.Visible = _setting.EnableAdvancedSettings = enableAdvSettingCb.Checked;
            if (enableAdvSettingCb.Checked)
            {
                _tabAdvSettingTab.Enabled = true;
                tabControl.TabPages.Add(_tabAdvSettingTab);
            }
            else
            {
                _tabAdvSettingTab.Enabled = false;
                tabControl.TabPages.Remove(_tabAdvSettingTab);
            }
        }

        #endregion

        private void latLabel_Click(object sender, EventArgs e)
        {

        }

        private void tbLatitude_TextChanged(object sender, EventArgs e)
        {

        }
    }
}